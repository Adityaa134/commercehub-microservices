using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Core.DTO;
using ProductsService.Infrastructure.DBContext;

namespace ProductsService.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CartRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Cart> GetOrCreateCartAsync(Guid userId)
        {
            Cart? cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };
                await _dbContext.Carts.AddAsync(cart);
                await _dbContext.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<CartResponse?> GetCartByUserIdAsync(Guid userId)
        {
            return await _dbContext.Carts
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(c => new CartResponse
                {
                    CartId = c.Id,
                    UserId = userId,
                    CartTotal = c.CartItems.Sum(ci => ci.Quantity * ci.Product.UnitPrice),
                    TotalItems = c.CartItems.Sum(ci => ci.Quantity),
                    CartItems = c.CartItems.Select(ci => new CartItemResponse
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        ProductName = ci.Product.Name,
                        ProductImagePath = ci.Product.ProductImagePath,
                        UnitPrice = ci.Product.UnitPrice,
                        TotalPrice = ci.Quantity * ci.Product.UnitPrice
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(Guid userId, Guid productId)
        {
            return await _dbContext.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);
        }

        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            await _dbContext.CartItems.AddAsync(cartItem);
            await _dbContext.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem> UpdateCartItemQuantityAsync(CartItem cartItem, int quantity)
        {
            cartItem.Quantity = quantity;
            await _dbContext.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> RemoveCartItemAsync(Guid userId, Guid productId)
        {
            CartItem? cartItem = await _dbContext.CartItems
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);

            if (cartItem == null)
                return false;

            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
