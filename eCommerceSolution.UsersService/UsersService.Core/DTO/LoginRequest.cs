using System.ComponentModel.DataAnnotations;

namespace UsersService.Core.DTO
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email should be in a proper format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
