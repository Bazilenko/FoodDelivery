using Orders.Dal.Context.Interfaces;
using Orders.Dal.Entities;
using Orders.Dal.Repository.Interfaces;
using Dommel;

namespace Orders.Dal.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(IDapperContext context) : base(context) { }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
        {
            var result = await _context.Connection.SelectAsync<Payment>(
                p => p.OrderId == orderId, 
                _context.Transaction);
            
            return result.FirstOrDefault();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            var result = await _context.Connection.SelectAsync<Payment>(
                p => p.TransactionId == transactionId, 
                _context.Transaction);
            
            return result.FirstOrDefault();
        }
    }
}