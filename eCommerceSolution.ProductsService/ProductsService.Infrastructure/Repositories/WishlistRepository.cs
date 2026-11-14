using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Infrastructure.DBContext;

namespace ProductsService.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WishlistRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<WishlistItem>> GetWishlistItemsByUserIdAsync(Guid userId)
        {
            return await _dbContext.WishlistItems
                .AsNoTracking()
                .Include(w => w.Product)
                    .ThenInclude(p => p.Category)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetWishlistProductIdsAsync(Guid userId)
        {
            return await _dbContext.WishlistItems
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .Select(w => w.ProductId)
                .ToListAsync();
        }

        public async Task<WishlistItem?> GetWishlistItemAsync(Guid userId, Guid productId)
        {
            return await _dbContext.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<WishlistItem> AddWishlistItemAsync(WishlistItem wishlistItem)
        {
            await _dbContext.WishlistItems.AddAsync(wishlistItem);
            await _dbContext.SaveChangesAsync();
            return wishlistItem;
        }

        public async Task<bool> RemoveWishlistItemAsync(Guid userId, Guid productId)
        {
            WishlistItem? item = await _dbContext.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (item == null)
                return false;

            _dbContext.WishlistItems.Remove(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

