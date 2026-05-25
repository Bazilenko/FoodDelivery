using Catalog.Bll.DTOs.Category;
using Catalog.Bll.DTOs.ModifierGroup;

namespace Catalog.Bll.DTOs.Dish
{
    public record DishManageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? Weight { get; set; }
        public string? Unit { get; set; }
        public decimal? Calories { get; set; }
        public bool IsAvailable { get; set; }
        public CategoryDto Category { get; set; } = null!;
        public IEnumerable<ModifierGroupDto> ModifierGroups { get; set; } = null!;

    }
}
