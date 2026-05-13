using Catalog.Bll.DTOs.Address;

namespace Catalog.Bll.DTOs.Restaurant
{
    public record RestaurantCardDto(
        int Id,
        string Name,
        string? ImageUrl,
        decimal Rating,
        decimal DeliveryRadiusKm,
        IEnumerable<string> Cuisines,
        AddressDto? PrimaryAddress);
}
