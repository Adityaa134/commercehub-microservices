using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Core.DTO;
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

        public async Task<List<CategoryResponse>> GetCategoriesAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == null && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(parent => new CategoryResponse
                {
                    CategoryId = parent.Id,
                    CategoryName = parent.Name,
                    Description = parent.Description,
                    CategoryImagePath = parent.CategoryImagePath,
                    IsActive = parent.IsActive,
                    ParentCategoryId = parent.ParentCategoryId,
                    DisplayOrder = parent.DisplayOrder,
                    SubCategories = parent.SubCategories
                        .Where(sc => sc.IsActive)
                        .OrderBy(sc => sc.DisplayOrder)
                        .Select(sub => new CategoryResponse
                        {
                            CategoryId = sub.Id,
                            CategoryName = sub.Name,
                            Description = sub.Description,
                            IsActive = sub.IsActive,
                            ParentCategoryId = sub.ParentCategoryId,
                            DisplayOrder = sub.DisplayOrder
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<CategoryResponse>> GetCategoriesAdminAsync()
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == null)
                .OrderBy(c => c.DisplayOrder)
                .Select(parent => new CategoryResponse
                {
                    CategoryId = parent.Id,
                    CategoryName = parent.Name,
                    Description = parent.Description,
                    CategoryImagePath = parent.CategoryImagePath,
                    IsActive = parent.IsActive,
                    ParentCategoryId = parent.ParentCategoryId,
                    DisplayOrder = parent.DisplayOrder,
                    SubCategories = parent.SubCategories
                        .OrderBy(sc => sc.DisplayOrder)
                        .Select(sub => new CategoryResponse
                        {
                            CategoryId = sub.Id,
                            CategoryName = sub.Name,
                            Description = sub.Description,
                            IsActive = sub.IsActive,
                            ParentCategoryId = sub.ParentCategoryId,
                            DisplayOrder = sub.DisplayOrder
                        }).ToList()
                })
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
