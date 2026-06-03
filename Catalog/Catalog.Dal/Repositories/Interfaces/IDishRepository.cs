using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IDishRepository : IGenericRepository<Dish>
    {
        Task<Dish?> GetWithModifiersAsync(int dishId, CancellationToken ct = default);
        Task<IEnumerable<Dish>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);
        Task<IEnumerable<Dish>> GetByRestaurantAsync(int restaurantId, bool availableOnly = false, CancellationToken ct = default);
        Task<Dish?> GetDetailAsync(int dishId, CancellationToken ct = default);
    }
}
