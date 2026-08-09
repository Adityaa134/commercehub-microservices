using UsersService.Core.DTO;

namespace UsersService.Core.ServiceContracts
{
    public interface IAddressService
    {
        Task<AddressResponse> AddAddressAsync(Guid userId, AddAddressRequest request);
        Task<List<AddressResponse>> GetUserAddressesAsync(Guid userId);
        Task<AddressResponse> GetAddressByIdAsync(Guid userId, Guid addressId);
        Task<AddressResponse> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request);
        Task<bool> DeleteAddressAsync(Guid userId, Guid addressId);
        Task<AddressResponse> SetDefaultAddressAsync(Guid userId, Guid addressId);
    }
}
