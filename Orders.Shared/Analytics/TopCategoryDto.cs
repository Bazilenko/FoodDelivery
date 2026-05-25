namespace Orders.Shared.DTOs;

public record TopCategoryDto(
    string CategorySnapshot,
    int TotalQuantityOrdered,
    decimal TotalRevenue,
    int DishCount);