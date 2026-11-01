namespace UsersService.Core.DTO
{
    public class UserProfileResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
