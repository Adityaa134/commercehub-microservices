using ProductsService.Core.Domain.Entities;
using ProductsService.Core.DTO;

namespace ProductsService.Core.Domain.RepositoryContracts
{
    public interface ICartRepository
    {
        Task<Cart> GetOrCreateCartAsync(Guid userId);
        Task<CartResponse?> GetCartByUserIdAsync(Guid userId);
        Task<CartItem?> GetCartItemAsync(Guid userId, Guid productId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemQuantityAsync(CartItem cartItem, int quantity);
        Task<bool> RemoveCartItemAsync(Guid userId, Guid productId);
    }
}
