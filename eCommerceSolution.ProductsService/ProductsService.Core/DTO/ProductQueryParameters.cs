namespace ProductsService.Core.DTO
{
    public class ProductQueryParameters
    {
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 12;
    }
}
