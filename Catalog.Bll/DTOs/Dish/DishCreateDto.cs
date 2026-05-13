using Catalog.Bll.DTOs.ModifierGroup;

namespace Catalog.Bll.DTOs.Dish
{
    public record DishCreateDto(
        int CategoryId,
        string Name,
        string? Description,
        decimal Price,
        decimal? Weight,
        string? Unit,
        decimal? Calories,
        string? ImageUrl,
        bool IsAvailable = true,
        IEnumerable<ModifierGroupCreateDto>? ModifierGroups = null);
}
