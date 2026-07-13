using System.ComponentModel.DataAnnotations;

namespace OrdersService.Core.DTO
{
    public class OrderItemAddRequest
    {
        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "UnitPrice is required")]
        public decimal UnitPrice { get; set; }
    }
}
