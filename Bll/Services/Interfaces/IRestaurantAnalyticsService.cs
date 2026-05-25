namespace Orders.Bll.Services.Interfaces;
using Orders.Bll.DTOs.Analytics;
using Orders.Shared.DTOs;
public interface IRestaurantAnalyticsService
{
    Task<DashboardDto> GetDashboardAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<RevenueSummaryDto> GetRevenueSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<RevenuePeriodDto>> GetRevenueByDayAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<RevenuePeriodDto>> GetRevenueByWeekAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<RevenuePeriodDto>> GetRevenueByMonthAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<OrderSummaryDto> GetOrderSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<PeakHourDto>> GetPeakHoursAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopDishDto>> GetTopDishesByQuantityAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopDishDto>> GetTopDishesByRevenueAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopCategoryDto>> GetTopCategoriesAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopOptionDto>> GetTopOptionsAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<CustomerRetentionDto> GetCustomerRetentionAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<PaymentSummaryDto> GetPaymentSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default);
}