using ProductsService.Core.DTO;

namespace ProductsService.Core.ServiceContracts
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategoryAsync(CategoryAddRequest categoryAddRequest);
        Task<List<CategoryResponse>> GetCategoriesAsync();
        Task<List<CategoryResponse>> GetCategoriesAdminAsync();
        Task<CategoryResponse> GetCategoryByIdAsync(Guid categoryId);
        Task<CategoryResponse> GetCategoryByIdAdminAsync(Guid categoryId);
        Task<bool> IsCategoryExistAsync(Guid categoryId);
        Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, CategoryUpdateRequest request);
        Task<CategoryResponse> UpdateCategoryStatusAsync(Guid categoryId, bool isActive);
    }
}
