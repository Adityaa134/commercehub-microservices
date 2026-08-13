using ProductsService.Core.DTO;

namespace ProductsService.Core.ServiceContracts
{
    public interface ICartService
    {
        Task<CartResponse> GetCartByUserIdAsync(Guid userId);
        Task<CartItemResponse> AddItemToCartAsync(Guid userId, CartItemAddRequest request);
        Task<CartItemResponse> UpdateCartItemQuantityAsync(Guid userId, CartItemUpdateRequest request);
        Task<bool> RemoveItemFromCartAsync(Guid userId, Guid productId);
    }
}
