using Microsoft.EntityFrameworkCore;
using OrdersService.Core.Enums;

namespace OrdersService.Core.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
