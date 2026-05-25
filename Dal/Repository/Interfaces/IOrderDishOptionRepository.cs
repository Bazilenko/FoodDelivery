using Orders.Dal.Entities;

namespace Orders.Dal.Repository.Interfaces
{
    public interface IOrderDishOptionRepository : IGenericRepository<OrderDishOption>
    {
        Task<IEnumerable<OrderDishOption>> GetByOrderDishIdAsync(int orderDishId);

    }
}
