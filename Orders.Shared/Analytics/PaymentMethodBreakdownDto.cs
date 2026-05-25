namespace Orders.Shared.DTOs;

public record PaymentMethodBreakdownDto(
    string PaymentMethod,
    int Count,
    decimal TotalAmount,
    decimal Percentage);