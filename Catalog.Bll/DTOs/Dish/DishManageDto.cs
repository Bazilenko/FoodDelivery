using Catalog.Bll.DTOs.Category;
using Catalog.Bll.DTOs.ModifierGroup;

namespace Catalog.Bll.DTOs.Dish
{
    public record DishManageDto(
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
