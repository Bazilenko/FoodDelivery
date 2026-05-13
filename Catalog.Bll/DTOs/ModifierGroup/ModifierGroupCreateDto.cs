using Catalog.Bll.DTOs.DishOption;

namespace Catalog.Bll.DTOs.ModifierGroup
{
    public record ModifierGroupCreateDto(
        string Name,
        int MinSelect,
        int MaxSelect,
        IEnumerable<DishOptionCreateDto> Options);
}