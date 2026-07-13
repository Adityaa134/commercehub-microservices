using OrdersService.Core.Enums;

namespace OrdersService.Core.DTO
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public  string? OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemResponse>? OrderItems { get; set; }
    }
}
