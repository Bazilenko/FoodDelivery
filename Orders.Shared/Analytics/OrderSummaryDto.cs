namespace Orders.Shared.DTOs;

public record OrderSummaryDto(
    int TotalOrders,
    int DeliveredOrders,
    int CancelledOrders,
    int FailedOrders,
    decimal CancellationRate,
    IEnumerable<OrderStatusBreakdownDto> ByStatus);