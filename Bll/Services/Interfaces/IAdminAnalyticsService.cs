using Orders.Bll.DTOs.Analytics;
using Orders.Shared.DTOs;
namespace Orders.Bll.Services.Interfaces;
public interface IAdminAnalyticsService
{
    Task<DashboardDto> GetPlatformDashboardAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<RevenueSummaryDto> GetPlatformRevenueSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<RevenuePeriodDto>> GetPlatformRevenueByDayAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<RevenuePeriodDto>> GetPlatformRevenueByWeekAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<RevenuePeriodDto>> GetPlatformRevenueByMonthAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<OrderSummaryDto> GetPlatformOrderSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<PeakHourDto>> GetPlatformPeakHoursAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopDishDto>> GetPlatformTopDishesByQuantityAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopDishDto>> GetPlatformTopDishesByRevenueAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopCategoryDto>> GetPlatformTopCategoriesAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopOptionDto>> GetPlatformTopOptionsAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<IEnumerable<TopCustomerDto>> GetPlatformTopCustomersAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<CustomerRetentionDto> GetPlatformCustomerRetentionAsync(
        AnalyticsRequest req, CancellationToken ct = default);
 
    Task<PaymentSummaryDto> GetPlatformPaymentSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default);
}