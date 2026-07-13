using ProductsService.Core.Domain.Entities;

namespace ProductsService.Core.Domain.RepositoryContracts
{
    public interface ICategoryRepository
    {
        Task<Category> AddCategory(Category category);
        Task<List<Category>> GetActiveCategories();
        Task<List<Category>> GetAllCategoriesForAdmin();
        Task<Category?> GetCategoryById(Guid categoryId);
        Task<bool> IsCategoryExist(Guid categoryId);
    }
}
