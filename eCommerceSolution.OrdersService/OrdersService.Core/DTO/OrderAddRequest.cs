using System.ComponentModel.DataAnnotations;

namespace OrdersService.Core.DTO
{
    public class OrderAddRequest
    {
        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "AddressId is required")]
        public Guid AddressId { get; set; }

        [Required(ErrorMessage = "OrderDate is required")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "OrderItems is required")]
        public List<OrderItemAddRequest> OrderItems { get; set; }
    }
}
