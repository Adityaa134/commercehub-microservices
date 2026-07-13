namespace ProductsService.Core.DTO
{
    public class CategoryResponse
    {
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public string? CategoryImagePath { get; set; }
        public bool Status { get; set; }
    }
}
