
using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IRestaurantCuisineRepository : IGenericRepository<RestaurantCuisine>
    {
        Task<bool> ExistsAsync(int restaurantId, int cuisineId, CancellationToken ct = default);
        Task<IEnumerable<RestaurantCuisine>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default);
    }
}
