using System.ComponentModel.DataAnnotations;

namespace UsersService.Core.DTO
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 50 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email should be in a proper format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone Number should contains only digits")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
