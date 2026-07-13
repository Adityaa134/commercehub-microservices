using ProductsService.Core.Domain.Entities;

namespace ProductsService.Core.Domain.RepositoryContracts
{
    public interface IProductRepository
    {
        Task<Product> AddProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task<Product?> GetProductById(Guid productId);
        Task<bool> IsProductExist(Guid productId);
    }
}
