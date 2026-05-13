using Catalog.Bll.DTOs.Address;
using Catalog.Bll.DTOs.Contact;
using Catalog.Bll.DTOs.Cuisine;
using Catalog.Bll.DTOs.WorkingHour;

namespace Catalog.Bll.DTOs.Restaurant
{
    public record RestaurantProfileDto(
        int Id,
        string Name,
        string? ImageUrl,
        string? Description,
        decimal Rating,
        decimal DeliveryRadiusKm,
        IEnumerable<CuisineDto> Cuisines,
        IEnumerable<AddressDto> Addresses,
        IEnumerable<ContactDto> Contacts,
        IEnumerable<WorkingHourDto> WorkingHours);
}