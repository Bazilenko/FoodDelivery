namespace Orders.Shared.DTOs;

public record PaymentSummaryDto(
    int TotalPayments,
    int PaidCount,
    int FailedCount,
    int RefundedCount,
    decimal TotalRefundedAmount,
    decimal RefundRate,
    IEnumerable<PaymentMethodBreakdownDto> ByMethod);