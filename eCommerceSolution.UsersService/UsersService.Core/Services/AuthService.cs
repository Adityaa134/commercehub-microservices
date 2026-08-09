using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using UsersService.Core.Domain.Entities;
using UsersService.Core.DTO;
using UsersService.Core.Exceptions;
using UsersService.Core.Options;
using UsersService.Core.ServiceContracts;

namespace UsersService.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly AuthSettings _authSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IRefreshTokenService refreshTokenService,
            IMapper mapper,
            IEmailService emailService,
            IOptions<AuthSettings> authSettingsOptions,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _mapper = mapper;
            _emailService = emailService;
            _authSettings = authSettingsOptions.Value;
            _logger = logger;
        }

        public async Task<RegisterResponse> CreateAccountAsync(RegisterRequest registerRequest)
        {
            ApplicationUser? existingUser = await _userManager.FindByEmailAsync(registerRequest.Email!);
            if (existingUser != null)
                throw new ConflictException("An account with this email already exists");

            ApplicationUser newUser = _mapper.Map<RegisterRequest, ApplicationUser>(registerRequest);

            IdentityResult result = await _userManager.CreateAsync(newUser, registerRequest.Password!);
            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessValidationException(errors);
            }

            IdentityResult roleResult = await _userManager.AddToRoleAsync(newUser, Constants.Roles.User);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                throw new InvalidOperationException("Unable to create account. Please try again");
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            if (string.IsNullOrWhiteSpace(_authSettings.EmailConfirmationUrl))
            {
                throw new InvalidOperationException("EmailConfirmationUrl is not configured.");
            }

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string verificationLink = $"{_authSettings.EmailConfirmationUrl}?uid={newUser.Id}&token={encodedToken}";
           
            await _emailService.SendEmailVerificationLinkAsync(newUser.Email!, newUser.UserName ?? newUser.Email!, verificationLink);

            return new RegisterResponse()
            {
                Email = newUser.Email!,
                Message = "Registration successful. Please check your email to verify your account."
            };
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginRequest loginRequest)
        {
            ApplicationUser? user =  await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                throw new UnauthorizedOperationException("Invalid email or password");

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password,lockoutOnFailure:false);
            if (result.IsNotAllowed)
            {
                throw new ForbiddenOperationException("Please confirm your email before logging in.");
            }
            if (result.Succeeded)
            {
                IList<string> roles =  await _userManager.GetRolesAsync(user);
                string accessToken = _jwtService.CreateJwtToken(user,roles);
                string refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);
                UserProfileResponse userProfile =  _mapper.Map<ApplicationUser, UserProfileResponse>(user);
                userProfile.Role = roles.FirstOrDefault() ?? string.Empty;

                return new AuthenticationResponse()
                {
                    User = userProfile,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
            throw new UnauthorizedOperationException("Invalid email or password");
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string refreshToken)
        {
            Guid userId = await _refreshTokenService.GetUserIdFromRefreshTokenAsync(refreshToken);

            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new UnauthorizedOperationException("Your session is no longer valid. Please log in again.");

            IList<string> roles = await _userManager.GetRolesAsync(user);
            string newAccessToken = _jwtService.CreateJwtToken(user, roles);
            string newRefreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id);

            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

            UserProfileResponse userProfile = _mapper.Map<ApplicationUser, UserProfileResponse>(user);
            userProfile.Role = roles.FirstOrDefault() ?? string.Empty;

            return new AuthenticationResponse()
            {
                User = userProfile,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await _refreshTokenService.GetUserIdFromRefreshTokenAsync(refreshToken);
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task ConfirmEmailAsync(ConfirmEmailRequest confirmEmailRequest)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(confirmEmailRequest.UserId.ToString());
            if (user == null)
                throw new NotFoundException("Invalid or expired confirmation link.");

            if (user.EmailConfirmed)
                return;

            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailRequest.Token));

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                throw new BusinessValidationException("The confirmation link is invalid or has expired.");
            }
        }

        public async Task ResendEmailConfirmationLinkAsync(EmailRequest emailRequest)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(emailRequest.Email);
            if (user == null || user.EmailConfirmed)
                return;

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (string.IsNullOrWhiteSpace(_authSettings.EmailConfirmationUrl))
            {
                throw new InvalidOperationException("EmailConfirmationUrl is not configured.");
            }

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string verificationLink = $"{_authSettings.EmailConfirmationUrl}?uid={user.Id}&token={encodedToken}";

            await _emailService.SendEmailVerificationLinkAsync(user.Email!, user.UserName ?? user.Email!, verificationLink);
        }

        public async Task SendForgotPasswordEmailAsync(EmailRequest emailRequest)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(emailRequest.Email);
            if (user == null || !user.EmailConfirmed)
                return;

            string rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrWhiteSpace(_authSettings.PasswordResetUrl))
            {
                throw new InvalidOperationException("PasswordResetUrl is not configured.");
            }

            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
            string resetLink = $"{_authSettings.PasswordResetUrl}?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";

            await _emailService.SendPasswordResetLinkAsync(user.Email!, user.UserName ?? user.Email!, resetLink);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
                throw new NotFoundException("Invalid password reset request or link has expired.");

            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordRequest.Token));

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessValidationException(errors);
            }
        }
    }
}
