using System.Net.Http.Json;
using OrdersService.Core.DTO;
using OrdersService.Core.ServiceContracts;

namespace OrdersService.Infrastructure.HttpClients
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProductResponse>> GetProductsByIdsAsync(List<Guid> productIds)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "api/Product/batch",productIds);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<ProductResponse>>() ?? new List<ProductResponse>();
        }
    }
}
