using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using OrdersService.Core.Enums;

namespace OrdersService.Core.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        [Required, StringLength(100)]
        public string ShippingFullName { get; set; } = string.Empty;

        [Required, StringLength(13)]
        public string ShippingPhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string ShippingAddressLine1 { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ShippingAddressLine2 { get; set; }

        [Required, StringLength(100)]
        public string ShippingCity { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string ShippingState { get; set; } = string.Empty;

        [Required, StringLength(6)]
        public string ShippingPinCode { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Landmark { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
