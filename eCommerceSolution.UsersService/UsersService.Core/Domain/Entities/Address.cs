using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UsersService.Core.Enums;

namespace UsersService.Core.Domain.Entities
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(13)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(6)]
        public string PinCode { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(200)]
        public string? AddressLine2 { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Landmark { get; set; }
        public AddressType Type { get; set; }
        public bool IsDefault { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
