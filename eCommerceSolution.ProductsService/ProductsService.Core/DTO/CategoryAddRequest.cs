using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ProductsService.Core.CustomValidators;

namespace ProductsService.Core.DTO
{
    public class CategoryAddRequest : IValidatableObject
    {
        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(200, ErrorMessage = "Category Name cannot exceed 200 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Status is required")]
        public bool IsActive { get; set; }

        public Guid? ParentCategoryId { get; set; }

        [ValidatingFileType(isRequiredForNewImage: false)]
        public IFormFile? CategoryImage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ParentCategoryId == null && CategoryImage == null)
            {
                yield return new ValidationResult(
                    "Category Image is required for parent categories.",
                    new[] { nameof(CategoryImage) }
                );
            }
        }
    }
}
