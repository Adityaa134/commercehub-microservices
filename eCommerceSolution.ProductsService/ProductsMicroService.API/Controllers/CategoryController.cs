using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Core.Constants;
using ProductsService.Core.DTO;
using ProductsService.Core.ServiceContracts;

namespace ProductsMicroService.API.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromForm] CategoryAddRequest categoryAddRequest)
        {
            CategoryResponse categoryResponse = await _categoryService.CreateCategoryAsync(categoryAddRequest);
            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = categoryResponse.CategoryId }, categoryResponse);
        }

        [HttpGet("{categoryId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponse>> GetCategoryById(Guid categoryId)
        {
            CategoryResponse categoryResponse = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(categoryResponse);
        }

        [HttpGet("admin/{categoryId:guid}")]
        public async Task<ActionResult<CategoryResponse>> GetCategoryByIdAdmin(Guid categoryId)
        {
            CategoryResponse categoryResponse = await _categoryService.GetCategoryByIdAdminAsync(categoryId);
            return Ok(categoryResponse);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryResponse>>> GetCategories()
        {
            List<CategoryResponse> categoryResponses = await _categoryService.GetCategoriesAsync();
            return Ok(categoryResponses);
        }

        [HttpGet("admin")]
        public async Task<ActionResult<List<CategoryResponse>>> GetCategoriesAdmin()
        {
            List<CategoryResponse> categoryResponses = await _categoryService.GetCategoriesAdminAsync();
            return Ok(categoryResponses);
        }

        [HttpPut("{categoryId:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<CategoryResponse>> UpdateCategory(Guid categoryId, [FromForm] CategoryUpdateRequest categoryUpdateRequest)
        {
            CategoryResponse categoryResponse = await _categoryService.UpdateCategoryAsync(categoryId, categoryUpdateRequest);
            return Ok(categoryResponse);
        }

        [HttpPatch("{categoryId:guid}/status")]
        public async Task<ActionResult<CategoryResponse>> UpdateCategoryStatus(Guid categoryId,CategoryStatusUpdateRequest request)
        {
            CategoryResponse categoryResponse = await _categoryService.UpdateCategoryStatusAsync(categoryId, request.IsActive);
            return Ok(categoryResponse);
        }
    }
}
