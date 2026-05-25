using Dapper;
using Dommel;
using Orders.Dal.Context.Interfaces;
using Orders.Dal.Entities;
using Orders.Dal.Repository.Interfaces;

namespace Orders.Dal.Repository
{
    public class OrderDishRepository : GenericRepository<OrderDish>, IOrderDishRepository
    {
        public OrderDishRepository(IDapperContext context) : base(context) { }

        public async Task<OrderDish?> GetDishWithOptionsAsync(int orderDishId)
        {
            const string sql = @"
                SELECT * FROM OrderDishes od
                LEFT JOIN OrderDishOptions odo ON od.Id = odo.OrderDishId
                WHERE od.Id = @Id";

            var dishDictionary = new Dictionary<int, OrderDish>();

            await _context.Connection.QueryAsync<OrderDish, OrderDishOption, OrderDish>(
                sql,
                (dish, option) =>
                {
                    if (!dishDictionary.TryGetValue(dish.Id, out var dishEntry))
                    {
                        dishEntry = dish;
                        dishDictionary.Add(dishEntry.Id, dishEntry);
                    }

                    if (option != null)
                    {
                        dishEntry.OrderDishOptions.Add(option);
                    }

                    return dishEntry;
                },
                param: new { Id = orderDishId },
                transaction: _context.Transaction);

            return dishDictionary.Values.FirstOrDefault();
        }


        public async Task<IEnumerable<OrderDish>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Connection.SelectAsync<OrderDish>(
                od => od.OrderId == orderId,
                _context.Transaction);
        }

        public async Task UpdateQuantityAsync(int orderDishId, int quantity)
        {
            const string sql = "UPDATE OrderDishes SET Quantity = @Quantity, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            await _context.Connection.ExecuteAsync(sql,
                new { Id = orderDishId, Quantity = quantity, UpdatedAt = DateTime.UtcNow },
                _context.Transaction);
        }
    }
}