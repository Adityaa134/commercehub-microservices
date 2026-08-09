using AutoMapper;
using UsersService.Core.Domain.Entities;
using UsersService.Core.Domain.RepositoryContracts;
using UsersService.Core.DTO;
using UsersService.Core.Exceptions;
using UsersService.Core.ServiceContracts;

namespace UsersService.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<AddressResponse> AddAddressAsync(Guid userId, AddAddressRequest request)
        {
            Address address = _mapper.Map<Address>(request);
            address.UserId = userId;

            Address addedAddress = await _addressRepository.AddAddressAsync(address);
            return _mapper.Map<AddressResponse>(addedAddress);
        }

        public async Task<List<AddressResponse>> GetUserAddressesAsync(Guid userId)
        {
            List<Address> addresses = await _addressRepository.GetAddressesByUserIdAsync(userId);
            return _mapper.Map<List<AddressResponse>>(addresses);
        }

        public async Task<AddressResponse> GetAddressByIdAsync(Guid userId, Guid addressId)
        {
            Address? address = await _addressRepository.GetAddressByIdAndUserIdAsync(addressId, userId);
            if (address == null)
            {
                throw new NotFoundException($"Address was not found.");
            }

            return _mapper.Map<AddressResponse>(address);
        }

        public async Task<AddressResponse> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressRequest request)
        {
            Address? existingAddress = await _addressRepository.GetAddressByIdAndUserIdAsync(addressId, userId);
            if (existingAddress == null)
            {
                throw new NotFoundException($"Address was not found.");
            }

            _mapper.Map(request, existingAddress);

            Address updatedAddress = await _addressRepository.UpdateAddressAsync(existingAddress);
            return _mapper.Map<AddressResponse>(updatedAddress);
        }

        public async Task<bool> DeleteAddressAsync(Guid userId, Guid addressId)
        {
            bool result = await _addressRepository.DeleteAddressAsync(addressId, userId);
            if (!result)
            {
                throw new NotFoundException($"Address was not found.");
            }

            return true;
        }

        public async Task<AddressResponse> SetDefaultAddressAsync(Guid userId, Guid addressId)
        {
            Address? address = await _addressRepository.GetAddressByIdAndUserIdAsync(addressId, userId);
            if (address == null)
            {
                throw new NotFoundException($"Address was not found.");
            }

            await _addressRepository.ResetDefaultAddressForUserAsync(userId);

            address.IsDefault = true;
            Address updatedAddress = await _addressRepository.UpdateAddressAsync(address);
            return _mapper.Map<AddressResponse>(updatedAddress);
        }
    }
}
