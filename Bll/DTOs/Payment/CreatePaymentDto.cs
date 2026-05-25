namespace Orders.Bll.DTOs.Payment;

public record CreatePaymentDto(
    int    OrderId,
    string PaymentMethod);