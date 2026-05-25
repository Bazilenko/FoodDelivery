using Catalog.Bll.DTOs.DishOption;

namespace Catalog.Bll.DTOs.ModifierGroup
{
    public class ModifierGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinSelect { get; set; }
        public int MaxSelect { get; set; }
        public IEnumerable<DishOptionDto> Options { get; set; } = [];
    }
}
