using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using AutoMapper;
using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.DTOs.Dish;
using Catalog.Bll.DTOs.Category;

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

                result.Add(new CategoryWithDishesDto(
                    _mapper.Map<CategoryDto>(category),
                    _mapper.Map<IEnumerable<DishCardDto>>(availableDishes)));
            }

            return result;
        }
    }
}