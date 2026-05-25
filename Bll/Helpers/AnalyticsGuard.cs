using Orders.Bll.DTOs.Analytics;
using Orders.Shared.DTOs;
namespace Orders.Bll.Helpers;
internal static class AnalyticsGuard
{
    private const int MaxRangeDays = 366;
    private const int MaxTopN      = 100;
    private const int MinTopN      = 1;
 
    internal static void Validate(AnalyticsRequest req)
    {
        if (req.From >= req.To)
            throw new ArgumentException("'From' must be earlier than 'To'.");
 
        if ((req.To - req.From).TotalDays > MaxRangeDays)
            throw new ArgumentException($"Date range cannot exceed {MaxRangeDays} days.");
 
        if (req.To > DateTime.UtcNow)
            throw new ArgumentException("'To' cannot be in the future.");
 
        if (req.TopN is < MinTopN or > MaxTopN)
            throw new ArgumentException($"TopN must be between {MinTopN} and {MaxTopN}.");
    }
 
    internal static AnalyticsFilter BuildFilter(AnalyticsRequest req, int? restaurantId)
    {
        Validate(req);
 
        // Normalise to start/end of day so partial-day boundaries don't skew numbers
        var from = req.From.Date;
        var to   = req.To.Date.AddDays(1).AddTicks(-1);   // end of the To day
 
        return new AnalyticsFilter(from, to, restaurantId);
    }
 
    /// <summary>
    /// Returns the filter for the previous period of the same length.
    /// e.g. if current = Jan 1–31, previous = Dec 1–31.
    /// </summary>
    internal static AnalyticsFilter BuildPreviousPeriodFilter(
        AnalyticsFilter current, int? restaurantId)
    {
        var length = current.To - current.From;
        return new AnalyticsFilter(
            current.From - length,
            current.From.AddTicks(-1),
            restaurantId);
    }
 
    internal static PeriodComparisonDto BuildComparison(
        RevenueSummaryDto current, RevenueSummaryDto previous)
    {
        static decimal ChangePercent(decimal curr, decimal prev)
            => prev == 0
                ? (curr > 0 ? 100m : 0m)
                : Math.Round((curr - prev) / prev * 100, 2);
 
        return new PeriodComparisonDto(
            RevenueChangePercent:          ChangePercent(current.TotalRevenue, previous.TotalRevenue),
            OrderCountChangePercent:       ChangePercent(current.TotalDeliveredOrders, previous.TotalDeliveredOrders),
            AverageOrderValueChangePercent:ChangePercent(current.AverageOrderValue, previous.AverageOrderValue),
            CurrentRevenue:                current.TotalRevenue,
            PreviousRevenue:               previous.TotalRevenue,
            CurrentOrderCount:             current.TotalDeliveredOrders,
            PreviousOrderCount:            previous.TotalDeliveredOrders);
    }
}
 