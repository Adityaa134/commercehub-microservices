namespace ProductsService.Core.DTO
{
    public class CategoryResponse
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryImagePath { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public List<CategoryResponse> SubCategories { get; set; } = new List<CategoryResponse>();
    }
}
