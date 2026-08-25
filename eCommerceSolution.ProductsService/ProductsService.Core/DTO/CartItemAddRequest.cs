using System.ComponentModel.DataAnnotations;

namespace ProductsService.Core.DTO
{
    public class CartItemAddRequest
    {
        [Required(ErrorMessage = "Product Id is required")]
        public Guid ProductId { get; set; }

        [Range(1, 5, ErrorMessage = "Quantity must be between 1 and 5")]
        public int Quantity { get; set; } = 1;
    }
}
