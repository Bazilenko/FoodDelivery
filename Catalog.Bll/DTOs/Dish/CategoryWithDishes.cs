using Catalog.Bll.DTOs.Category;

namespace Catalog.Bll.DTOs.Dish
{
     public record CategoryWithDishesDto(CategoryDto Category, IEnumerable<DishCardDto> Dishes);
}