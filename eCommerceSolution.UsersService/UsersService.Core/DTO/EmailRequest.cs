using System.ComponentModel.DataAnnotations;

namespace UsersService.Core.DTO
{
    public class EmailRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;
    }
}
