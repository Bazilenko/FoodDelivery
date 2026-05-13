using Catalog.Bll.DTOs.ModifierGroup;
using Catalog.Bll.DTOs.Category;

namespace Catalog.Bll.DTOs.Dish
{
    
    public record DishDetailDto(
        int Id,
        string Name,
        string? ImageUrl,
        string? Description,
        decimal Price,
        decimal? Weight,
        string? Unit,
        decimal? Calories,
        bool IsAvailable,
        CategoryDto Category,
        IEnumerable<ModifierGroupDto> ModifierGroups);
}