using Orders.Shared.DTOs;
namespace Orders.Bll.DTOs.Analytics;
public record DashboardDto(
    RevenueSummaryDto Revenue,
    OrderSummaryDto Orders,
    IEnumerable<TopDishDto> TopDishes,
    IEnumerable<TopCategoryDto> TopCategories,
    PaymentSummaryDto Payments,
    CustomerRetentionDto CustomerRetention,
    PeriodComparisonDto Comparison);