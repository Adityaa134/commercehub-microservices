namespace UsersService.Core.DTO
{
    public class UserProfileResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
