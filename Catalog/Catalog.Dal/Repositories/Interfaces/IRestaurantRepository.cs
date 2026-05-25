using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IRestaurantRepository : IGenericRepository<Restaurant>
    {
        Task<Restaurant?> GetWithFullDetailsAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Restaurant>> GetByCuisineAsync(int cuisineId, CancellationToken ct = default);
        Task<IEnumerable<Restaurant>> GetByCityAsync(string city, CancellationToken ct = default);

        /// <summary>
        /// Cuisine-filtered restaurants paged and sorted entirely at DB level.
        /// Includes Addresses, WorkingHours, and RestaurantCuisines.Cuisine
        /// so the service can build RestaurantCardDto without extra queries.
        /// </summary>
        Task<(IEnumerable<Restaurant> Items, int TotalCount)> GetPagedByCuisineAsync(
            int cuisineId,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default);
    }
}
