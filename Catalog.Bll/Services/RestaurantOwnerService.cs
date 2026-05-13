using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using AutoMapper;

namespace Catalog.Bll.Services
{
    public class RestaurantOwnerService : IRestaurantOwnerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRestaurantContext _restaurantContext;

        public RestaurantOwnerService(IUnitOfWork uow, IMapper mapper, IRestaurantContext restaurantContext)
        {
            _uow = uow;
            _mapper = mapper;
            _restaurantContext = restaurantContext;
        }

        public async Task<RestaurantProfileDto> CreateAsync(RestaurantCreateDto dto, CancellationToken ct = default)
        {
            if (!dto.Addresses.Any())
                throw new ArgumentException("At least one address is required.");

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

            var created = await _uow.Restaurants.GetWithFullDetailsAsync(restaurant.Id, ct);
            return _mapper.Map<RestaurantProfileDto>(created);
        }

        public async Task<RestaurantProfileDto> GetProfileAsync(CancellationToken ct = default)
        {
            var restaurant = await _uow.Restaurants.GetWithFullDetailsAsync(_restaurantContext.RestaurantId, ct)
                ?? throw new KeyNotFoundException("Restaurant not found.");

            return _mapper.Map<RestaurantProfileDto>(restaurant);
        }

        public async Task<RestaurantProfileDto> UpdateProfileAsync(RestaurantUpdateDto dto, CancellationToken ct = default)
        {
            var restaurant = await _uow.Restaurants.GetByIdAsync(_restaurantContext.RestaurantId, ct)
                ?? throw new KeyNotFoundException("Restaurant not found.");

            _mapper.Map(dto, restaurant);

            await _uow.Restaurants.UpdateAsync(restaurant, ct);
            await _uow.SaveChangesAsync(ct);

            return await GetProfileAsync(ct);
        }

        public async Task UpdateCuisinesAsync(RestaurantCuisinesUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = _restaurantContext.RestaurantId;

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