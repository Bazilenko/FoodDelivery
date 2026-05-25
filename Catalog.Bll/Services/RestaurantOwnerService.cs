using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.Helpers; 
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Catalog.Bll.Services
{
    public class RestaurantOwnerService : IRestaurantOwnerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IIdentityClient _identityClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RestaurantOwnerService(
            IUnitOfWork uow, 
            IMapper mapper, 
            IIdentityClient identityClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _identityClient = identityClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RestaurantProfileDto> CreateAsync(RestaurantCreateDto dto, CancellationToken ct = default)
        {
            if (!dto.Addresses.Any())
                throw new ArgumentException("At least one address is required.");

            var currentUserId = AuthHelper.GetUserIdFromToken(_httpContextAccessor);

            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.Rating = 0;

            await _uow.Restaurants.AddAsync(restaurant, ct);

            if (dto.CuisineIds is not null)
            {
                var cuisineLinks = dto.CuisineIds.Select(cuisineId => new RestaurantCuisine
                {
                    Restaurant = restaurant,
                    CuisineId = cuisineId
                });
                await _uow.RestaurantCuisines.AddRangeAsync(cuisineLinks, ct);
            }

            await _uow.SaveChangesAsync(ct);

            await _identityClient.LinkUserToRestaurantAsync(currentUserId, restaurant.Id, ct);

            var created = await _uow.Restaurants.GetWithFullDetailsAsync(restaurant.Id, ct);
            return _mapper.Map<RestaurantProfileDto>(created);
        }

        public async Task<RestaurantProfileDto> GetProfileAsync(CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var restaurant = await _uow.Restaurants.GetWithFullDetailsAsync(restaurantId, ct)
                ?? throw new KeyNotFoundException("Restaurant not found.");

            return _mapper.Map<RestaurantProfileDto>(restaurant);
        }

        public async Task<RestaurantProfileDto> UpdateProfileAsync(RestaurantUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var restaurant = await _uow.Restaurants.GetByIdAsync(restaurantId, ct)
                ?? throw new KeyNotFoundException("Restaurant not found.");

            _mapper.Map(dto, restaurant);

            await _uow.Restaurants.UpdateAsync(restaurant, ct);
            await _uow.SaveChangesAsync(ct);

            return await GetProfileAsync(ct);
        }

        public async Task UpdateCuisinesAsync(RestaurantCuisinesUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var existing = await _uow.RestaurantCuisines.GetByRestaurantAsync(restaurantId, ct);
            foreach (var link in existing)
                await _uow.RestaurantCuisines.DeleteAsync(link, ct);

            var newLinks = dto.CuisineIds.Select(cuisineId => new RestaurantCuisine
            {
                RestaurantId = restaurantId,
                CuisineId = cuisineId
            });

            await _uow.RestaurantCuisines.AddRangeAsync(newLinks, ct);
            await _uow.SaveChangesAsync(ct);
        }
    }
}