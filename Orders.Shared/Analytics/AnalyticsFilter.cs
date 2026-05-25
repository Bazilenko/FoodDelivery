namespace Orders.Shared.DTOs;

public record AnalyticsFilter(
    DateTime From,
    DateTime To,
    int? RestaurantId);