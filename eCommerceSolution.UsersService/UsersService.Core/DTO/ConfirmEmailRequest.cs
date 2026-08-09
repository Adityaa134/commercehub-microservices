using System.ComponentModel.DataAnnotations;

namespace UsersService.Core.DTO
{
    public class ConfirmEmailRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; } = string.Empty;
    }
}
