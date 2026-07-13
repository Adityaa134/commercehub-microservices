using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductsService.Core.Domain.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string ProductImagePath { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        [Precision(18,2)]
        public decimal UnitPrice { get; set; }
        public Guid CategoryId { get; set; }

        [Precision(18, 2)]
        public decimal AverageRating { get; set; } = 0;
        public int TotalRatings { get; set; } = 0;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
