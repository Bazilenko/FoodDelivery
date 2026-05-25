using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface ICuisineRepository : IGenericRepository<Cuisine>
    {
        Task<Cuisine?> GetByNameAsync(string name, CancellationToken ct = default);
         Task<IEnumerable<Cuisine>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default);
    }
}
