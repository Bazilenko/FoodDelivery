using Dapper;
using Dommel;
using Orders.Dal.Context.Interfaces;
using Orders.Dal.Entities;
using Orders.Dal.Enums;
using Orders.Dal.Repository.Interfaces;

namespace Orders.Dal.Repository
{
    public class OrderRepository : GenericRepository<Entities.Order>, IOrderRepository
    {
        public OrderRepository(IDapperContext context) : base(context) { }
        public async Task<decimal> GetTotalRevenueAsync(int restaurantId, DateTime from, DateTime to)
        {
            const string sql = @"
            SELECT SUM(TotalAmount) 
            FROM Orders 
            WHERE RestaurantId = @RestaurantId 
              AND Status = 'Delivered' 
              AND CreatedAt BETWEEN @From AND @To";

            return await _context.Connection.ExecuteScalarAsync<decimal>(sql,
                new { RestaurantId = restaurantId, From = from, To = to },
                _context.Transaction);
        }
        public async Task<int> GetOrdersCountAsync(int restaurantId, DateTime from, DateTime to)
        {
            const string sql = @"
            SELECT COUNT(*) 
            FROM Orders 
            WHERE RestaurantId = @RestaurantId 
              AND CreatedAt BETWEEN @From AND @To";

            return await _context.Connection.ExecuteScalarAsync<int>(sql,
                new
                {
                    RestaurantId = restaurantId,
                    From = from,
                    To = to
                },
                _context.Transaction);
        }

        public async Task<IEnumerable<dynamic>> GetDailyRevenueAsync(int restaurantId, DateTime from, DateTime to)
        {
            const string sql = @"
            SELECT CAST(CreatedAt AS DATE) AS Date, SUM(TotalAmount) AS Revenue
            FROM Orders
            WHERE RestaurantId = @RestaurantId AND Status = 'Delivered'
              AND CreatedAt BETWEEN @From AND @To
            GROUP BY CAST(CreatedAt AS DATE)
            ORDER BY Date";

            return await _context.Connection.QueryAsync(sql,
                new { RestaurantId = restaurantId, From = from, To = to },
                _context.Transaction);
        }

        public async Task<IEnumerable<Entities.Order>> GetOrdersByRestaurantIdAsync(int restaurantId)
        {
            const string sql = @"
        SELECT o.*, od.*, odo.*
        FROM Orders o
        LEFT JOIN OrderDishes od ON o.Id = od.OrderId
        LEFT JOIN OrderDishOptions odo ON od.Id = odo.OrderDishId
        WHERE o.RestaurantId = @RestaurantId
        ORDER BY o.CreatedAt DESC";

            return await MapOrderWithDetails(sql, new { RestaurantId = restaurantId });
        }
        public async Task<Entities.Order?> GetFullOrderDetailsAsync(int orderId)
        {
            const string sql = @"
                SELECT * FROM Orders o
                LEFT JOIN OrderDishes od ON o.Id = od.OrderId
                LEFT JOIN OrderDishOptions odo ON od.Id = odo.OrderDishId
                LEFT JOIN OrderStatusHistory osh ON o.Id = osh.OrderId
                LEFT JOIN Payments p ON o.Id = p.OrderId
                WHERE o.Id = @Id";

            var orderDictionary = new Dictionary<int, Entities.Order>();
            var dishDictionary = new Dictionary<int, OrderDish>();

            await _context.Connection.QueryAsync<Entities.Order, OrderDish, OrderDishOption, OrderStatusHistory, Payment, Entities.Order>(
                sql,
                (order, dish, option, history, payment) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }

                    if (payment != null)
                    {
                        orderEntry.Payment = payment;
                    }

                    if (history != null && !orderEntry.StatusHistory.Any(h => h.Id == history.Id))
                    {
                        orderEntry.StatusHistory.Add(history);
                    }

                    if (dish != null)
                    {
                        if (!dishDictionary.TryGetValue(dish.Id, out var dishEntry))
                        {
                            dishEntry = dish;
                            dishDictionary.Add(dishEntry.Id, dishEntry);
                            orderEntry.OrderDishes.Add(dishEntry);
                        }

                        if (option != null && !dishEntry.OrderDishOptions.Any(o => o.Id == option.Id))
                        {
                            dishEntry.OrderDishOptions.Add(option);
                        }
                    }

                    return orderEntry;
                },
                param: new { Id = orderId },
                transaction: _context.Transaction);

            return orderDictionary.Values.FirstOrDefault();
        }

        private async Task<IEnumerable<Entities.Order>> MapOrderWithDetails(string sql, object parameters)
        {
            var orderDict = new Dictionary<int, Entities.Order>();

            await _context.Connection.QueryAsync<Entities.Order, OrderDish, OrderDishOption, Entities.Order>(
                sql,
                (order, dish, option) =>
                {
                    if (!orderDict.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.OrderDishes = new List<OrderDish>();
                        orderDict.Add(orderEntry.Id, orderEntry);
                    }

                    if (dish != null)
                    {
                        var dishEntry = orderEntry.OrderDishes.FirstOrDefault(d => d.Id == dish.Id);
                        if (dishEntry == null)
                        {
                            dishEntry = dish;
                            dishEntry.OrderDishOptions = new List<OrderDishOption>();
                            orderEntry.OrderDishes.Add(dishEntry);
                        }

                        if (option != null)
                        {
                            dishEntry.OrderDishOptions.Add(option);
                        }
                    }
                    return orderEntry;
                },
                parameters,
                transaction: _context.Transaction);

            return orderDict.Values;
        }

        public async Task<IEnumerable<Entities.Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            const string sql = @"
        SELECT o.*, od.*, odo.*
        FROM Orders o
        LEFT JOIN OrderDishes od ON o.Id = od.OrderId
        LEFT JOIN OrderDishOptions odo ON od.Id = odo.OrderDishId
        WHERE o.CustomerId = @CustomerId
        ORDER BY o.CreatedAt DESC";

            return await MapOrderWithDetails(sql, new { CustomerId = customerId });
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            const string sql = "UPDATE Orders SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            await _context.Connection.ExecuteAsync(sql,
                new { Id = orderId, Status = (int)status, UpdatedAt = DateTime.UtcNow },
                _context.Transaction);
        }

        public async Task<decimal> GetOrderTotalAmountAsync(int orderId)
        {
            const string sql = "SELECT TotalAmount FROM Orders WHERE Id = @Id";
            return await _context.Connection.ExecuteScalarAsync<decimal>(sql,
                new { Id = orderId },
                _context.Transaction);
        }
    }
}