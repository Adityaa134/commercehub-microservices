using ProductsService.Core.Domain.Entities;
using ProductsService.Core.DTO;

namespace ProductsService.Core.Domain.RepositoryContracts
{
    public interface IProductRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<Product?> GetProductByIdAdminAsync(Guid productId);
        Task<List<ProductBatchResponse>> GetProductsByIdsAsync(List<Guid> productIds);
        Task<bool> IsProductExistAsync(Guid productId);
        Task<bool> IsProductNameExistAsync(string productName, Guid? excludeProductId = null);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> UpdateProductStatusAsync(Guid productId, bool isActive);
    }
}
