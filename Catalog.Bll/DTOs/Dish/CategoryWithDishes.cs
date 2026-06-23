using Catalog.Bll.DTOs.Category;

namespace Catalog.Bll.DTOs.Dish
{
     public class CategoryWithDishesDto
     {
          public CategoryDto Category { get; set; } = null!;

          public IEnumerable<DishCardDto> Dishes { get; set; } = [];
     }
}