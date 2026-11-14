namespace ProductsService.Core.DTO
{
    public class WishlistResponse
    {
        public Guid UserId { get; set; }
        public int TotalItems => Items.Count;
        public List<WishlistItemResponse> Items { get; set; } = new();
    }
}

