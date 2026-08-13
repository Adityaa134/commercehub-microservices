using ProductsService.Core.Domain.Entities;

namespace ProductsService.Core.Domain.RepositoryContracts
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(Category category);
        Task<List<Category>> GetCategoriesAsync();
        Task<List<Category>> GetCategoriesAdminAsync();
        Task<Category?> GetCategoryByIdAsync(Guid categoryId);
        Task<Category?> GetCategoryByIdAdminAsync(Guid categoryId);
        Task<bool> IsCategoryExistAsync(Guid categoryId);
        Task<bool> IsCategoryNameExistAsync(string categoryName, Guid? excludeCategoryId = null);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> UpdateCategoryStatusAsync(Guid categoryId, bool isActive);
    }
}
