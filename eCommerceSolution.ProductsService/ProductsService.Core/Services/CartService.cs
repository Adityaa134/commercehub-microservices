using AutoMapper;
using ProductsService.Core.Constants;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Core.DTO;
using ProductsService.Core.Exceptions;
using ProductsService.Core.ServiceContracts;

namespace ProductsService.Core.Services
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IImageService _imageService;

        public CartService(
            IMapper mapper,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IImageService imageService)
        {
            _mapper = mapper;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _imageService = imageService;
        }

        public async Task<CartResponse> GetCartByUserIdAsync(Guid userId)
        {
            CartResponse? cartResponse = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cartResponse == null)
            {
                return new CartResponse
                {
                    UserId = userId,
                    CartItems = new List<CartItemResponse>(),
                    CartTotal = 0,
                    TotalItems = 0
                };
            }

            if (cartResponse.CartItems != null)
            {
                foreach (CartItemResponse item in cartResponse.CartItems)
                {
                    if (!string.IsNullOrEmpty(item.ProductImagePath))
                    {
                        item.ProductImagePath = _imageService.GetImageUrl(
                            BlobContainers.ProductImages,
                            item.ProductImagePath);
                    }
                }
            }

            return cartResponse;
        }

        public async Task<CartItemResponse> AddItemToCartAsync(Guid userId, CartItemAddRequest request)
        {
            Product? product = await _productRepository.GetProductByIdAsync(request.ProductId);
            if (product == null || !product.IsActive)
                throw new NotFoundException("Product not found or unavailable.");

            Cart cart = await _cartRepository.GetOrCreateCartAsync(userId);
            CartItem? existingItem = await _cartRepository.GetCartItemAsync(userId, request.ProductId);

            int newQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;
            if (newQuantity > product.QuantityInStock)
                throw new BusinessValidationException($"Only {product.QuantityInStock} items available in stock.");

            CartItem cartItem;
            if (existingItem != null)
            {
                cartItem = await _cartRepository.UpdateCartItemQuantityAsync(existingItem, newQuantity);
            }
            else
            {
                cartItem = _mapper.Map<CartItemAddRequest, CartItem>(request);
                cartItem.CartId = cart.Id;
                cartItem = await _cartRepository.AddCartItemAsync(cartItem);
            }

            cartItem.Product = product;

            CartItemResponse response = _mapper.Map<CartItem, CartItemResponse>(cartItem);
            if (!string.IsNullOrEmpty(product.ProductImagePath))
            {
                response.ProductImagePath = _imageService.GetImageUrl(
                    BlobContainers.ProductImages,
                    product.ProductImagePath);
            }

            return response;
        }

        public async Task<CartItemResponse> UpdateCartItemQuantityAsync(Guid userId, CartItemUpdateRequest request)
        {
            CartItem? cartItem = await _cartRepository.GetCartItemAsync(userId, request.ProductId);
            if (cartItem == null)
                throw new NotFoundException("Item not found in cart.");

            if (request.Quantity > cartItem.Product.QuantityInStock)
                throw new BusinessValidationException($"Only {cartItem.Product.QuantityInStock} items available in stock.");

            CartItem updatedCartItem = await _cartRepository.UpdateCartItemQuantityAsync(cartItem, request.Quantity);

            CartItemResponse response = _mapper.Map<CartItem, CartItemResponse>(updatedCartItem);
            if (!string.IsNullOrEmpty(updatedCartItem.Product?.ProductImagePath))
            {
                response.ProductImagePath = _imageService.GetImageUrl(
                    BlobContainers.ProductImages,
                    updatedCartItem.Product.ProductImagePath);
            }

            return response;
        }

        public async Task<bool> RemoveItemFromCartAsync(Guid userId, Guid productId)
        {
            bool isRemoved = await _cartRepository.RemoveCartItemAsync(userId, productId);
            if (!isRemoved)
                throw new NotFoundException("Item not found in cart.");

            return true;
        }

        public async Task<CartResponse> MergeCartAsync(Guid userId, List<CartItemAddRequest> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                return await GetCartByUserIdAsync(userId);
            }

            Cart cart = await _cartRepository.GetOrCreateCartAsync(userId);

            foreach (CartItemAddRequest cartItem in cartItems)
            {
                Product? product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
                if (product == null || !product.IsActive) continue;

                int maxAllowed = Math.Min(product.QuantityInStock, 5);
                CartItem? existingItem = await _cartRepository.GetCartItemAsync(userId, cartItem.ProductId);

                if (existingItem != null)
                {
                    int combinedQty = Math.Min(existingItem.Quantity + cartItem.Quantity, maxAllowed);
                    await _cartRepository.UpdateCartItemQuantityAsync(existingItem, combinedQty);
                }
                else
                {
                    CartItem newItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = product.Id,
                        Quantity = Math.Min(cartItem.Quantity, maxAllowed)
                    };
                    await _cartRepository.AddCartItemAsync(newItem);
                }
            }

            return await GetCartByUserIdAsync(userId);
        }
    }
}
