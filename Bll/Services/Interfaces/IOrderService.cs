using Orders.Bll.DTOs.Order;
using Bll.DTOs.Order;

namespace Orders.Bll.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto dto, int customerId);

        Task<OrderResponseDto?> GetOrderByIdForCustomerAsync(int orderId, int customerId);

        Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(int customerId);

        Task<IEnumerable<OrderResponseDto>> GetRestaurantOrdersAsync(int restaurantId);

        Task UpdateOrderStatusAsync(
            int orderId,
            int restaurantId,
            UpdateOrderStatusRequestDto dto);

        Task<OrderSummaryDto?> GetOrderByIdForRestaurantAsync(int orderId, int restaurantId);
    }
}
