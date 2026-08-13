using System.ComponentModel.DataAnnotations;

namespace ProductsService.Core.DTO
{
    public class ProductStatusUpdateRequest
    {
        [Required(ErrorMessage = "Status is required")]
        public bool IsActive { get; set; }
    }
}
