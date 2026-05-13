namespace Catalog.Bll.DTOs.Restaurant
{
    public record RestaurantUpdateDto(
        string Name,
        string? ImageUrl,
        string? Description,
        decimal DeliveryRadiusKm);
}
