namespace ProductsService.Core.DTO
{
    public class ProductBatchResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int QuantityInStock { get; set; }
        public bool IsActive { get; set; }
    }
}
