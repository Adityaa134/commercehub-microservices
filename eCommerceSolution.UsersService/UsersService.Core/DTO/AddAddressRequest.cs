using System.ComponentModel.DataAnnotations;
using UsersService.Core.Enums;

namespace UsersService.Core.DTO
{
    public class AddAddressRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(13,MinimumLength = 10, ErrorMessage = "Phone number must be 10 of digits.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pincode is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pin code must be exactly 6 characters.")]
        public string PinCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address Line 1 is required.")]
        [StringLength(200, ErrorMessage = "Address Line 1 cannot exceed 200 characters.")]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address Line 2 cannot exceed 200 characters.")]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters.")]
        public string State { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Landmark cannot exceed 100 characters.")]
        public string? Landmark { get; set; }

        [EnumDataType(typeof(AddressType), ErrorMessage = "Invalid address type.")]
        public AddressType Type { get; set; } = AddressType.Home;

        public bool IsDefault { get; set; } = false;
    }
}
