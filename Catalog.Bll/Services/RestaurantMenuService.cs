using Catalog.Bll.DTOs.Category;
using Catalog.Bll.DTOs.Dish;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.DTOs.ModifierGroup;
using Catalog.Bll.DTOs.DishOption;

namespace Catalog.Bll.Services;

public class RestaurantMenuService : IRestaurantMenuService
{
    private readonly IUnitOfWork _unitOfWork;

    public RestaurantMenuService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryWithDishesDto>> GetRestaurantMenuAsync(
        int restaurantId,
        CancellationToken ct = default)
    {
        var categories = await _unitOfWork.Categories
            .GetMenuByRestaurantIdAsync(restaurantId, ct);

        return categories.Select(c => new CategoryWithDishesDto
        {
            Category = new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            },

            Dishes = c.Dishes.Select(d => new DishCardDto
            {
                Id = d.Id,
                Name = d.Name,
                ImageUrl = d.ImageUrl,
                Price = d.Price,
                Calories = d.Calories,
                IsAvailable = d.IsAvailable
            }).ToList()
        }).ToList();
    }

    public async Task<DishDetailDto> GetDishDetailAsync(
        int dishId,
        CancellationToken ct = default)
    {
        var dish = await _unitOfWork.Dishes.GetDetailAsync(dishId, ct);

        if (dish == null)
            throw new KeyNotFoundException("Dish not found.");

        return new DishDetailDto
        {
            Id = dish.Id,
            Name = dish.Name,
            ImageUrl = dish.ImageUrl,
            Description = dish.Description,
            Price = dish.Price,
            Weight = dish.Weight,
            Unit = dish.Unit,
            Calories = dish.Calories,
            IsAvailable = dish.IsAvailable,

            Category = new CategoryDto
            {
                Id = dish.Category.Id,
                Name = dish.Category.Name
            },

            ModifierGroups = dish.ModifierGroups.Select(g => new ModifierGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                MinSelect = g.MinSelect,
                MaxSelect = g.MaxSelect,

                Options = g.DishOptions.Select(o => new DishOptionDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Price = o.Price,
                    IsAvailable = o.IsAvailable
                }).ToList()
            }).ToList()
        };
    }
}