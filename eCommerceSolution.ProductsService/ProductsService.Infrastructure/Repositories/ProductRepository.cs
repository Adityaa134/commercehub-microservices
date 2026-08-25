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
    }
}
