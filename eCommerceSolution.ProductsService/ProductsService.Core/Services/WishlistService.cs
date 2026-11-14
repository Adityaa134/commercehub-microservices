using AutoMapper;
using ProductsService.Core.Constants;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Core.DTO;
using ProductsService.Core.Exceptions;
using ProductsService.Core.ServiceContracts;

namespace ProductsService.Core.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IMapper _mapper;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IProductRepository _productRepository;
        private readonly IImageService _imageService;

        public WishlistService(
            IMapper mapper,
            IWishlistRepository wishlistRepository,
            IProductRepository productRepository,
            IImageService imageService)
        {
            _mapper = mapper;
            _wishlistRepository = wishlistRepository;
            _productRepository = productRepository;
            _imageService = imageService;
        }

        public async Task<WishlistResponse> GetWishlistByUserIdAsync(Guid userId)
        {
            List<WishlistItem> items = await _wishlistRepository.GetWishlistItemsByUserIdAsync(userId);

            List<WishlistItemResponse> itemResponses = _mapper.Map<List<WishlistItemResponse>>(items);

            foreach (WishlistItemResponse item in itemResponses)
            {
                if (!string.IsNullOrEmpty(item.ProductImagePath))
                {
                    item.ProductImagePath = _imageService.GetImageUrl(
                        BlobContainers.ProductImages,
                        item.ProductImagePath);
                }
            }

            return new WishlistResponse
            {
                UserId = userId,
                Items = itemResponses
            };
        }

        public async Task<List<Guid>> GetWishlistProductIdsAsync(Guid userId)
        {
            return await _wishlistRepository.GetWishlistProductIdsAsync(userId);
        }

        public async Task<WishlistToggleResponse> ToggleWishlistAsync(Guid userId, Guid productId)
        {
            Product? product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null || !product.IsActive)
                throw new NotFoundException("Product not found or unavailable.");

            WishlistItem? existingItem = await _wishlistRepository.GetWishlistItemAsync(userId, productId);

            if (existingItem != null)
            {
                await _wishlistRepository.RemoveWishlistItemAsync(userId, productId);
                return new WishlistToggleResponse
                {
                    ProductId = productId,
                    IsInWishlist = false
                };
            }

            WishlistItem newItem = new WishlistItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                AddedAt = DateTime.UtcNow
            };

            await _wishlistRepository.AddWishlistItemAsync(newItem);

            return new WishlistToggleResponse
            {
                ProductId = productId,
                IsInWishlist = true
            };
        }

        public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId)
        {
            bool isRemoved = await _wishlistRepository.RemoveWishlistItemAsync(userId, productId);
            if (!isRemoved)
                throw new NotFoundException("Item not found in wishlist.");

            return true;
        }
    }
}

