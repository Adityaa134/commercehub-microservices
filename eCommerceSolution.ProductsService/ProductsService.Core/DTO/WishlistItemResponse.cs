namespace ProductsService.Core.DTO
{
    public class WishlistItemResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string? ProductImagePath { get; set; }
        public string? CategoryName { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public bool InStock { get; set; }
        public DateTime AddedAt { get; set; }
    }
}

