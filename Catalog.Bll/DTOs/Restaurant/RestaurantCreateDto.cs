using Catalog.Bll.DTOs.Contact;
using Catalog.Bll.DTOs.WorkingHour;
using Catalog.Bll.DTOs.Address;

namespace Catalog.Bll.DTOs.Restaurant
{
    public record RestaurantCreateDto(
        string Name,
        string? ImageUrl,
        string? Description,
        decimal DeliveryRadiusKm,
        // At least one address is required on creation
        IEnumerable<AddressCreateDto> Addresses,
        // Optional at creation but typical to supply
        IEnumerable<ContactCreateDto>? Contacts,
        // Cuisine ids to link on creation
        IEnumerable<int>? CuisineIds,
        // Full week schedule can be set upfront
        IEnumerable<WorkingHourCreateDto>? WorkingHours);
}
