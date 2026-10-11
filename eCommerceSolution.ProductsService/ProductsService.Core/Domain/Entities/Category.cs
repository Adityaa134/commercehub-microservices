using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductsService.Core.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public string? CategoryImagePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; } = 0;
        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
