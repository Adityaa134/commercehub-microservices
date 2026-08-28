using OrdersService.Core.DTO;

namespace OrdersService.Core.ServiceContracts
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(Guid userId, OrderAddRequest orderAddRequest);
        Task<OrderResponse> GetOrderByIdAsync(Guid userId, Guid orderId);
        Task<List<OrderResponse>> GetOrdersByUserIdAsync(Guid userId);
        Task<bool> IsOrderExistAsync(Guid orderId);
        Task<OrderResponse> CancelOrderAsync(Guid userId, Guid orderId);
    }
}
