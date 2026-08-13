using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductsService.Core.CustomValidators;

namespace ProductsService.Core.DTO
{
    public class CategoryAddRequest
    {
        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(200, ErrorMessage = "Category Name cannot exceed 200 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Status is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Category Image is required")]
        [ValidatingFileType]
        public IFormFile CategoryImage { get; set; } = null!;
    }
}
