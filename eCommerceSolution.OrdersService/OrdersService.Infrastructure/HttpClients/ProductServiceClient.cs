using System.Net.Http.Json;
using OrdersService.Core.DTO;
using OrdersService.Core.Exceptions;
using OrdersService.Core.ServiceContracts;
using Polly.CircuitBreaker;
using Polly.Timeout;

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
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                    "api/Product/batch", productIds);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<List<ProductResponse>>() ?? new List<ProductResponse>();
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceUnavailableException("Product Service is unreachable.", ex);
            }
            catch (BrokenCircuitException ex)
            {
                throw new ServiceUnavailableException("Product Service is temporarily unavailable.", ex);
            }
            catch (TimeoutRejectedException ex)
            {
                throw new DownstreamTimeoutException("Product Service request timed out.", ex);
            }
        }
    }
}
