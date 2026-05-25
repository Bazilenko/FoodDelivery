using Orders.Bll.DTOs.Payment;

namespace Orders.Bll.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(int orderId, string paymentMethod);
        Task MarkAsPaidAsync(int paymentId);
        Task MarkAsFailedAsync(int paymentId);
        Task RefundAsync(int paymentId);
    }
}
