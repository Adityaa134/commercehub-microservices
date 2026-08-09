using Microsoft.EntityFrameworkCore;
using UsersService.Core.Domain.Entities;
using UsersService.Core.Domain.RepositoryContracts;
using UsersService.Infrastructure.DBContext;

namespace UsersService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RefreshTokenRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string hashedToken)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == hashedToken);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            RefreshToken? token = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (token == null)
                return;

            token.IsRevoked = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
