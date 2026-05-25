namespace Orders.Shared.DTOs;

public record TopOptionDto(
    int OptionId,
    string OptionNameSnapshot,
    int TotalQuantityOrdered,
    decimal TotalRevenue);