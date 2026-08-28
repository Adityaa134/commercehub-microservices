using OrdersService.Core.Enums;

namespace OrdersService.Core.DTO
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public string ShippingFullName { get; set; } = string.Empty;
        public string ShippingPhoneNumber { get; set; } = string.Empty;
        public string ShippingAddressLine1 { get; set; } = string.Empty;
        public string? ShippingAddressLine2 { get; set; }
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingState { get; set; } = string.Empty;
        public string ShippingPinCode { get; set; } = string.Empty;
        public string? Landmark { get; set; }
        public  string? OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemResponse>? OrderItems { get; set; }
    }
}
