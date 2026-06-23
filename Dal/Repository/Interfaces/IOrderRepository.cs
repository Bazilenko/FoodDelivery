using Orders.Dal.Enums;
using Orders.Dal.Entities;

namespace Orders.Dal.Repository.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Entities.Order>
    {
        Task<IEnumerable<Entities.Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<Entities.Order?> GetFullOrderDetailsAsync(int orderId);
        Task UpdateStatusAsync(int orderId, OrderStatus status);
        Task<decimal> GetOrderTotalAmountAsync(int orderId);
        Task<decimal> GetTotalRevenueAsync(int restaurantId, DateTime from, DateTime to);
        Task<int> GetOrdersCountAsync(int restaurantId, DateTime from, DateTime to);
        Task<IEnumerable<dynamic>> GetDailyRevenueAsync(int restaurantId, DateTime from, DateTime to);
        Task<IEnumerable<Entities.Order>> GetOrdersByRestaurantIdAsync(int restaurantId);
        Task<Order?> GetOrderByIdForRestaurantAsync(int orderId, int restaurantId);
    }
}
