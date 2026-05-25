using Catalog.Bll.DTOs.DishOption;

namespace Catalog.Bll.DTOs.ModifierGroup
{
    public record ModifierGroupCreateDto
    {
        public string Name { get; set; }
        public int MinSelect { get; set; }
        public int MaxSelect { get; set; }
        public IEnumerable<DishOptionCreateDto> Options { get; set; } = null!;
    }
}