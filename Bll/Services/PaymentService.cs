using Orders.Dal.UoW.Interfaces;
using Orders.Bll.Services.Interfaces;
using Orders.Bll.DTOs.Payment;
using Orders.Dal.Enums;
using Orders.Dal.Entities;


namespace Orders.Bll.Services;
 
public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
 
    public PaymentService(IUnitOfWork uow) => _uow = uow;
 
    public async Task<PaymentResponseDto> CreatePaymentAsync(int orderId, string paymentMethod)
    {
        var order = await _uow.Orders.GetAsync(orderId)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");
 
        if (order.Payment is not null)
            throw new InvalidOperationException(
                $"Order {orderId} already has a payment (status: '{order.Payment.Status}').");
 
        if (order.Status != OrderStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot create payment for an order with status '{order.Status}'.");
 
        var payment = new Payment
        {
            OrderId       = orderId,
            Amount        = order.TotalAmount, 
            Status        = PaymentStatus.Pending,
            PaymentMethod = paymentMethod,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };
 
        var newId = await _uow.Payments.AddAsync(payment);
        payment.Id = newId;
 
        return Map(payment);
    }
 
    public async Task MarkAsPaidAsync(int paymentId)
    {
        var payment = await GetPaymentOrThrowAsync(paymentId);
 
        if (payment.Status != PaymentStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot mark as Paid — current status is '{payment.Status}'.");
 
        _uow.BeginTransaction();
        try
        {
            payment.Status        = PaymentStatus.Paid;
            payment.TransactionId = $"TXN-{Guid.NewGuid():N}";
            payment.UpdatedAt     = DateTime.UtcNow;
 
            await _uow.Payments.ReplaceAsync(payment);
 
            // Advance order to Confirmed
            var order = await _uow.Orders.GetAsync(payment.OrderId)
                ?? throw new InvalidOperationException($"Order {payment.OrderId} not found.");
 
            if (order.Status == OrderStatus.Pending)
            {
                order.Status    = OrderStatus.Confirmed;
                order.UpdatedAt = DateTime.UtcNow;
                await _uow.Orders.ReplaceAsync(order);
 
                var historyEntry = new OrderStatusHistory
                {
                    OrderId   = order.Id,
                    Status    = OrderStatus.Confirmed,
                    Comment   = "Payment confirmed.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                await _uow.StatusHistory.AddAsync(historyEntry);
            }
 
            _uow.Commit();
        }
        catch
        {
            _uow.Rollback();
            throw;
        }
    }
 
    public async Task MarkAsFailedAsync(int paymentId)
    {
        var payment = await GetPaymentOrThrowAsync(paymentId);
 
        if (payment.Status != PaymentStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot mark as Failed — current status is '{payment.Status}'.");
 
        _uow.BeginTransaction();
        try
        {
            payment.Status    = PaymentStatus.Failed;
            payment.UpdatedAt = DateTime.UtcNow;
            await _uow.Payments.ReplaceAsync(payment);
 
            var order = await _uow.Orders.GetAsync(payment.OrderId);
            if (order is not null)
            {
                order.Status    = OrderStatus.Failed;
                order.UpdatedAt = DateTime.UtcNow;
                await _uow.Orders.ReplaceAsync(order);
 
                await _uow.StatusHistory.AddAsync(new OrderStatusHistory
                {
                    OrderId   = order.Id,
                    Status    = OrderStatus.Failed,
                    Comment   = "Payment failed.",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                });
            }
 
            _uow.Commit();
        }
        catch
        {
            _uow.Rollback();
            throw;
        }
    }
 
    public async Task RefundAsync(int paymentId)
    {
        var payment = await GetPaymentOrThrowAsync(paymentId);
 
        if (payment.Status != PaymentStatus.Paid)
            throw new InvalidOperationException(
                $"Cannot refund — status is '{payment.Status}'. Only Paid payments can be refunded.");
 
        payment.Status    = PaymentStatus.Refunded;
        payment.UpdatedAt = DateTime.UtcNow;
 
        await _uow.Payments.ReplaceAsync(payment);
    }
    private async Task<Payment> GetPaymentOrThrowAsync(int paymentId)
        => await _uow.Payments.GetAsync(paymentId)
            ?? throw new KeyNotFoundException($"Payment {paymentId} not found.");
 
    private static PaymentResponseDto Map(Payment p) => new(
        p.Id,
        p.OrderId,
        p.Amount,
        p.Status.ToString(),
        p.PaymentMethod,
        p.TransactionId,
        p.CreatedAt,
        p.UpdatedAt);
}