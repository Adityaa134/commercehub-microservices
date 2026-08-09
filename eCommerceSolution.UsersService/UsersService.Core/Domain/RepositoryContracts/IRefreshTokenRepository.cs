using UsersService.Core.Domain.Entities;

namespace UsersService.Core.Domain.RepositoryContracts
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken); 
    }
}
