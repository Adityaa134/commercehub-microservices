using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using UsersService.Core.Domain.Entities;
using UsersService.Core.Domain.RepositoryContracts;
using UsersService.Core.Exceptions;
using UsersService.Core.ServiceContracts;

namespace UsersService.Core.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IConfiguration _configuration;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<string> CreateRefreshTokenAsync(Guid userId)
        {
            string refreshToken = GenerateRefreshToken();
            string hashedToken = HashToken(refreshToken);

            RefreshToken refreshToken1 = new RefreshToken()
            {
                UserId = userId,
                Token = hashedToken,
                ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["RefreshToken:ExpirationTime"]))
            };

            await _refreshTokenRepository.AddRefreshTokenAsync(refreshToken1);

            return refreshToken;
        }

        public async Task<Guid> GetUserIdFromRefreshTokenAsync(string refreshToken)
        {
            string hashedToken = HashToken(refreshToken);
            RefreshToken? tokenEntity = await _refreshTokenRepository.GetRefreshTokenAsync(hashedToken);

            if (tokenEntity == null)
                throw new UnauthorizedOperationException("Your session is invalid. Please log in again.");

            if (tokenEntity.IsRevoked)
                throw new UnauthorizedOperationException("Your session has ended. Please log in again.");

            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedOperationException("Your session has expired. Please log in again.");

            return tokenEntity.UserId;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            string hashedToken = HashToken(refreshToken);
            await _refreshTokenRepository.RevokeTokenAsync(hashedToken);
        }

        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return WebEncoders.Base64UrlEncode(bytes);
        }

        private string HashToken(string refreshToken)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(bytes);
        }
    }
}
