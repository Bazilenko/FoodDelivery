using Orders.Shared.DTOs;
namespace Orders.Dal.Repository.Interfaces;
public interface IAnalyticsRepository
{

    /// <summary>Aggregate revenue summary for the period.</summary>
    Task<RevenueSummaryDto> GetRevenueSummaryAsync(
        AnalyticsFilter filter, CancellationToken ct = default);

    /// <summary>Revenue broken down day by day.</summary>
    Task<IEnumerable<RevenuePeriodDto>> GetRevenueByDayAsync(
        AnalyticsFilter filter, CancellationToken ct = default);

    /// <summary>Revenue broken down week by week (ISO week number).</summary>
    Task<IEnumerable<RevenuePeriodDto>> GetRevenueByWeekAsync(
        AnalyticsFilter filter, CancellationToken ct = default);

    /// <summary>Revenue broken down month by month.</summary>
    Task<IEnumerable<RevenuePeriodDto>> GetRevenueByMonthAsync(
        AnalyticsFilter filter, CancellationToken ct = default);


    /// <summary>Order counts broken down by status with cancellation rate.</summary>
    Task<OrderSummaryDto> GetOrderSummaryAsync(
        AnalyticsFilter filter, CancellationToken ct = default);

    /// <summary>Order volume and revenue grouped by hour of day (0–23).</summary>
    Task<IEnumerable<PeakHourDto>> GetPeakHoursAsync(
        AnalyticsFilter filter, CancellationToken ct = default);


    /// <summary>Top N dishes ranked by total quantity ordered.</summary>
    Task<IEnumerable<TopDishDto>> GetTopDishesByQuantityAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default);

    /// <summary>Top N dishes ranked by total revenue generated.</summary>
    Task<IEnumerable<TopDishDto>> GetTopDishesByRevenueAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default);

    /// <summary>Top N categories ranked by total quantity ordered.</summary>
    Task<IEnumerable<TopCategoryDto>> GetTopCategoriesAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default);

    /// <summary>Top N modifier options ranked by how often they were selected.</summary>
    Task<IEnumerable<TopOptionDto>> GetTopOptionsAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default);


    /// <summary>Top N customers ranked by total spend in the period.</summary>
    Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(
        AnalyticsFilter filter, int topN = 10, CancellationToken ct = default);

    /// <summary>New vs returning customers and retention rate.</summary>
    Task<CustomerRetentionDto> GetCustomerRetentionAsync(
        AnalyticsFilter filter, CancellationToken ct = default);


    /// <summary>Payment breakdown by method and refund rate.</summary>
    Task<PaymentSummaryDto> GetPaymentSummaryAsync(
        AnalyticsFilter filter, CancellationToken ct = default);
}