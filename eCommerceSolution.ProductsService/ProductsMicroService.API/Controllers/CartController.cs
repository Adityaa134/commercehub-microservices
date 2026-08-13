using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Core.DTO;
using ProductsService.Core.ServiceContracts;

namespace ProductsMicroService.API.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart()
        {
            Guid userId = GetUserId();
            CartResponse cartResponse = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(cartResponse);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartItemResponse>> AddItemToCart(CartItemAddRequest cartItemAddRequest)
        {
            Guid userId = GetUserId();
            CartItemResponse cartItemResponse = await _cartService.AddItemToCartAsync(userId, cartItemAddRequest);
            return Ok(cartItemResponse);
        }

        [HttpPut("items")]
        public async Task<ActionResult<CartItemResponse>> UpdateCartItemQuantity(CartItemUpdateRequest cartItemUpdateRequest)
        {
            Guid userId = GetUserId();
            CartItemResponse cartItemResponse = await _cartService.UpdateCartItemQuantityAsync(userId, cartItemUpdateRequest);
            return Ok(cartItemResponse);
        }

        [HttpDelete("items/{productId:guid}")]
        public async Task<ActionResult> RemoveItemFromCart(Guid productId)
        {
            Guid userId = GetUserId();
            await _cartService.RemoveItemFromCartAsync(userId, productId);
            return NoContent();
        }
    }
}
