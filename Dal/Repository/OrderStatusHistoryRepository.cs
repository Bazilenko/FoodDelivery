using Orders.Dal.Context.Interfaces;
using Orders.Dal.Entities;
using Orders.Dal.Repository.Interfaces;
using Dommel;

namespace Orders.Dal.Repository
{
    public class OrderStatusHistoryRepository : GenericRepository<OrderStatusHistory>, IOrderStatusHistoryRepository
    {
        public OrderStatusHistoryRepository(IDapperContext context) : base(context) { }

        public async Task<IEnumerable<OrderStatusHistory>> GetHistoryByOrderIdAsync(int orderId)
        {
            return await _context.Connection.SelectAsync<OrderStatusHistory>(
                osh => osh.OrderId == orderId, 
                _context.Transaction);
        }
    }
}