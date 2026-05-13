using Catalog.Bll.DTOs.Cuisine;
using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.DTOs.Pagination;

namespace Catalog.Bll.Services.Interfaces
{
    public interface ICuisineService
    {
        /// <summary>All cuisines for the main menu filter bar.</summary>
        Task<IEnumerable<CuisineDto>> GetAllAsync(CancellationToken ct = default);
 
        /// <summary>Restaurants filtered by a cuisine with pagination.</summary>
        Task<PagedResult<RestaurantCardDto>> GetRestaurantsByCuisineAsync(int cuisineId, int page, int pageSize, CancellationToken ct = default);
    }
}