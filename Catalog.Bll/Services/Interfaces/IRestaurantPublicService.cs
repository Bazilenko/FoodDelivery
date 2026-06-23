using Catalog.Bll.DTOs.Dish;
using Catalog.Bll.DTOs.Restaurant;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IRestaurantPublicService
    {
        /// <summary>Public detail page (addresses, contacts, working hours, cuisines).</summary>
        Task<RestaurantDetailDto> GetDetailAsync(int restaurantId, CancellationToken ct = default);
 
        /// <summary>Full menu: categories each containing their available dishes.</summary>
        Task<IEnumerable<CategoryWithDishesDto>> GetMenuAsync(int restaurantId, CancellationToken ct = default);
        Task<IEnumerable<RestaurantCardDto>> GetByCuisineAsync(int cuisineId, CancellationToken ct);

    }
}
