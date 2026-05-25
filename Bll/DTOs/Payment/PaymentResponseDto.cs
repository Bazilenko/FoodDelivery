namespace Orders.Bll.DTOs.Payment;

public record PaymentResponseDto(
    int Id,
    int OrderId,
    decimal Amount,
    string Status,          
    string PaymentMethod,
    string? TransactionId,   
    DateTime CreatedAt,
    DateTime? UpdatedAt);