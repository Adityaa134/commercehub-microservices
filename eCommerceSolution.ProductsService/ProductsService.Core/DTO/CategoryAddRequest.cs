using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductsService.Core.CustomValidators;

namespace ProductsService.Core.DTO
{
    public class CategoryAddRequest
    {
        [Required(ErrorMessage ="Category Name is required")]
        [StringLength(200)]
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Category Description is required")]
        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category Status is required")]
        public bool Status { get; set; }

        [Required(ErrorMessage = "Category Image is required")]
        [ValidatingFileType]
        public IFormFile? CategoryImage { get; set; }
    }
}
