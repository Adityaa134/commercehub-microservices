namespace UsersService.Core.ServiceContracts
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshTokenAsync(Guid userId);
        Task<Guid> GetUserIdFromRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}
