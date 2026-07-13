namespace ProductsService.Core.DTO
{
    public class ProductResponse
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? ProductImagePath { get; set; }
        public bool Status { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid CategoryId { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
}
