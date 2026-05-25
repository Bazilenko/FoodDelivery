using Orders.Dal.Entities;

namespace Orders.Dal.Repository.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<Payment?> GetByOrderIdAsync(int orderId);
    }
}
