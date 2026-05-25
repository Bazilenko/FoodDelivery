namespace Orders.Shared.DTOs;

public record RevenueSummaryDto(
    decimal TotalRevenue,
    decimal TotalDeliveryFees,
    decimal TotalFoodRevenue,
    decimal AverageOrderValue,
    int TotalDeliveredOrders,
    DateTime From,
    DateTime To);