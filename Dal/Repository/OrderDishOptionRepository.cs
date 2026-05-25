using Orders.Dal.Context.Interfaces;
using Orders.Dal.Entities;
using Orders.Dal.Repository.Interfaces;
using Dommel;

namespace Orders.Dal.Repository
{
    public class OrderDishOptionRepository : GenericRepository<OrderDishOption>, IOrderDishOptionRepository
    {
        public OrderDishOptionRepository(IDapperContext context) : base(context) { }

        public async Task<IEnumerable<OrderDishOption>> GetByOrderDishIdAsync(int orderDishId)
        {
            return await _context.Connection.SelectAsync<OrderDishOption>(
                odo => odo.OrderDishId == orderDishId, 
                _context.Transaction);
        }
    }
}