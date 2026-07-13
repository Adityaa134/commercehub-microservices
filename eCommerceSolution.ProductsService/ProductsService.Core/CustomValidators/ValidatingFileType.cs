using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProductsService.Core.CustomValidators
{
    public class ValidatingFileType : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg",".jpeg",".png" };
        private readonly int _maxFileSizeMb = 5;
        private readonly bool _isRequiredForNewImage;
        public ValidatingFileType(bool isRequiredForNewImage = true)
        {
            _isRequiredForNewImage = isRequiredForNewImage;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if(file == null || file.Length == 0)
            {
                if(IsNewProduct(validationContext) && _isRequiredForNewImage)
                {
                    return new ValidationResult("Dish image is required for new products");
                }
                return ValidationResult.Success;

            }
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                return new ValidationResult("Only .jpg, .jpeg, .png types are allowed");
            }

            if(file.Length > _maxFileSizeMb * 1024  * 1024)
            {
                return new ValidationResult($"File size must be less than {_maxFileSizeMb} MB");
            }
            return ValidationResult.Success;
        }

        private bool IsNewProduct(ValidationContext context)
        {
            var idProperty =  context.ObjectType.GetProperty("ProductId");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(context.ObjectInstance);
                return idValue == null || idValue.Equals(0);
            }
            return true;
        }
    }
}
