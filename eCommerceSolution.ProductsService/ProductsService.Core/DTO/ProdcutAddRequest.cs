using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductsService.Core.CustomValidators;

namespace ProductsService.Core.DTO
{
    public class ProdcutAddRequest
    {
        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(200)]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Product Description is required")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Product Status is required")]
        public bool Status { get; set; }

        [Required(ErrorMessage = "Product Price is required")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Product Category is required")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Product Quantity is required")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Product Image is required")]
        [ValidatingFileType]
        public IFormFile? ProductImage { get; set; }
    }
}
