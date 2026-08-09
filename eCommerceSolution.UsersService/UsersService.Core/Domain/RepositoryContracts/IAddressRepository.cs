using UsersService.Core.Domain.Entities;

namespace UsersService.Core.Domain.RepositoryContracts
{
    public interface IAddressRepository
    {
        Task<Address> AddAddressAsync(Address address);
        Task<List<Address>> GetAddressesByUserIdAsync(Guid userId);
        Task<Address?> GetAddressByIdAsync(Guid addressId);
        Task<Address?> GetAddressByIdAndUserIdAsync(Guid addressId, Guid userId);
        Task<Address> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(Guid addressId, Guid userId);
        Task ResetDefaultAddressForUserAsync(Guid userId);
    }
}
