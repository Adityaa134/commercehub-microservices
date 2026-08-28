using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrdersService.Core.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
