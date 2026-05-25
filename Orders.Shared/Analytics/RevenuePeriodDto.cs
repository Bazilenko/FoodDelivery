namespace Orders.Shared.DTOs;

public record RevenuePeriodDto(
    string Period,
    decimal Revenue,
    decimal DeliveryFees,
    decimal FoodRevenue,
    int OrderCount);