namespace Orders.Bll.DTOs.Analytics;
public record PeriodComparisonDto(
    decimal RevenueChangePercent,
    decimal OrderCountChangePercent,
    decimal AverageOrderValueChangePercent,
    // Absolute values for both periods so UI can render both numbers
    decimal CurrentRevenue,
    decimal PreviousRevenue,
    int CurrentOrderCount,
    int PreviousOrderCount);