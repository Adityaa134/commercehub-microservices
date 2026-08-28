using System.ComponentModel.DataAnnotations;

namespace OrdersService.Core.DTO
{
    public class OrderItemAddRequest
    {
        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 5, ErrorMessage = "Quantity must be between 1 and 5")]
        public int Quantity { get; set; }
    }
}
