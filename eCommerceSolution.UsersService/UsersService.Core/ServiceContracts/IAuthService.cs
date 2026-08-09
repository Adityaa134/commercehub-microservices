using UsersService.Core.DTO;

namespace UsersService.Core.ServiceContracts
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> LoginAsync(LoginRequest loginRequest);
        Task<RegisterResponse> CreateAccountAsync(RegisterRequest registerRequest);
        Task<AuthenticationResponse> RefreshTokenAsync(string rawRefreshToken);
        Task LogoutAsync(string refreshToken);
        Task ConfirmEmailAsync(ConfirmEmailRequest confirmEmailRequest);
        Task ResendEmailConfirmationLinkAsync(EmailRequest emailRequest);
        Task SendForgotPasswordEmailAsync(EmailRequest emailRequest);
        Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
    }
}
