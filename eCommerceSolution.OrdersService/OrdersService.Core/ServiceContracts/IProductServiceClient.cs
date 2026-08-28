using OrdersService.Core.DTO;

namespace OrdersService.Core.ServiceContracts
{
    public interface IProductServiceClient
    {
        Task<List<ProductResponse>> GetProductsByIdsAsync(List<Guid> productIds);
    }
}
