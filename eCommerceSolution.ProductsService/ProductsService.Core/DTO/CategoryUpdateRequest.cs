using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductsService.Core.CustomValidators;

namespace ProductsService.Core.DTO
{
    public class CategoryUpdateRequest
    {
        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(200, ErrorMessage = "Category Name cannot exceed 200 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        public Guid? ParentCategoryId { get; set; }

        [ValidatingFileType(isRequiredForNewImage: false)]
        public IFormFile? CategoryImage { get; set; }
    }
}
