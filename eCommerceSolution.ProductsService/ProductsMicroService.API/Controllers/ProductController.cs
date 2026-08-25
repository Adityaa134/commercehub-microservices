using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Core.Constants;
using ProductsService.Core.DTO;
using ProductsService.Core.ServiceContracts;

namespace ProductsMicroService.API.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{productId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponse>> GetProductById(Guid productId)
        {
            ProductResponse productResponse = await _productService.GetProductByIdAsync(productId);
            return Ok(productResponse);
        }

        [HttpGet("admin/{productId:guid}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<ProductResponse>> GetProductByIdAdmin(Guid productId)
        {
            ProductResponse productResponse = await _productService.GetProductByIdAdminAsync(productId);
            return Ok(productResponse);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductResponse>> AddProduct([FromForm] ProductAddRequest productAddRequest)
        {
            ProductResponse productResponse = await _productService.AddProductAsync(productAddRequest);
            return CreatedAtAction(nameof(GetProductById), new { productId = productResponse.ProductId }, productResponse);
        }

        [HttpPut("{productId:guid}")]
        [Authorize(Roles = Roles.Admin)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductResponse>> UpdateProduct(Guid productId, [FromForm] ProductUpdateRequest productUpdateRequest)
        {
            ProductResponse productResponse = await _productService.UpdateProductAsync(productId, productUpdateRequest);
            return Ok(productResponse);
        }

        [HttpPatch("{productId:guid}/status")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<ProductResponse>> UpdateProductStatus(Guid productId, ProductStatusUpdateRequest request)
        {
            ProductResponse productResponse = await _productService.UpdateProductStatusAsync(productId, request.IsActive);
            return Ok(productResponse);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<List<ProductBatchResponse>>> GetProductsByIds(List<Guid> productIds)
        {
            List<ProductBatchResponse> products = await _productService.GetProductsByIdsAsync(productIds);
            return Ok(products);
        }
    }
}
