namespace Catalog.Bll.DTOs.DishOption
{
    public record DishOptionCreateDto(string Name, decimal Price, bool IsAvailable = true);
}