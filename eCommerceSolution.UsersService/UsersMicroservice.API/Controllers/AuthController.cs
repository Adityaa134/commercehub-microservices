using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Core.DTO;
using UsersService.Core.Exceptions;
using UsersService.Core.ServiceContracts;

namespace UsersMicroservice.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest registerRequest)
        {
            RegisterResponse response = await _authService.CreateAccountAsync(registerRequest);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserProfileResponse>> Login(LoginRequest loginRequest)
        {
            AuthenticationResponse response = await _authService.LoginAsync(loginRequest);

            Response.Cookies.Append("access_token", response.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpirationTime"]))
            });

            Response.Cookies.Append("refresh_token", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/Auth",
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(
                   Convert.ToDouble(_configuration["RefreshToken:ExpirationTime"]))
            });

            return Ok(response.User);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserProfileResponse>> RefreshToken()
        {
            string? refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedOperationException("Session has expired or is invalid. Please log in again.");

            AuthenticationResponse response = await _authService.RefreshTokenAsync(refreshToken);

            Response.Cookies.Append("access_token", response.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpirationTime"]))
            });

            Response.Cookies.Append("refresh_token", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/Auth",
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(
                   Convert.ToDouble(_configuration["RefreshToken:ExpirationTime"]))
            });

            return Ok(response.User);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            string? refreshToken = Request.Cookies["refresh_token"];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken);
            }

            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                IsEssential = true,
            });

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/api/Auth",
                IsEssential = true,
            });

            return NoContent();
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult<MessageResponse>> ConfirmEmail(ConfirmEmailRequest request)
        {
            await _authService.ConfirmEmailAsync(request);
            return Ok(new MessageResponse("Email confirmed successfully."));
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<ActionResult<MessageResponse>> ResendConfirmationEmail(EmailRequest request)
        {
            await _authService.ResendEmailConfirmationLinkAsync(request);
            return Ok(new MessageResponse("Verification email sent successfully. Please check your inbox."));
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<MessageResponse>> ForgotPassword(EmailRequest request)
        {
            await _authService.SendForgotPasswordEmailAsync(request);
            return Ok(new MessageResponse("If an account with that email address exists, a password reset link has been sent. Please check your inbox."));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<MessageResponse>> ResetPassword(ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request);
            return Ok(new MessageResponse("Password has been reset successfully. You can now log in with your new password."));
        }
    }
}
