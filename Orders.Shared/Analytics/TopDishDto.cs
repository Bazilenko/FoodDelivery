namespace Orders.Shared.DTOs;

public record TopDishDto(
    int DishId,
    string DishNameSnapshot,
    string? CategorySnapshot,
    int TotalQuantityOrdered,
    decimal TotalRevenue,
    int AppearanceInOrders);