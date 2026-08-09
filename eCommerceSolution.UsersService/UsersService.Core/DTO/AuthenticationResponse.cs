namespace UsersService.Core.DTO
{
    public class AuthenticationResponse
    {
        public UserProfileResponse User { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
