using AutoMapper;
using OrdersService.Core.Domain.Entities;
using OrdersService.Core.Domain.RepositoryContracts;
using OrdersService.Core.DTO;
using OrdersService.Core.Enums;
using OrdersService.Core.Exceptions;
using OrdersService.Core.ServiceContracts;

namespace OrdersService.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IProductServiceClient _productServiceClient;

        public OrderService(IMapper mapper, IOrderRepository orderRepository, IUserServiceClient userServiceClient, IProductServiceClient productServiceClient)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _userServiceClient = userServiceClient;
            _productServiceClient = productServiceClient;
        }

        public async Task<OrderResponse> CreateOrderAsync(Guid userId, OrderAddRequest orderAddRequest)
        {
            List<Guid> productIds = orderAddRequest.OrderItems.Select(i => i.ProductId).ToList();

            Task<AddressResponse?> addressTask = _userServiceClient.GetAddressByIdAsync(orderAddRequest.AddressId);
            Task<List<ProductResponse>> productsTask = _productServiceClient.GetProductsByIdsAsync(productIds);

            await Task.WhenAll(addressTask, productsTask);

            AddressResponse? address = await addressTask;
            List<ProductResponse> products = await productsTask;

            if (address == null)
                throw new NotFoundException("Address was not found.");

            if (address.UserId != userId)
                throw new UnauthorizedOperationException("The address is invalid.");

            Dictionary<Guid, ProductResponse> productMap = products.ToDictionary(p => p.ProductId);

            Order order = _mapper.Map<Order>(orderAddRequest);
            order.UserId = userId;
            _mapper.Map(address, order);

            foreach (OrderItem orderItem in order.OrderItems)
            {
                if (!productMap.TryGetValue(orderItem.ProductId, out ProductResponse? product))
                    throw new NotFoundException("Product was not found.");

                if (!product.IsActive)
                    throw new BusinessValidationException($"'{product.ProductName}' is currently unavailable.");

                if (product.QuantityInStock < orderItem.Quantity)
                    throw new BusinessValidationException($"Not enough stock available for '{product.ProductName}'. Please reduce the quantity.");

                orderItem.ProductName = product.ProductName;
                orderItem.ProductImageUrl = product.ProductImageUrl;
                orderItem.UnitPrice = product.UnitPrice;
            }

            order.TotalPrice = order.OrderItems.Sum(item => item.UnitPrice * item.Quantity);

            Order createdOrder = await _orderRepository.CreateOrderAsync(order);
            return _mapper.Map<OrderResponse>(createdOrder);
        }

        public async Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new BusinessValidationException("Order ID cannot be empty.");

            OrderResponse? orderResponse = await _orderRepository.GetOrderByIdAsync(orderId);
            if (orderResponse == null || orderResponse.UserId != userId)
                throw new NotFoundException("Order was not found.");

            return orderResponse;
        }

        public async Task<List<OrderResponse>> GetOrdersByUserIdAsync(Guid userId)
        {
            List<OrderResponse>? orderResponses = await _orderRepository.GetOrdersByUserIdAsync(userId);
            return orderResponses ?? new List<OrderResponse>();
        }

        public async Task<bool> IsOrderExistAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new BusinessValidationException("Order ID cannot be empty.");

            return await _orderRepository.IsOrderExistAsync(orderId);
        }

        public async Task<OrderResponse> CancelOrderAsync(Guid userId, Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new BusinessValidationException("Order ID cannot be empty.");

            Order? order = await _orderRepository.GetOrderEntityByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Order was not found.");

            if (order.UserId != userId)
                throw new UnauthorizedOperationException("You do not have permission to cancel this order.");

            if (order.OrderStatus == OrderStatus.Cancelled)
                throw new BusinessValidationException("This order is already cancelled.");

            if (order.OrderStatus == OrderStatus.Shipped || order.OrderStatus == OrderStatus.Delivered)
                throw new BusinessValidationException("This order cannot be cancelled as it has already been shipped or delivered.");

            order.OrderStatus = OrderStatus.Cancelled;

            Order updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            return _mapper.Map<OrderResponse>(updatedOrder);
        }
    }
}
