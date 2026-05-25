using Catalog.Bll.DTOs.Address;
using Catalog.Bll.DTOs.Contact;
using Catalog.Bll.DTOs.Cuisine;
using Catalog.Bll.DTOs.WorkingHour;

namespace Catalog.Bll.DTOs.Restaurant
{
    public class RestaurantProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public decimal Rating { get; set; }
        public decimal DeliveryRadiusKm { get; set; }
        public IEnumerable<CuisineDto> Cuisines { get; set; } = [];
        public IEnumerable<AddressDto> Addresses { get; set; } = [];
        public IEnumerable<ContactDto> Contacts { get; set; } = [];
        public IEnumerable<WorkingHourDto> WorkingHours { get; set; } = [];
    }
}