using ProductsService.Core.Domain.Entities;
using ProductsService.Core.DTO;

namespace ProductsService.Core.Domain.RepositoryContracts
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(Category category);
        Task<List<CategoryResponse>> GetCategoriesAsync();
        Task<List<CategoryResponse>> GetCategoriesAdminAsync();
        Task<Category?> GetCategoryByIdAsync(Guid categoryId);
        Task<Category?> GetCategoryByIdAdminAsync(Guid categoryId);
        Task<bool> IsCategoryExistAsync(Guid categoryId);
        Task<bool> IsCategoryNameExistAsync(string categoryName, Guid? excludeCategoryId = null);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> UpdateCategoryStatusAsync(Guid categoryId, bool isActive);
    }
}
