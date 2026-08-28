using System.Net;
using System.Net.Http.Json;
using OrdersService.Core.DTO;
using OrdersService.Core.ServiceContracts;

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
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Address/{addressId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AddressResponse>();
        }
    }
}
