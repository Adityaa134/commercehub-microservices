using OrdersService.Core.Domain.Entities;
using OrdersService.Core.DTO;

namespace OrdersService.Core.Domain.RepositoryContracts
{
    public interface IOrderRepository
    {
        Task<List<OrderResponse>?> GetOrdersByUserIdAsync(Guid userId);
        Task<bool> IsOrderExistAsync(Guid orderId);
        Task<Order> CreateOrderAsync(Order order);
        Task<OrderResponse?> GetOrderByIdAsync(Guid orderId);
        Task<Order?> GetOrderEntityByIdAsync(Guid orderId);
        Task<Order> UpdateOrderAsync(Order order);
    }
}
