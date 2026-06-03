using Catalog.Bll.DTOs.Dish;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IRestaurantMenuService
{
    Task<IEnumerable<CategoryWithDishesDto>> GetRestaurantMenuAsync(
        int restaurantId,
        CancellationToken ct = default);

    Task<DishDetailDto> GetDishDetailAsync(
        int dishId,
        CancellationToken ct = default);
}
}