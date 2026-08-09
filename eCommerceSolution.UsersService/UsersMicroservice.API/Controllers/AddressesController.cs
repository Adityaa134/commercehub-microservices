using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Core.DTO;
using UsersService.Core.ServiceContracts;

namespace UsersMicroservice.API.Controllers
{
    [Authorize]
    public class AddressesController : BaseController
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost]
        public async Task<ActionResult<AddressResponse>> AddAddress(AddAddressRequest request)
        {
            Guid userId = GetUserId();
            AddressResponse response = await _addressService.AddAddressAsync(userId, request);
            return CreatedAtAction(nameof(GetAddressById), new { addressId = response.Id }, response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressResponse>>> GetUserAddresses()
        {
            Guid userId = GetUserId();
            List<AddressResponse> addresses = await _addressService.GetUserAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpGet("{addressId:guid}")]
        public async Task<ActionResult<AddressResponse>> GetAddressById(Guid addressId)
        {
            Guid userId = GetUserId();
            AddressResponse address = await _addressService.GetAddressByIdAsync(userId, addressId);
            return Ok(address);
        }

        [HttpPut("{addressId:guid}")]
        public async Task<ActionResult<AddressResponse>> UpdateAddress(Guid addressId,UpdateAddressRequest request)
        {
            Guid userId = GetUserId();
            AddressResponse response = await _addressService.UpdateAddressAsync(userId, addressId, request);
            return Ok(response);
        }

        [HttpDelete("{addressId:guid}")]
        public async Task<ActionResult> DeleteAddress(Guid addressId)
        {
            Guid userId = GetUserId();
            await _addressService.DeleteAddressAsync(userId, addressId);
            return NoContent();
        }

        [HttpPatch("{addressId:guid}/set-default")]
        public async Task<ActionResult<AddressResponse>> SetDefaultAddress(Guid addressId)
        {
            Guid userId = GetUserId();
            AddressResponse response = await _addressService.SetDefaultAddressAsync(userId, addressId);
            return Ok(response);
        }
    }
}
