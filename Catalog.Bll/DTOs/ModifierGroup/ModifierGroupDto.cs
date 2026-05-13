using Catalog.Bll.DTOs.DishOption;

namespace Catalog.Bll.DTOs.ModifierGroup
{
    public record ModifierGroupDto(
        int Id,
        string Name,
        int MinSelect,
        int MaxSelect,
        IEnumerable<DishOptionDto> Options);
}