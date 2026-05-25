using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using AutoMapper;
using Catalog.Bll.DTOs.Dish;


namespace Catalog.Bll.Services
{
    public class DishPublicService : IDishPublicService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DishPublicService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<DishDetailDto> GetDetailAsync(int dishId, CancellationToken ct = default)
        {
            var dish = await _uow.Dishes.GetWithModifiersAsync(dishId, ct)
                ?? throw new KeyNotFoundException($"Dish {dishId} not found.");

            return _mapper.Map<DishDetailDto>(dish);
        }
    }
}