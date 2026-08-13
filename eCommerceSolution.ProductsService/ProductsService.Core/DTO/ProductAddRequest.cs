using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductsService.Core.CustomValidators;

namespace ProductsService.Core.DTO
{
    public class ProductAddRequest
    {
        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(200, ErrorMessage = "Product Name cannot exceed 200 characters")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Status is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Product Price is required")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Product Price must be greater than 0")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Product Category is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Product Quantity is required")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Product Image is required")]
        [ValidatingFileType(isRequiredForNewImage: true)]
        public IFormFile ProductImage { get; set; } = null!;
    }
}
