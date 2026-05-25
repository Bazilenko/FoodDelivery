namespace Orders.Shared.DTOs;
public record OrderStatusBreakdownDto(
    string Status,
    int Count,
    decimal Percentage);