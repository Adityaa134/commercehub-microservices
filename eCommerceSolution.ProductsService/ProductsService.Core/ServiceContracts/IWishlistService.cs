using ProductsService.Core.DTO;

namespace ProductsService.Core.ServiceContracts
{
    public interface IWishlistService
    {
        Task<WishlistResponse> GetWishlistByUserIdAsync(Guid userId);
        Task<List<Guid>> GetWishlistProductIdsAsync(Guid userId);
        Task<WishlistToggleResponse> ToggleWishlistAsync(Guid userId, Guid productId);
        Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId);
    }
}

