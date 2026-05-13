using Catalog.Bll.DTOs.DishOption;

namespace Catalog.Bll.DTOs.ModifierGroup
{
    public record ModifierGroupUpdateDto(
        int Id,
        string Name,
        int MinSelect,
        int MaxSelect);
}