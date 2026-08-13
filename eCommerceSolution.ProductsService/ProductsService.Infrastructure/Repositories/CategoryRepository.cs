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

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .Where(c => c.IsActive == true)
                .ToListAsync();
        }

        public async Task<List<Category>> GetCategoriesAdminAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.IsActive == true);
        }

        public async Task<Category?> GetCategoryByIdAdminAsync(Guid categoryId)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<bool> IsCategoryExistAsync(Guid categoryId)
        {
            return await _dbContext.Categories
                .AnyAsync(c => c.Id == categoryId);
        }

        public async Task<bool> IsCategoryNameExistAsync(string categoryName, Guid? excludeCategoryId = null)
        {
            return await _dbContext.Categories
                .AnyAsync(c => c.Name.ToLower() == categoryName.ToLower() &&
                              (!excludeCategoryId.HasValue || c.Id != excludeCategoryId.Value));
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryStatusAsync(Guid categoryId, bool isActive)
        {
            Category? category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
                return false;

            category.IsActive = isActive;
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
