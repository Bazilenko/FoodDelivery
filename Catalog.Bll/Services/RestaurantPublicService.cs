using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using AutoMapper;
using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.DTOs.Dish;
using Catalog.Bll.DTOs.Address;
using Catalog.Bll.DTOs.Category;
using Catalog.Dal.Entities;

namespace Catalog.Bll.Services
{

    public class RestaurantPublicService : IRestaurantPublicService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RestaurantPublicService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<RestaurantDetailDto> GetDetailAsync(int restaurantId, CancellationToken ct = default)
        {
            var restaurant = await _uow.Restaurants.GetWithFullDetailsAsync(restaurantId, ct)
                ?? throw new KeyNotFoundException($"Restaurant {restaurantId} not found.");

            return _mapper.Map<RestaurantDetailDto>(restaurant);
        }

        public async Task<IEnumerable<CategoryWithDishesDto>> GetMenuAsync(int restaurantId, CancellationToken ct = default)
        {
            var categories = await _uow.Categories.GetByRestaurantAsync(restaurantId, ct);

            var result = new List<CategoryWithDishesDto>();

            foreach (var category in categories)
            {
                var dishes = await _uow.Dishes.GetByCategoryAsync(category.Id, ct);
                var availableDishes = dishes.Where(d => d.IsAvailable);

                result.Add(new CategoryWithDishesDto
                {
                    Category = _mapper.Map<CategoryDto>(category),
                    Dishes = _mapper.Map<IEnumerable<DishCardDto>>(availableDishes)
                });
            }

            return result;
        }


        public async Task<IEnumerable<RestaurantCardDto>> GetByCuisineAsync(int cuisineId, CancellationToken ct)
        {
            var restaurants = await _uow.Restaurants.GetByCuisineAsync(cuisineId, ct);

            return restaurants.Select(r => new RestaurantCardDto(
                Id: r.Id,
                Name: r.Name,
                ImageUrl: r.ImageUrl,
                Rating: r.Rating,
                DeliveryRadiusKm: r.DeliveryRadiusKm,
                IsOpen: IsRestaurantOpen(r),
                Cuisines: r.RestaurantCuisines
                    .Select(rc => rc.Cuisine.Name)
                    .ToList(),
                PrimaryAddress: r.Addresses
            .Select(a => new AddressDto(
                a.Id,
                a.City,
                a.Street,
                a.BuildingNumber,
                a.PostalCode ?? null,
                a.Latitude,
                a.Longitude
    ))
    .FirstOrDefault()
            ));
        }
        private static bool IsRestaurantOpen(Restaurant restaurant)
        {
            var now = DateTime.Now;

            int currentDay = ConvertDayOfWeek(now.DayOfWeek);
            var currentTime = now.TimeOfDay;

            var todayHours = restaurant.WorkingHours
                .FirstOrDefault(w => w.DayOfWeek == currentDay);

            if (todayHours == null || todayHours.IsClosed)
                return false;

            return currentTime >= todayHours.OpeningTime &&
                   currentTime <= todayHours.ClosingTime;
        }

        private static int ConvertDayOfWeek(DayOfWeek day)
        {
            return day == DayOfWeek.Sunday
                ? 7
                : (int)day;
        }
    }
}