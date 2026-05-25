using Orders.Shared.DTOs;
using Orders.Bll.Helpers;
using Orders.Bll.Services.Interfaces;
using Orders.Dal.UoW.Interfaces;
using Orders.Bll.DTOs.Analytics;
using Orders.Shared.Context;

namespace Orders.Bll.Services;

public class AdminAnalyticsService : IAdminAnalyticsService
{
    private readonly IUnitOfWork _uow;
    private readonly IRestaurantContext _context;

    public AdminAnalyticsService(IUnitOfWork uow, IRestaurantContext context)
    {
        _uow = uow;
        _context = context;

    }


    private AnalyticsFilter Filter(AnalyticsRequest req)
        => AnalyticsGuard.BuildFilter(req, restaurantId: _context.RestaurantId);

    public async Task<DashboardDto> GetPlatformDashboardAsync(
        AnalyticsRequest req, CancellationToken ct = default)
    {
        var filter = Filter(req);
        var prevFilter = AnalyticsGuard.BuildPreviousPeriodFilter(filter, restaurantId: _context.RestaurantId);

        var revenueTask = _uow.Analytics.GetRevenueSummaryAsync(filter, ct);
        var prevRevenueTask = _uow.Analytics.GetRevenueSummaryAsync(prevFilter, ct);
        var ordersTask = _uow.Analytics.GetOrderSummaryAsync(filter, ct);
        var topDishesTask = _uow.Analytics.GetTopDishesByRevenueAsync(filter, req.TopN, ct);
        var topCatsTask = _uow.Analytics.GetTopCategoriesAsync(filter, req.TopN, ct);
        var paymentsTask = _uow.Analytics.GetPaymentSummaryAsync(filter, ct);
        var retentionTask = _uow.Analytics.GetCustomerRetentionAsync(filter, ct);

        await Task.WhenAll(
            revenueTask, prevRevenueTask, ordersTask,
            topDishesTask, topCatsTask, paymentsTask, retentionTask);

        var revenue = await revenueTask;
        var prevRevenue = await prevRevenueTask;

        return new DashboardDto(
            Revenue: revenue,
            Orders: await ordersTask,
            TopDishes: await topDishesTask,
            TopCategories: await topCatsTask,
            Payments: await paymentsTask,
            CustomerRetention: await retentionTask,
            Comparison: AnalyticsGuard.BuildComparison(revenue, prevRevenue));
    }

    public Task<RevenueSummaryDto> GetPlatformRevenueSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetRevenueSummaryAsync(Filter(req), ct);

    public Task<IEnumerable<RevenuePeriodDto>> GetPlatformRevenueByDayAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetRevenueByDayAsync(Filter(req), ct);

    public Task<IEnumerable<RevenuePeriodDto>> GetPlatformRevenueByWeekAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetRevenueByWeekAsync(Filter(req), ct);

    public Task<IEnumerable<RevenuePeriodDto>> GetPlatformRevenueByMonthAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetRevenueByMonthAsync(Filter(req), ct);

    public Task<OrderSummaryDto> GetPlatformOrderSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetOrderSummaryAsync(Filter(req), ct);

    public Task<IEnumerable<PeakHourDto>> GetPlatformPeakHoursAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetPeakHoursAsync(Filter(req), ct);

    public Task<IEnumerable<TopDishDto>> GetPlatformTopDishesByQuantityAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetTopDishesByQuantityAsync(Filter(req), req.TopN, ct);

    public Task<IEnumerable<TopDishDto>> GetPlatformTopDishesByRevenueAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetTopDishesByRevenueAsync(Filter(req), req.TopN, ct);

    public Task<IEnumerable<TopCategoryDto>> GetPlatformTopCategoriesAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetTopCategoriesAsync(Filter(req), req.TopN, ct);

    public Task<IEnumerable<TopOptionDto>> GetPlatformTopOptionsAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetTopOptionsAsync(Filter(req), req.TopN, ct);

    public Task<IEnumerable<TopCustomerDto>> GetPlatformTopCustomersAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetTopCustomersAsync(Filter(req), req.TopN, ct);

    public Task<CustomerRetentionDto> GetPlatformCustomerRetentionAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetCustomerRetentionAsync(Filter(req), ct);

    public Task<PaymentSummaryDto> GetPlatformPaymentSummaryAsync(
        AnalyticsRequest req, CancellationToken ct = default)
        => _uow.Analytics.GetPaymentSummaryAsync(Filter(req), ct);
}