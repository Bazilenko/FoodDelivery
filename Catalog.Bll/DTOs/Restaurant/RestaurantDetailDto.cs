using Catalog.Bll.DTOs.Address;
using Catalog.Bll.DTOs.Contact;
using Catalog.Bll.DTOs.Cuisine;
using Catalog.Bll.DTOs.WorkingHour;

namespace Catalog.Bll.DTOs.Restaurant
{
    public record RestaurantDetailDto(  
        int Id,
        string Name,
        string? ImageUrl,
        string? Description,
        decimal Rating,
        decimal DeliveryRadiusKm,
        bool IsOpen,
        IEnumerable<CuisineDto> Cuisines,
        IEnumerable<AddressDto> Addresses,
        IEnumerable<ContactDto> Contacts,
        IEnumerable<WorkingHourDto> WorkingHours);
}