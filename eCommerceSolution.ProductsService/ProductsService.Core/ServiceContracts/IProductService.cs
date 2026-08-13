using ProductsService.Core.DTO;

namespace ProductsService.Core.ServiceContracts
{
    public interface IProductService
    {
        Task<ProductResponse> AddProductAsync(ProductAddRequest productAddRequest);
        Task<ProductResponse> GetProductByIdAsync(Guid productId);
        Task<ProductResponse> GetProductByIdAdminAsync(Guid productId);
        Task<ProductResponse> UpdateProductAsync(Guid productId, ProductUpdateRequest request);
        Task<ProductResponse> UpdateProductStatusAsync(Guid productId, bool isActive);
        Task<bool> IsProductExistAsync(Guid productId);
    }
}
