namespace Catalog.Bll.DTOs.Dish
{
    public record DishCardDto(
        int Id,
        string Name,
        string? ImageUrl,
        decimal Price,
        decimal? Calories,
        bool IsAvailable);
}
