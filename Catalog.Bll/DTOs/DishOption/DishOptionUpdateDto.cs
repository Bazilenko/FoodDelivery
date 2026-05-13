namespace Catalog.Bll.DTOs.DishOption
{
    public record DishOptionUpdateDto(int Id, string Name, decimal Price, bool IsAvailable);
}