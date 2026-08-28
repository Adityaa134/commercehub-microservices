using OrdersService.Core.DTO;

namespace OrdersService.Core.ServiceContracts
{
    public interface IUserServiceClient
    {
        Task<AddressResponse?> GetAddressByIdAsync(Guid addressId);
    }
}
