using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Core.DTO;
using ProductsService.Infrastructure.DBContext;

namespace ProductsService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive && p.Category.IsActive);
        }

        public async Task<Product?> GetProductByIdAdminAsync(Guid productId)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<bool> IsProductExistAsync(Guid productId)
        {
            return await _dbContext.Products.AnyAsync(p => p.Id == productId);
        }

        public async Task<bool> IsProductNameExistAsync(string productName, Guid? excludeProductId = null)
        {
            string name = productName.Trim().ToLower();
            return await _dbContext.Products
                .AnyAsync(p => p.Name.ToLower() == name && (!excludeProductId.HasValue || p.Id != excludeProductId.Value));
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateProductStatusAsync(Guid productId, bool isActive)
        {
            Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
                return false;

            product.IsActive = isActive;
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductBatchResponse>> GetProductsByIdsAsync(List<Guid> productIds)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id) && p.IsActive && p.Category.IsActive)
                .Select(p => new ProductBatchResponse
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ProductImageUrl = p.ProductImagePath,
                    UnitPrice = p.UnitPrice,
                    QuantityInStock = p.QuantityInStock,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<(List<Product> Products, int TotalCount)> GetProductsAsync(ProductQueryParameters parameters)
        {
            IQueryable<Product> query = _dbContext.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.IsActive && p.Category.IsActive);

            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim();
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{search}%") || EF.Functions.Like(p.Description, $"%{search}%"));
            }

            if (parameters.CategoryId.HasValue && parameters.CategoryId.Value != Guid.Empty)
            {
                query = query.Where(p => p.CategoryId == parameters.CategoryId.Value);
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice <= parameters.MaxPrice.Value);
            }

            if (parameters.MinRating.HasValue)
            {
                query = query.Where(p => p.AverageRating >= parameters.MinRating.Value);
            }

            int totalCount = await query.CountAsync();

            query = (parameters.SortBy?.Trim().ToLowerInvariant()) switch
            {
                "price_asc" or "price-low" => query.OrderBy(p => p.UnitPrice),
                "price_desc" or "price-high" => query.OrderByDescending(p => p.UnitPrice),
                "rating_desc" or "rating" or "trending" => query.OrderByDescending(p => p.AverageRating).ThenByDescending(p => p.TotalRatings),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            List<Product> products = await query
                .Skip((parameters.Page - 1) * parameters.Limit)
                .Take(parameters.Limit)
                .ToListAsync();

            return (products, totalCount);
        }
    }
}
