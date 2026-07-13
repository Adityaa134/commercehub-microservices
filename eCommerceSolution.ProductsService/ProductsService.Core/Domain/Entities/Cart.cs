using System.ComponentModel.DataAnnotations;

namespace ProductsService.Core.Domain.Entities
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
