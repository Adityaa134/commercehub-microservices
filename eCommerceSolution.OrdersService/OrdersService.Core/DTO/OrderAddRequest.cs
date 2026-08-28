using System.ComponentModel.DataAnnotations;

namespace OrdersService.Core.DTO
{
    public class OrderAddRequest
    {

        [Required(ErrorMessage = "AddressId is required")]
        public Guid AddressId { get; set; }

        [Required(ErrorMessage = "OrderItems is required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<OrderItemAddRequest> OrderItems { get; set; }
    }
}
