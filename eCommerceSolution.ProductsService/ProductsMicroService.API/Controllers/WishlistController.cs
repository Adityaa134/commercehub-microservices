using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Core.DTO;
using ProductsService.Core.ServiceContracts;

namespace ProductsMicroService.API.Controllers
{
    [Authorize]
    public class WishlistController : BaseController
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<WishlistResponse>> GetWishlist()
        {
            Guid userId = GetUserId();
            WishlistResponse response = await _wishlistService.GetWishlistByUserIdAsync(userId);
            return Ok(response);
        }

        [HttpGet("ids")]
        public async Task<ActionResult<List<Guid>>> GetWishlistProductIds()
        {
            Guid userId = GetUserId();
            List<Guid> productIds = await _wishlistService.GetWishlistProductIdsAsync(userId);
            return Ok(productIds);
        }

        [HttpPost("toggle/{productId:guid}")]
        public async Task<ActionResult<WishlistToggleResponse>> ToggleWishlist(Guid productId)
        {
            Guid userId = GetUserId();
            WishlistToggleResponse response = await _wishlistService.ToggleWishlistAsync(userId, productId);
            return Ok(response);
        }

        [HttpDelete("{productId:guid}")]
        public async Task<ActionResult> RemoveFromWishlist(Guid productId)
        {
            Guid userId = GetUserId();
            await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            return NoContent();
        }
    }
}

