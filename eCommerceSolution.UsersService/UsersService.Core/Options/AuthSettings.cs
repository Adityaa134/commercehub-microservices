namespace UsersService.Core.Options
{
    public class AuthSettings
    {
        public string EmailConfirmationUrl { get; set; } = string.Empty;
        public string PasswordResetUrl { get; set; } = string.Empty;
    }
}
