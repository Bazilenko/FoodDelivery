using Orders.Dal.Repository.Interfaces;

namespace Orders.Dal.UoW.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        IOrderDishRepository OrderDishes { get; }
        IOrderDishOptionRepository OrderDishOptions { get; }
        IPaymentRepository Payments { get; }
        IOrderStatusHistoryRepository StatusHistory { get; }
        IAnalyticsRepository Analytics {get;} 

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
