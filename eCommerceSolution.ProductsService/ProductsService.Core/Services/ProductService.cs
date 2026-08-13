using AutoMapper;
using ProductsService.Core.Constants;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Core.DTO;
using ProductsService.Core.Exceptions;
using ProductsService.Core.ServiceContracts;

namespace ProductsService.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageService _imageService;

        public ProductService(
            IMapper mapper,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IImageService imageService)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageService = imageService;
        }

        public async Task<ProductResponse> AddProductAsync(ProductAddRequest productAddRequest)
        {
            Category? category = await _categoryRepository.GetCategoryByIdAdminAsync(productAddRequest.CategoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            bool isNameDuplicate = await _productRepository.IsProductNameExistAsync(productAddRequest.ProductName);
            if (isNameDuplicate)
                throw new ConflictException("A product with this name already exists.");

            Product product = _mapper.Map<ProductAddRequest, Product>(productAddRequest);

            string originalName = Path.GetFileNameWithoutExtension(productAddRequest.ProductImage.FileName).Trim().Replace(" ", "-");
            string extension = Path.GetExtension(productAddRequest.ProductImage.FileName);
            string fileName = $"{originalName}-{Guid.NewGuid()}{extension}";

            using (Stream stream = productAddRequest.ProductImage.OpenReadStream())
            {
                string blobName = await _imageService.UploadImageAsync(stream, fileName, BlobContainers.ProductImages);
                product.ProductImagePath = blobName;
            }

            await _productRepository.AddProductAsync(product);

            ProductResponse response = _mapper.Map<Product, ProductResponse>(product);
            response.ProductImagePath = _imageService.GetImageUrl(BlobContainers.ProductImages, response.ProductImagePath);
            return response;
        }

        public async Task<ProductResponse> GetProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new BusinessValidationException("Product Id cannot be empty.");

            Product? product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            ProductResponse response = _mapper.Map<Product, ProductResponse>(product);
            response.ProductImagePath = _imageService.GetImageUrl(BlobContainers.ProductImages, response.ProductImagePath);
            return response;
        }

        public async Task<ProductResponse> GetProductByIdAdminAsync(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new BusinessValidationException("Product Id cannot be empty.");

            Product? product = await _productRepository.GetProductByIdAdminAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            ProductResponse response = _mapper.Map<Product, ProductResponse>(product);
            response.ProductImagePath = _imageService.GetImageUrl(BlobContainers.ProductImages, response.ProductImagePath);
            return response;
        }

        public async Task<ProductResponse> UpdateProductAsync(Guid productId, ProductUpdateRequest request)
        {
            if (productId == Guid.Empty)
                throw new BusinessValidationException("Product Id cannot be empty.");

            Product? existingProduct = await _productRepository.GetProductByIdAdminAsync(productId);
            if (existingProduct == null)
                throw new NotFoundException("Product not found.");

            Category? category = await _categoryRepository.GetCategoryByIdAdminAsync(request.CategoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            if (!category.IsActive && existingProduct.IsActive)
                throw new BusinessValidationException("Cannot assign product to an inactive category while product is active.");

            bool isNameDuplicate = await _productRepository.IsProductNameExistAsync(request.ProductName, productId);
            if (isNameDuplicate)
                throw new ConflictException("A product with this name already exists.");

            _mapper.Map(request, existingProduct);
            existingProduct.Category = category;

            if (request.ProductImage != null && request.ProductImage.Length > 0)
            {
                string originalName = Path.GetFileNameWithoutExtension(request.ProductImage.FileName).Trim().Replace(" ", "-");
                string extension = Path.GetExtension(request.ProductImage.FileName);
                string newFileName = $"{originalName}-{Guid.NewGuid()}{extension}";

                using Stream stream = request.ProductImage.OpenReadStream();
                string newBlobName = await _imageService.ReplaceImageAsync(
                    stream,
                    newFileName,
                    existingProduct.ProductImagePath,
                    BlobContainers.ProductImages);

                existingProduct.ProductImagePath = newBlobName;
            }

            Product updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
            ProductResponse response = _mapper.Map<Product, ProductResponse>(updatedProduct);
            response.ProductImagePath = _imageService.GetImageUrl(BlobContainers.ProductImages, response.ProductImagePath);
            return response;
        }

        public async Task<ProductResponse> UpdateProductStatusAsync(Guid productId, bool isActive)
        {
            if (productId == Guid.Empty)
                throw new BusinessValidationException("Product Id cannot be empty.");

            Product? product = await _productRepository.GetProductByIdAdminAsync(productId);
            if (product == null)
                throw new NotFoundException("Product not found.");

            if (isActive && product.Category != null && !product.Category.IsActive)
                throw new BusinessValidationException("Cannot activate a product whose parent category is inactive.");

            product.IsActive = isActive;
            await _productRepository.UpdateProductStatusAsync(productId, isActive);

            ProductResponse response = _mapper.Map<Product, ProductResponse>(product);
            response.ProductImagePath = _imageService.GetImageUrl(BlobContainers.ProductImages, response.ProductImagePath);
            return response;
        }

        public async Task<bool> IsProductExistAsync(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new BusinessValidationException("Product Id cannot be empty.");

            return await _productRepository.IsProductExistAsync(productId);
        }
    }
}
