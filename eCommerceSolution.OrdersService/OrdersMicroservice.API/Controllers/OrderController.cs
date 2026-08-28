using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Core.DTO;
using OrdersService.Core.Exceptions;
using OrdersService.Core.ServiceContracts;

namespace OrdersMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder(OrderAddRequest orderAddRequest)
        {
            Guid userId = GetUserId();
            OrderResponse orderResponse = await _orderService.CreateOrderAsync(userId, orderAddRequest);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = orderResponse.OrderId }, orderResponse);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(Guid orderId)
        {
            Guid userId = GetUserId();
            OrderResponse orderResponse = await _orderService.GetOrderByIdAsync(userId, orderId);
            return Ok(orderResponse);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetUserOrders()
        {
            Guid userId = GetUserId();
            List<OrderResponse> orderResponses = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orderResponses);
        }

        [HttpPatch("{orderId:guid}/cancel")]
        public async Task<ActionResult<OrderResponse>> CancelOrder(Guid orderId)
        {
            Guid userId = GetUserId();
            OrderResponse orderResponse = await _orderService.CancelOrderAsync(userId, orderId);
            return Ok(orderResponse);
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                throw new UnauthorizedOperationException("Invalid or missing authentication token.");
            return parsedUserId;
        }
    }
}
