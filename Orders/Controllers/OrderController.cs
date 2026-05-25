using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Bll.DTOs.Order;
using Orders.Bll.Services.Interfaces;
using System.Security.Claims;

namespace Orders.Api.Controllers
{
    [ApiController]
    [Route("orders/")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

      

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderRequestDto dto)
        {
            var customerId = GetUserIdFromToken();
            
            var createdOrder = await _orderService.CreateOrderAsync(dto, customerId);
            
            return CreatedAtAction(nameof(GetCustomerOrderById), new { orderId = createdOrder.Id }, createdOrder);
        }

    
        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetCustomerOrders()
        {
            var customerId = GetUserIdFromToken();
            var orders = await _orderService.GetCustomerOrdersAsync(customerId);
            
            return Ok(orders);
        }

 
        [HttpGet("{orderId}/customer")]
        public async Task<ActionResult<OrderResponseDto>> GetCustomerOrderById(int orderId)
        {
            var customerId = GetUserIdFromToken();
            var order = await _orderService.GetOrderByIdForCustomerAsync(orderId, customerId);

            if (order == null)
            {
                return NotFound(new { Message = "Замовлення не знайдено або ви не маєте до нього доступу." });
            }

            return Ok(order);
        }



        [HttpGet("restaurant-orders")]
        //[Authorize(Roles = "RestaurantOwner")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetRestaurantOrders()
        {
            var restaurantId = GetRestaurantIdFromToken();
            var orders = await _orderService.GetRestaurantOrdersAsync(restaurantId);
            
            return Ok(orders);
        }

        [HttpPatch("{orderId}/status")]
        //[Authorize(Roles = "RestaurantOwner")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequestDto dto)
        {
            var restaurantId = GetRestaurantIdFromToken();

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



        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var userId) ? userId : 0;
        }

        private int GetRestaurantIdFromToken()
        {
            var claim = User.FindFirst("restaurant_id")?.Value;
            return int.TryParse(claim, out var restaurantId) ? restaurantId : 0;
        }
    }
}