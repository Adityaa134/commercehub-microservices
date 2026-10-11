using AutoMapper;
using ProductsService.Core.Constants;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.Domain.RepositoryContracts;
using ProductsService.Core.DTO;
using ProductsService.Core.Exceptions;
using ProductsService.Core.ServiceContracts;

namespace ProductsService.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageService _imageService;

        public CategoryService(IMapper mapper, ICategoryRepository categoryRepository, IImageService imageService)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _imageService = imageService;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CategoryAddRequest categoryAddRequest)
        {
            bool isNameDuplicate = await _categoryRepository.IsCategoryNameExistAsync(categoryAddRequest.CategoryName);
            if (isNameDuplicate)
                throw new ConflictException("A category with this name already exists.");

            if (categoryAddRequest.ParentCategoryId != null)
            {
                bool parentExists = await _categoryRepository.IsCategoryExistAsync(categoryAddRequest.ParentCategoryId.Value);
                if (!parentExists)
                    throw new NotFoundException("Parent category does not exist.");
            }

            Category category = _mapper.Map<CategoryAddRequest, Category>(categoryAddRequest);

            if (categoryAddRequest.CategoryImage != null && categoryAddRequest.CategoryImage.Length > 0)
            {
                string originalName = Path.GetFileNameWithoutExtension(categoryAddRequest.CategoryImage.FileName).Trim().Replace(" ", "-");
                string extension = Path.GetExtension(categoryAddRequest.CategoryImage.FileName);
                string fileName = $"{originalName}-{Guid.NewGuid()}{extension}";

                using Stream stream = categoryAddRequest.CategoryImage.OpenReadStream();
                string blobName = await _imageService.UploadImageAsync(stream, fileName, BlobContainers.CategoryImages);
                category.CategoryImagePath = blobName;
            }

            await _categoryRepository.AddCategoryAsync(category);

            CategoryResponse categoryResponse = _mapper.Map<Category, CategoryResponse>(category);
            if (!string.IsNullOrEmpty(categoryResponse.CategoryImagePath))
            {
                categoryResponse.CategoryImagePath = _imageService.GetImageUrl(BlobContainers.CategoryImages, categoryResponse.CategoryImagePath);
            }
            return categoryResponse;
        }

        public async Task<List<CategoryResponse>> GetCategoriesAsync()
        {
            List<CategoryResponse> categoryResponses = await _categoryRepository.GetCategoriesAsync();
            foreach (CategoryResponse categoryResponse in categoryResponses)
            {
                if (!string.IsNullOrEmpty(categoryResponse.CategoryImagePath))
                {
                    categoryResponse.CategoryImagePath = _imageService.GetImageUrl(BlobContainers.CategoryImages, categoryResponse.CategoryImagePath);
                }
            }
            return categoryResponses;
        }

        public async Task<List<CategoryResponse>> GetCategoriesAdminAsync()
        {
            List<CategoryResponse> categoryResponses = await _categoryRepository.GetCategoriesAdminAsync();
            foreach (CategoryResponse categoryResponse in categoryResponses)
            {
                if (!string.IsNullOrEmpty(categoryResponse.CategoryImagePath))
                {
                    categoryResponse.CategoryImagePath = _imageService.GetImageUrl(BlobContainers.CategoryImages, categoryResponse.CategoryImagePath);
                }
            }
            return categoryResponses;
        }

        public async Task<CategoryResponse> GetCategoryByIdAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new BusinessValidationException("Category Id cannot be empty.");

            Category? category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            CategoryResponse categoryResponse = _mapper.Map<Category, CategoryResponse>(category);
            if (!string.IsNullOrEmpty(categoryResponse.CategoryImagePath))
            {
                categoryResponse.CategoryImagePath = _imageService.GetImageUrl(BlobContainers.CategoryImages, categoryResponse.CategoryImagePath);
            }
            return categoryResponse;
        }

        public async Task<CategoryResponse> GetCategoryByIdAdminAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new BusinessValidationException("Category Id cannot be empty.");

            Category? category = await _categoryRepository.GetCategoryByIdAdminAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            CategoryResponse categoryResponse = _mapper.Map<Category, CategoryResponse>(category);
            if (!string.IsNullOrEmpty(categoryResponse.CategoryImagePath))
            {
                categoryResponse.CategoryImagePath = _imageService.GetImageUrl(BlobContainers.CategoryImages, categoryResponse.CategoryImagePath);
            }
            return categoryResponse;
        }

        public async Task<bool> IsCategoryExistAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new BusinessValidationException("Category Id cannot be empty.");

            return await _categoryRepository.IsCategoryExistAsync(categoryId);
        }

        public async Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, CategoryUpdateRequest request)
        {
            if (categoryId == Guid.Empty)
                throw new BusinessValidationException("Category Id cannot be empty.");

            Category? category = await _categoryRepository.GetCategoryByIdAdminAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            bool isNameDuplicate = await _categoryRepository.IsCategoryNameExistAsync(request.CategoryName, categoryId);
            if (isNameDuplicate)
                throw new ConflictException("A category with this name already exists.");

            if (request.ParentCategoryId.HasValue)
            {
                if (request.ParentCategoryId.Value == categoryId)
                    throw new BusinessValidationException("A category cannot be its own parent.");

                bool parentExists = await _categoryRepository.IsCategoryExistAsync(request.ParentCategoryId.Value);
                if (!parentExists)
                    throw new NotFoundException("Parent category does not exist.");
            }

            if (!request.ParentCategoryId.HasValue && string.IsNullOrEmpty(category.CategoryImagePath) && (request.CategoryImage == null || request.CategoryImage.Length == 0))
            {
                throw new BusinessValidationException("Category image is required for parent categories.");
            }

            category.Name = request.CategoryName;
            category.Description = request.Description;
            category.ParentCategoryId = request.ParentCategoryId;

            if (request.CategoryImage != null && request.CategoryImage.Length > 0)
            {
                string originalName = Path.GetFileNameWithoutExtension(request.CategoryImage.FileName).Trim().Replace(" ", "-");
                string extension = Path.GetExtension(request.CategoryImage.FileName);
                string newFileName = $"{originalName}-{Guid.NewGuid()}{extension}";

                using Stream stream = request.CategoryImage.OpenReadStream();
                string newBlobName = !string.IsNullOrEmpty(category.CategoryImagePath)
                    ? await _imageService.ReplaceImageAsync(stream, newFileName, category.CategoryImagePath, BlobContainers.CategoryImages)
                    : await _imageService.UploadImageAsync(stream, newFileName, BlobContainers.CategoryImages);
                category.CategoryImagePath = newBlobName;
            }

            Category updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);
            CategoryResponse categoryResponse = _mapper.Map<Category, CategoryResponse>(updatedCategory);
            if (!string.IsNullOrEmpty(categoryResponse.CategoryImagePath))
            {
                categoryResponse.CategoryImagePath = _imageService.GetImageUrl(BlobContainers.CategoryImages, categoryResponse.CategoryImagePath);
            }
            return categoryResponse;
        }

        public async Task<CategoryResponse> UpdateCategoryStatusAsync(Guid categoryId, bool isActive)
        {
            if (categoryId == Guid.Empty)
                throw new BusinessValidationException("Category Id cannot be empty.");

            Category? category = await _categoryRepository.GetCategoryByIdAdminAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            category.IsActive = isActive;
            await _categoryRepository.UpdateCategoryStatusAsync(categoryId, isActive);

            CategoryResponse categoryResponse = _mapper.Map<Category, CategoryResponse>(category);
            if (!string.IsNullOrEmpty(categoryResponse.CategoryImagePath))
            {
                categoryResponse.CategoryImagePath = _imageService.GetImageUrl(BlobContainers.CategoryImages, categoryResponse.CategoryImagePath);
            }
            return categoryResponse;
        }
    }
}
