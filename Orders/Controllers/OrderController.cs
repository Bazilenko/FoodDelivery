using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Bll.DTOs.Order;
using Orders.Bll.Services.Interfaces;
using System.Security.Claims;
using Orders.Dal.Enums;
using Orders.Api.Helpers;

namespace Orders.Api.Controllers
{
    [ApiController]
    [Route("orders/")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(
            IOrderService orderService,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderRequestDto dto)
        {
            var customerId = AuthHelper.GetUserIdFromToken(_httpContextAccessor);

            var createdOrder = await _orderService.CreateOrderAsync(dto, customerId);

            return CreatedAtAction(nameof(GetCustomerOrderById), new { orderId = createdOrder.Id }, createdOrder);
        }


        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetCustomerOrders()
        {
            var customerId = AuthHelper.GetUserIdFromToken(_httpContextAccessor);
            var orders = await _orderService.GetCustomerOrdersAsync(customerId);

            return Ok(orders);
        }


        [HttpGet("{orderId}/customer")]
        public async Task<ActionResult<OrderResponseDto>> GetCustomerOrderById(int orderId)
        {
            var customerId = AuthHelper.GetUserIdFromToken(_httpContextAccessor);
            var order = await _orderService.GetOrderByIdForCustomerAsync(orderId, customerId);

            if (order == null)
            {
                return NotFound(new { Message = "Замовлення не знайдено або ви не маєте до нього доступу." });
            }

            return Ok(order);
        }



        [HttpGet("restaurant-orders")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetRestaurantOrders()
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var orders = await _orderService.GetRestaurantOrdersAsync(restaurantId);

            return Ok(orders);
        }

        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto dto)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, restaurantId, dto);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("owner")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOwnerOrders(
    [FromQuery] string? status = null)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var orders = await _orderService.GetRestaurantOrdersAsync(restaurantId);

            // Фільтрація за статусом
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<OrderStatus>(status, true, out var statusEnum))
                {
                    orders = orders.Where(o => o.Status == statusEnum).ToList();
                }
                else
                {
                    return BadRequest(new { Message = $"Invalid status value: {status}" });
                }
            }

            return Ok(orders);
        }

        [HttpGet("owner/{orderId}")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<ActionResult<OrderResponseDto>> GetOwnerOrderById(int orderId)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var order = await _orderService.GetOrderByIdForRestaurantAsync(orderId, restaurantId);

            if (order == null)
            {
                return NotFound(new { Message = "Order not found" });
            }

            return Ok(order);
        }

        [HttpPatch("owner/{orderId}/status")]
        [Authorize(Roles = "RestaurantOwner")]
        public async Task<IActionResult> UpdateOwnerOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto dto)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, restaurantId, dto);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}