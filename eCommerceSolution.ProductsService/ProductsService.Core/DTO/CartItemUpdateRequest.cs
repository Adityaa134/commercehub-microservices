using System.ComponentModel.DataAnnotations;

namespace ProductsService.Core.DTO
{
    public class CartItemUpdateRequest
    {
        [Required(ErrorMessage = "Product Id is required")]
        public Guid ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }
}
