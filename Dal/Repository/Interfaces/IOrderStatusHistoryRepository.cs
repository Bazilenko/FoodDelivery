using Orders.Dal.Entities;

namespace Orders.Dal.Repository.Interfaces
{
    public interface IOrderStatusHistoryRepository : IGenericRepository<OrderStatusHistory>
    {
        Task<IEnumerable<OrderStatusHistory>> GetHistoryByOrderIdAsync(int orderId);

    }
}