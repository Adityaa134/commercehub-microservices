using ProductsService.Core.Domain.Entities;

namespace ProductsService.Core.Domain.RepositoryContracts
{
    public interface IWishlistRepository
    {
        Task<List<WishlistItem>> GetWishlistItemsByUserIdAsync(Guid userId);
        Task<List<Guid>> GetWishlistProductIdsAsync(Guid userId);
        Task<WishlistItem?> GetWishlistItemAsync(Guid userId, Guid productId);
        Task<WishlistItem> AddWishlistItemAsync(WishlistItem wishlistItem);
        Task<bool> RemoveWishlistItemAsync(Guid userId, Guid productId);
    }
}

