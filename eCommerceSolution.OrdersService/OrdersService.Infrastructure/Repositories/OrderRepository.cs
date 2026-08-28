using Microsoft.EntityFrameworkCore;
using OrdersService.Core.Domain.Entities;
using OrdersService.Core.Domain.RepositoryContracts;
using OrdersService.Core.DTO;
using OrdersService.Infrastructure.DBContext;

namespace OrdersService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    UserId = o.UserId,
                    AddressId = o.AddressId,
                    ShippingFullName = o.ShippingFullName,
                    ShippingPhoneNumber = o.ShippingPhoneNumber,
                    ShippingAddressLine1 = o.ShippingAddressLine1,
                    ShippingAddressLine2 = o.ShippingAddressLine2,
                    ShippingCity = o.ShippingCity,
                    ShippingState = o.ShippingState,
                    ShippingPinCode = o.ShippingPinCode,
                    Landmark = o.Landmark,
                    TotalPrice = o.TotalPrice,
                    OrderStatus = o.OrderStatus.ToString(),
                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductImageUrl = oi.ProductImageUrl,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.UnitPrice * oi.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<OrderResponse>?> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    UserId = o.UserId,
                    AddressId = o.AddressId,
                    ShippingFullName = o.ShippingFullName,
                    ShippingPhoneNumber = o.ShippingPhoneNumber,
                    ShippingAddressLine1 = o.ShippingAddressLine1,
                    ShippingAddressLine2 = o.ShippingAddressLine2,
                    ShippingCity = o.ShippingCity,
                    ShippingState = o.ShippingState,
                    ShippingPinCode = o.ShippingPinCode,
                    Landmark = o.Landmark,
                    TotalPrice = o.TotalPrice,
                    OrderStatus = o.OrderStatus.ToString(),
                    OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductImageUrl = oi.ProductImageUrl,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        TotalPrice = oi.UnitPrice * oi.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> IsOrderExistAsync(Guid orderId)
        {
            return await _dbContext.Orders.AnyAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderEntityByIdAsync(Guid orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }
    }
}
