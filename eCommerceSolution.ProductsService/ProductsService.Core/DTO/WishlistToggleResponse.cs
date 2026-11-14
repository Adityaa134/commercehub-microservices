namespace ProductsService.Core.DTO
{
    public class WishlistToggleResponse
    {
        public Guid ProductId { get; set; }
        public bool IsInWishlist { get; set; }
    }
}

