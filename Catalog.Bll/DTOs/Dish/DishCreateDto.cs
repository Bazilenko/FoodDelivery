using Catalog.Bll.DTOs.ModifierGroup;

namespace Catalog.Bll.DTOs.Dish
{
    public class DishCreateDto
    {
        public int CategoryId {get; set;}
        public string Name {get; set;}
        public string? Description {get; set;}
        public decimal Price {get; set;}
        public decimal? Weight {get; set;}
        public string? Unit {get; set;}
        public decimal? Calories {get; set;}
        public string? ImageUrl {get; set;}
        public bool IsAvailable {get; set;} = true;
        public IEnumerable<ModifierGroupCreateDto>? ModifierGroups {get; set;} = null!;
}
}