using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
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

        public async Task<Product> AddProduct(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetProductById(Guid productId)
        {
            return await _dbContext.Products
                                   .FirstOrDefaultAsync(t=>t.Id == productId);
        }

        public async Task<bool> IsProductExist(Guid productId)
        {
            return await _dbContext.Products
                                   .AnyAsync(t=>t.Id == productId);
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var matchingProduct = await GetProductById(product.Id);
            matchingProduct.Description = product.Description;
            matchingProduct.Name = product.Name;
            matchingProduct.UnitPrice = product.UnitPrice;
            matchingProduct.ProductImagePath = product.ProductImagePath;
            matchingProduct.QuantityInStock = product.QuantityInStock;

            await _dbContext.SaveChangesAsync();
            return matchingProduct;
        }
    }
}
