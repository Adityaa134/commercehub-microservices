using OrdersService.Core.Domain.Entities;
using OrdersService.Core.DTO;

namespace OrdersService.Core.Domain.RepositoryContracts
{
    public interface IOrderRepository
    {
        Task<List<OrderResponse>?> GetOrdersByUserId(Guid userId);
        Task<bool> IsOrderExist(Guid orderId);
        Task<Order> CreateOrder(Order order);
        Task<OrderResponse?> GetOrderById(Guid orderId);
    }
}
