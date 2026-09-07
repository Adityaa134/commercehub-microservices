using System.Net;
using System.Net.Http.Json;
using OrdersService.Core.DTO;
using OrdersService.Core.Exceptions;
using OrdersService.Core.ServiceContracts;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace OrdersService.Infrastructure.HttpClients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AddressResponse?> GetAddressByIdAsync(Guid addressId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Address/{addressId}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<AddressResponse>();
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceUnavailableException("User Service is unreachable.", ex);
            }
            catch (BrokenCircuitException ex)
            {
                throw new ServiceUnavailableException("User Service is temporarily unavailable.", ex);
            }
            catch (TimeoutRejectedException ex)
            {
                throw new DownstreamTimeoutException("User Service request timed out.", ex);
            }
        }
    }
}
