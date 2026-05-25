namespace Catalog.Bll.DTOs.Dish
{
    public record DishUpdateDto(
        int Id,
        int CategoryId,
        string Name,
        string? Description,
        decimal Price,
        decimal? Weight,
        string? Unit,
        decimal? Calories,
        string? ImageUrl);
}
