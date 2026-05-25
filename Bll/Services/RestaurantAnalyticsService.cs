using Orders.Bll.Services.Interfaces;
using Orders.Dal.UoW.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Context;
using Orders.Bll.DTOs.Analytics;
using Orders.Bll.Helpers;
namespace Orders.Bll.Services;
public class RestaurantAnalyticsService : IRestaurantAnalyticsService
{
    private readonly IUnitOfWork _uow;
    private readonly IRestaurantContext   _restaurantContext;
 
    public RestaurantAnalyticsService(
        IUnitOfWork uow,
        IRestaurantContext restaurantContext)
    {
        _uow              = uow;
        _restaurantContext = restaurantContext;
    }
 
    // Every method builds its own scoped filter — restaurantId always comes
    // from IRestaurantContext, never from the caller. This is the isolation guarantee.
    private AnalyticsFilter Filter(AnalyticsRequest req)
        => AnalyticsGuard.BuildFilter(req, _restaurantContext.RestaurantId);
 
    public async Task<DashboardDto> GetDashboardAsync(
        AnalyticsRequest req, CancellationToken ct = default)
    {
        var filter = Filter(req);
        var prevFilter = AnalyticsGuard.BuildPreviousPeriodFilter(filter, _restaurantContext.RestaurantId);
 
        // All summary queries fire in parallel — single round-trip to DB layer
        var (revenue, prevRevenue, orders, topDishes, topCategories, payments, retention) =
            await FetchDashboardDataAsync(filter, prevFilter, req.TopN, ct);
 
        return new DashboardDto(
            Revenue:           revenue,
            Orders:            orders,
            TopDishes:         topDishes,
            TopCategories:     topCategories,
            Payments:          payments,
            CustomerRetention: retention,
            Comparison:        AnalyticsGuard.BuildComparison(revenue, prevRevenue));
    }
 
    public async Task<RevenueSummaryDto> GetRevenueSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetRevenueSummaryAsync(Filter(req), ct);
 
    public async Task<IEnumerable<RevenuePeriodDto>> GetRevenueByDayAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetRevenueByDayAsync(Filter(req), ct);
 
    public async Task<IEnumerable<RevenuePeriodDto>> GetRevenueByWeekAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetRevenueByWeekAsync(Filter(req), ct);
 
    public async Task<IEnumerable<RevenuePeriodDto>> GetRevenueByMonthAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetRevenueByMonthAsync(Filter(req), ct);
 
    public async Task<OrderSummaryDto> GetOrderSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetOrderSummaryAsync(Filter(req), ct);
 
    public async Task<IEnumerable<PeakHourDto>> GetPeakHoursAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetPeakHoursAsync(Filter(req), ct);
 
    public async Task<IEnumerable<TopDishDto>> GetTopDishesByQuantityAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetTopDishesByQuantityAsync(Filter(req), req.TopN, ct);
 
    public async Task<IEnumerable<TopDishDto>> GetTopDishesByRevenueAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetTopDishesByRevenueAsync(Filter(req), req.TopN, ct);
 
    public async Task<IEnumerable<TopCategoryDto>> GetTopCategoriesAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetTopCategoriesAsync(Filter(req), req.TopN, ct);
 
    public async Task<IEnumerable<TopOptionDto>> GetTopOptionsAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetTopOptionsAsync(Filter(req), req.TopN, ct);
 
    public async Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetTopCustomersAsync(Filter(req), req.TopN, ct);
 
    public async Task<CustomerRetentionDto> GetCustomerRetentionAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetCustomerRetentionAsync(Filter(req), ct);
 
    public async Task<PaymentSummaryDto> GetPaymentSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => await _uow.Analytics.GetPaymentSummaryAsync(Filter(req), ct);
 
    // Fires all dashboard queries concurrently via Task.WhenAll
    private async Task<(
        RevenueSummaryDto Revenue,
        RevenueSummaryDto PrevRevenue,
        OrderSummaryDto Orders,
        IEnumerable<TopDishDto> TopDishes,
        IEnumerable<TopCategoryDto> TopCategories,
        PaymentSummaryDto Payments,
        CustomerRetentionDto Retention)>
        FetchDashboardDataAsync(
            AnalyticsFilter filter,
            AnalyticsFilter prevFilter,
            int topN,
            CancellationToken ct)
    {
        var revenueTask     = _uow.Analytics.GetRevenueSummaryAsync(filter, ct);
        var prevRevenueTask = _uow.Analytics.GetRevenueSummaryAsync(prevFilter, ct);
        var ordersTask      = _uow.Analytics.GetOrderSummaryAsync(filter, ct);
        var topDishesTask   = _uow.Analytics.GetTopDishesByRevenueAsync(filter, topN, ct);
        var topCatsTask     = _uow.Analytics.GetTopCategoriesAsync(filter, topN, ct);
        var paymentsTask    = _uow.Analytics.GetPaymentSummaryAsync(filter, ct);
        var retentionTask   = _uow.Analytics.GetCustomerRetentionAsync(filter, ct);
 
        await Task.WhenAll(
            revenueTask, prevRevenueTask, ordersTask,
            topDishesTask, topCatsTask, paymentsTask, retentionTask);
 
        return (
            await revenueTask,
            await prevRevenueTask,
            await ordersTask,
            await topDishesTask,
            await topCatsTask,
            await paymentsTask,
            await retentionTask);
    }
}