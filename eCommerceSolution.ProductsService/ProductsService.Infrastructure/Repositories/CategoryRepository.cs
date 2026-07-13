using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Infrastructure.DBContext;

namespace ProductsService.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> AddCategory(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetActiveCategories()
        {
            return await _dbContext.Categories
                                   .AsNoTracking()
                                   .Where(t => t.IsActive == true)
                                   .ToListAsync();
        }

        public async Task<List<Category>> GetAllCategoriesForAdmin()
        {
            return await _dbContext.Categories
                                   .AsNoTracking()
                                   .ToListAsync();
        }

        public async Task<Category?> GetCategoryById(Guid categoryId)
        {
          return await _dbContext.Categories
                                 .FirstOrDefaultAsync(t=>t.Id == categoryId);
        }

        public async Task<bool> IsCategoryExist(Guid categoryId)
        {
            return await _dbContext.Categories
                                   .AnyAsync(t => t.Id == categoryId);
        }
    }
}
