using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default);
        Task<Category?> GetWithDishesAsync(int categoryId, CancellationToken ct = default);

        Task<IEnumerable<Category>> GetMenuByRestaurantIdAsync(
            int restaurantId,
            CancellationToken ct = default);
    }
}
