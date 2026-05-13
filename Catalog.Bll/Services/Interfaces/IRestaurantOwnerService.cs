using Catalog.Bll.DTOs.Restaurant;

namespace Catalog.Bll.Services.Interfaces
{
    
    public interface IRestaurantOwnerService
    {
        /// <summary>Register a new restaurant with address, contacts, cuisines, and schedule in one call.</summary>
        Task<RestaurantProfileDto> CreateAsync(RestaurantCreateDto dto, CancellationToken ct = default);
 
        /// <summary>Owner's full profile view.</summary>
        Task<RestaurantProfileDto> GetProfileAsync(CancellationToken ct = default);
 
        /// <summary>Update name, description, image, delivery radius.</summary>
        Task<RestaurantProfileDto> UpdateProfileAsync(RestaurantUpdateDto dto, CancellationToken ct = default);
 
        /// <summary>Replace the restaurant's cuisine list with the supplied ids.</summary>
        Task UpdateCuisinesAsync(RestaurantCuisinesUpdateDto dto, CancellationToken ct = default);
    }
}