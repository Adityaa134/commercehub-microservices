using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UsersService.Core.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required,StringLength(50)]
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
