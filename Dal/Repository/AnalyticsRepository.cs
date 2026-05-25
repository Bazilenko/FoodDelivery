using System.Data;
using Dapper;
using Orders.Dal.Repository.Interfaces;
using Orders.Shared.DTOs;
using Orders.Dal.Enums;
using Orders.Dal.Context.Interfaces;

namespace Orders.Dal.Repository;

/// <summary>
/// All queries only count Delivered orders for revenue — Pending/Cancelled orders
/// are not real revenue. Payment status Paid is the source of truth for money received.
/// restaurantId = null means platform-wide (admin); set means restaurant-scoped.
/// </summary>
public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly IDapperContext _context;

    public AnalyticsRepository(IDapperContext context) => _context = context;


    public async Task<RevenueSummaryDto> GetRevenueSummaryAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT
                COALESCE(SUM(o.TotalAmount),  0) AS TotalRevenue,
                COALESCE(SUM(o.DeliveryFee),  0) AS TotalDeliveryFees,
                COALESCE(AVG(o.TotalAmount),  0) AS AverageOrderValue,
                COUNT(*)                          AS TotalDeliveredOrders
            FROM Orders o
            INNER JOIN Payments p ON p.OrderId = o.Id
            WHERE o.Status     = @Delivered
              AND p.Status     = @Paid
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            """;

        var row = await _db.QuerySingleAsync<dynamic>(sql, new
        {
            Delivered = OrderStatus.Delivered.ToString(),
            Paid = PaymentStatus.Paid.ToString(),
            filter.From,
            filter.To,
            filter.RestaurantId
        });

        decimal totalRevenue = row.TotalRevenue;
        decimal deliveryFees = row.TotalDeliveryFees;

        return new RevenueSummaryDto(
            TotalRevenue: totalRevenue,
            TotalDeliveryFees: deliveryFees,
            TotalFoodRevenue: totalRevenue - deliveryFees,
            AverageOrderValue: row.AverageOrderValue,
            TotalDeliveredOrders: row.TotalDeliveredOrders,
            From: filter.From,
            To: filter.To);
    }

    public async Task<IEnumerable<RevenuePeriodDto>> GetRevenueByDayAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                CONVERT(varchar(10), o.CreatedAt, 120) AS Period,
                COALESCE(SUM(o.TotalAmount), 0)        AS Revenue,
                COALESCE(SUM(o.DeliveryFee), 0)        AS DeliveryFees,
                COUNT(*)                               AS OrderCount
            FROM Orders o
            INNER JOIN Payments p ON p.OrderId = o.Id
            WHERE o.Status     = @Delivered
              AND p.Status     = @Paid
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY CONVERT(varchar(10), o.CreatedAt, 120)
            ORDER BY Period
            """;

        return await QueryRevenuePeriodsAsync(sql, filter);
    }

    public async Task<IEnumerable<RevenuePeriodDto>> GetRevenueByWeekAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                CAST(DATEPART(year, o.CreatedAt) AS varchar) + '-W'
                    + RIGHT('0' + CAST(DATEPART(iso_week, o.CreatedAt) AS varchar), 2) AS Period,
                COALESCE(SUM(o.TotalAmount), 0) AS Revenue,
                COALESCE(SUM(o.DeliveryFee), 0) AS DeliveryFees,
                COUNT(*)                        AS OrderCount
            FROM Orders o
            INNER JOIN Payments p ON p.OrderId = o.Id
            WHERE o.Status     = @Delivered
              AND p.Status     = @Paid
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY
                DATEPART(year, o.CreatedAt),
                DATEPART(iso_week, o.CreatedAt)
            ORDER BY
                DATEPART(year, o.CreatedAt),
                DATEPART(iso_week, o.CreatedAt)
            """;

        return await QueryRevenuePeriodsAsync(sql, filter);
    }

    public async Task<IEnumerable<RevenuePeriodDto>> GetRevenueByMonthAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                CONVERT(varchar(7), o.CreatedAt, 120) AS Period,
                COALESCE(SUM(o.TotalAmount), 0)       AS Revenue,
                COALESCE(SUM(o.DeliveryFee), 0)       AS DeliveryFees,
                COUNT(*)                              AS OrderCount
            FROM Orders o
            INNER JOIN Payments p ON p.OrderId = o.Id
            WHERE o.Status     = @Delivered
              AND p.Status     = @Paid
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY CONVERT(varchar(7), o.CreatedAt, 120)
            ORDER BY Period
            """;

        return await QueryRevenuePeriodsAsync(sql, filter);
    }


    public async Task<OrderSummaryDto> GetOrderSummaryAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT
                o.Status,
                COUNT(*) AS Count
            FROM Orders o
            WHERE o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY o.Status
            """;

        var rows = (await _db.QueryAsync<(string Status, int Count)>(sql, new
        {
            filter.From,
            filter.To,
            filter.RestaurantId
        })).ToList();

        var total = rows.Sum(r => r.Count);
        var delivered = rows.FirstOrDefault(r => r.Status == OrderStatus.Delivered.ToString()).Count;
        var cancelledC = rows.FirstOrDefault(r => r.Status == OrderStatus.CancelledByCustomer.ToString()).Count;
        var cancelledR = rows.FirstOrDefault(r => r.Status == OrderStatus.CancelledByRestaurant.ToString()).Count;
        var failed = rows.FirstOrDefault(r => r.Status == OrderStatus.Failed.ToString()).Count;
        var cancelled = cancelledC + cancelledR;

        var breakdown = rows.Select(r => new OrderStatusBreakdownDto(
            r.Status,
            r.Count,
            total > 0 ? Math.Round((decimal)r.Count / total * 100, 2) : 0));

        return new OrderSummaryDto(
            TotalOrders: total,
            DeliveredOrders: delivered,
            CancelledOrders: cancelled,
            FailedOrders: failed,
            CancellationRate: total > 0 ? Math.Round((decimal)cancelled / total * 100, 2) : 0,
            ByStatus: breakdown);
    }

    public async Task<IEnumerable<PeakHourDto>> GetPeakHoursAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT
                DATEPART(hour, o.CreatedAt)    AS Hour,
                COUNT(*)                       AS OrderCount,
                COALESCE(SUM(o.TotalAmount),0) AS Revenue
            FROM Orders o
            WHERE o.Status     = @Delivered
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY DATEPART(hour, o.CreatedAt)
            ORDER BY Hour
            """;

        return await _db.QueryAsync<PeakHourDto>(sql, new
        {
            Delivered = OrderStatus.Delivered.ToString(),
            filter.From,
            filter.To,
            filter.RestaurantId
        });
    }

    public async Task<IEnumerable<TopDishDto>> GetTopDishesByQuantityAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT TOP (@TopN)
                od.DishId,
                od.DishNameSnapshot,
                od.CategorySnapshot,
                SUM(od.Quantity)                         AS TotalQuantityOrdered,
                SUM(od.Quantity * od.PriceAtTimeOfOrder) AS TotalRevenue,
                COUNT(DISTINCT od.OrderId)               AS AppearanceInOrders
            FROM OrderDishes od
            INNER JOIN Orders o ON o.Id = od.OrderId
            WHERE o.Status     = @Delivered
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND od.IsDeleted = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY od.DishId, od.DishNameSnapshot, od.CategorySnapshot
            ORDER BY TotalQuantityOrdered DESC
            """;

        return await _db.QueryAsync<TopDishDto>(sql, new
        {
            TopN = topN,
            Delivered = OrderStatus.Delivered.ToString(),
            filter.From,
            filter.To,
            filter.RestaurantId
        });
    }

    public async Task<IEnumerable<TopDishDto>> GetTopDishesByRevenueAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT TOP (@TopN)
                od.DishId,
                od.DishNameSnapshot,
                od.CategorySnapshot,
                SUM(od.Quantity)                         AS TotalQuantityOrdered,
                SUM(od.Quantity * od.PriceAtTimeOfOrder) AS TotalRevenue,
                COUNT(DISTINCT od.OrderId)               AS AppearanceInOrders
            FROM OrderDishes od
            INNER JOIN Orders o ON o.Id = od.OrderId
            WHERE o.Status     = @Delivered
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND od.IsDeleted = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY od.DishId, od.DishNameSnapshot, od.CategorySnapshot
            ORDER BY TotalRevenue DESC
            """;

        return await _db.QueryAsync<TopDishDto>(sql, new
        {
            TopN = topN,
            Delivered = OrderStatus.Delivered.ToString(),
            filter.From,
            filter.To,
            filter.RestaurantId
        });
    }

    public async Task<IEnumerable<TopCategoryDto>> GetTopCategoriesAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT TOP (@TopN)
                COALESCE(od.CategorySnapshot, 'Uncategorised') AS CategorySnapshot,
                SUM(od.Quantity)                               AS TotalQuantityOrdered,
                SUM(od.Quantity * od.PriceAtTimeOfOrder)       AS TotalRevenue,
                COUNT(DISTINCT od.DishId)                      AS DishCount
            FROM OrderDishes od
            INNER JOIN Orders o ON o.Id = od.OrderId
            WHERE o.Status     = @Delivered
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND od.IsDeleted = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY COALESCE(od.CategorySnapshot, 'Uncategorised')
            ORDER BY TotalRevenue DESC
            """;

        return await _db.QueryAsync<TopCategoryDto>(sql, new
        {
            TopN = topN,
            Delivered = OrderStatus.Delivered.ToString(),
            filter.From,
            filter.To,
            filter.RestaurantId
        });
    }

    public async Task<IEnumerable<TopOptionDto>> GetTopOptionsAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT TOP (@TopN)
                odo.OptionId,
                odo.OptionNameSnapshot,
                -- Each OrderDishOption row = 1 selection (quantity lives on OrderDish)
                -- so we count rows × parent dish quantity
                SUM(od.Quantity)               AS TotalQuantityOrdered,
                SUM(od.Quantity * odo.PriceAtTimeOfOrder) AS TotalRevenue
            FROM OrderDishOptions odo
            INNER JOIN OrderDishes od ON od.Id = odo.OrderDishId
            INNER JOIN Orders o       ON o.Id  = od.OrderId
            WHERE o.Status     = @Delivered
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND od.IsDeleted = 0
              AND odo.IsDeleted = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY odo.OptionId, odo.OptionNameSnapshot
            ORDER BY TotalQuantityOrdered DESC
            """;

        return await _db.QueryAsync<TopOptionDto>(sql, new
        {
            TopN = topN,
            Delivered = OrderStatus.Delivered.ToString(),
            filter.From,
            filter.To,
            filter.RestaurantId
        });
    }


    public async Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT TOP (@TopN)
                o.CustomerId,
                COUNT(*)             AS TotalOrders,
                SUM(o.TotalAmount)   AS TotalSpend,
                AVG(o.TotalAmount)   AS AverageOrderValue,
                MIN(o.CreatedAt)     AS FirstOrderAt,
                MAX(o.CreatedAt)     AS LastOrderAt
            FROM Orders o
            INNER JOIN Payments p ON p.OrderId = o.Id
            WHERE o.Status     = @Confirmed
              AND p.Status     = @Paid
              AND o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY o.CustomerId
            ORDER BY TotalSpend DESC
            """;
            var args = new {
        TopN = topN,
        Confirmed = (int)OrderStatus.Confirmed,
        Paid      = (int)PaymentStatus.Paid,
        filter.From,
        filter.To,
        filter.RestaurantId
};
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(args));
        return await _db.QueryAsync<TopCustomerDto>(sql, new
        {
            TopN = topN,
            Confirmed = (int)OrderStatus.Confirmed,
            Paid      = (int)PaymentStatus.Paid,
            filter.From,
            filter.To,
            filter.RestaurantId
        });
    }

    public async Task<CustomerRetentionDto> GetCustomerRetentionAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        // A "new" customer is one whose very first order ever falls within the period.
        // A "returning" customer had at least one order before the period start.
        var _db = _context.Connection;
        const string sql = """
            WITH CustomerFirstOrder AS (
                SELECT
                    CustomerId,
                    MIN(CreatedAt) AS FirstOrderEver
                FROM Orders
                WHERE IsDeleted = 0
                  AND (@RestaurantId IS NULL OR RestaurantId = @RestaurantId)
                GROUP BY CustomerId
            ),
            PeriodCustomers AS (
                SELECT DISTINCT o.CustomerId
                FROM Orders o
                WHERE o.Status     = @Delivered
                  AND o.CreatedAt >= @From
                  AND o.CreatedAt <= @To
                  AND o.IsDeleted  = 0
                  AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            )
            SELECT
                COUNT(pc.CustomerId)                                            AS TotalCustomers,
                SUM(CASE WHEN cfo.FirstOrderEver >= @From THEN 1 ELSE 0 END)   AS NewCustomers,
                SUM(CASE WHEN cfo.FirstOrderEver < @From  THEN 1 ELSE 0 END)   AS ReturningCustomers
            FROM PeriodCustomers pc
            INNER JOIN CustomerFirstOrder cfo ON cfo.CustomerId = pc.CustomerId
            """;

        var row = await _db.QuerySingleAsync<(int Total, int New, int Returning)>(sql, new
        {
            Delivered = OrderStatus.Delivered.ToString(),
            filter.From,
            filter.To,
            filter.RestaurantId
        });

        return new CustomerRetentionDto(
            TotalCustomers: row.Total,
            NewCustomers: row.New,
            ReturningCustomers: row.Returning,
            RetentionRate: row.Total > 0
                ? Math.Round((decimal)row.Returning / row.Total * 100, 2)
                : 0);
    }


    public async Task<PaymentSummaryDto> GetPaymentSummaryAsync(
        AnalyticsFilter filter, CancellationToken ct = default)
    {
        var _db = _context.Connection;
        const string sql = """
            SELECT
                p.Status,
                p.PaymentMethod,
                COUNT(*)           AS Count,
                SUM(p.Amount)      AS TotalAmount
            FROM Payments p
            INNER JOIN Orders o ON o.Id = p.OrderId
            WHERE o.CreatedAt >= @From
              AND o.CreatedAt <= @To
              AND o.IsDeleted  = 0
              AND p.IsDeleted  = 0
              AND (@RestaurantId IS NULL OR o.RestaurantId = @RestaurantId)
            GROUP BY p.Status, p.PaymentMethod
            """;

        var rows = (await _db.QueryAsync<(string Status, string PaymentMethod, int Count, decimal TotalAmount)>(
            sql, new { filter.From, filter.To, filter.RestaurantId })).ToList();

        var total = rows.Sum(r => r.Count);
        var paid = rows.Where(r => r.Status == PaymentStatus.Paid.ToString()).Sum(r => r.Count);
        var failed = rows.Where(r => r.Status == PaymentStatus.Failed.ToString()).Sum(r => r.Count);
        var refunded = rows.Where(r => r.Status == PaymentStatus.Refunded.ToString()).Sum(r => r.Count);
        var refundAmt = rows.Where(r => r.Status == PaymentStatus.Refunded.ToString()).Sum(r => r.TotalAmount);

        var byMethod = rows
            .GroupBy(r => r.PaymentMethod)
            .Select(g => new PaymentMethodBreakdownDto(
                PaymentMethod: g.Key,
                Count: g.Sum(r => r.Count),
                TotalAmount: g.Sum(r => r.TotalAmount),
                Percentage: total > 0
                    ? Math.Round((decimal)g.Sum(r => r.Count) / total * 100, 2)
                    : 0));

        return new PaymentSummaryDto(
            TotalPayments: total,
            PaidCount: paid,
            FailedCount: failed,
            RefundedCount: refunded,
            TotalRefundedAmount: refundAmt,
            RefundRate: paid > 0 ? Math.Round((decimal)refunded / paid * 100, 2) : 0,
            ByMethod: byMethod);
    }


    private async Task<IEnumerable<RevenuePeriodDto>> QueryRevenuePeriodsAsync(
        string sql, AnalyticsFilter filter)
    {
        var _db = _context.Connection;
        var rows = await _db.QueryAsync<(string Period, decimal Revenue, decimal DeliveryFees, int OrderCount)>(
            sql, new
            {
                Delivered = OrderStatus.Delivered.ToString(),
                Paid = PaymentStatus.Paid.ToString(),
                filter.From,
                filter.To,
                filter.RestaurantId
            });

        return rows.Select(r => new RevenuePeriodDto(
            r.Period,
            r.Revenue,
            r.DeliveryFees,
            FoodRevenue: r.Revenue - r.DeliveryFees,
            r.OrderCount));
    }
}