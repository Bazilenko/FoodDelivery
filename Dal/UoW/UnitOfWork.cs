using Orders.Dal.Repository.Interfaces;
using Orders.Dal.Context.Interfaces;
using Orders.Dal.UoW.Interfaces;

namespace Orders.Dal.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDapperContext _context;
        private bool _disposed;

        public IOrderRepository Orders { get; }
        public IOrderDishRepository OrderDishes { get; }
        public IOrderDishOptionRepository OrderDishOptions { get; }
        public IPaymentRepository Payments { get; }
        public IOrderStatusHistoryRepository StatusHistory { get; }
        public IAnalyticsRepository Analytics {get;} 

        public UnitOfWork(
            IDapperContext context,
            IOrderRepository orders,
            IOrderDishRepository orderDishes,
            IOrderDishOptionRepository orderDishOptions,
            IPaymentRepository payments,
            IOrderStatusHistoryRepository statusHistory,
            IAnalyticsRepository analytics)
        {
            _context = context;
            Orders = orders;
            OrderDishes = orderDishes;
            OrderDishOptions = orderDishOptions;
            Payments = payments;
            StatusHistory = statusHistory;
            Analytics = analytics;
        }

        public void BeginTransaction()
        {
            _context.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _context.Transaction?.Commit();
            }
            catch
            {
                _context.Transaction?.Rollback();
                throw;
            }
            finally
            {
                _context.CloseConnection();
            }
        }

        public void Rollback()
        {
            _context.Transaction?.Rollback();
            _context.CloseConnection();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.CloseConnection();
                }
                _disposed = true;
            }
        }
    }
}
