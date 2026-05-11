using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IRestaurantRepository : IGenericRepository<Restaurant>
    {
        Task<Restaurant?> GetWithFullDetailsAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Restaurant>> GetByCuisineAsync(int cuisineId, CancellationToken ct = default);
        Task<IEnumerable<Restaurant>> GetByCityAsync(string city, CancellationToken ct = default);
    }
}
