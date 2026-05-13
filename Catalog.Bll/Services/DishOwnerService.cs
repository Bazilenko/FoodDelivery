using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using AutoMapper;
using Catalog.Bll.DTOs.Dish;
using Catalog.Dal.Entities;

namespace Catalog.Bll.Services
{
    public class DishOwnerService : IDishOwnerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRestaurantContext _restaurantContext;

        public DishOwnerService(IUnitOfWork uow, IMapper mapper, IRestaurantContext restaurantContext)
        {
            _uow = uow;
            _mapper = mapper;
            _restaurantContext = restaurantContext;
        }

        public async Task<IEnumerable<DishManageDto>> GetAllAsync(CancellationToken ct = default)
        {
            var dishes = await _uow.Dishes.GetByRestaurantAsync(_restaurantContext.RestaurantId, ct: ct);
            return _mapper.Map<IEnumerable<DishManageDto>>(dishes);
        }

        public async Task<DishManageDto> GetByIdAsync(int id, CancellationToken ct = default)
            => _mapper.Map<DishManageDto>(await GetOwnedAsync(id, ct));

        public async Task<DishManageDto> CreateAsync(DishCreateDto dto, CancellationToken ct = default)
        {
            await AssertCategoryOwnershipAsync(dto.CategoryId, ct);

            var entity = _mapper.Map<Dish>(dto);
            entity.RestaurantId = _restaurantContext.RestaurantId;

            if (dto.ModifierGroups is not null)
                entity.ModifierGroups = _mapper.Map<List<ModifierGroup>>(dto.ModifierGroups);

            await _uow.Dishes.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var created = await _uow.Dishes.GetWithModifiersAsync(entity.Id, ct);
            return _mapper.Map<DishManageDto>(created);
        }

        public async Task<DishManageDto> UpdateAsync(DishUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(dto.Id, ct);

            if (entity.CategoryId != dto.CategoryId)
                await AssertCategoryOwnershipAsync(dto.CategoryId, ct);

            _mapper.Map(dto, entity);

            await _uow.Dishes.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var updated = await _uow.Dishes.GetWithModifiersAsync(entity.Id, ct);
            return _mapper.Map<DishManageDto>(updated);
        }

        public async Task SetAvailabilityAsync(DishAvailabilityDto dto, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(dto.Id, ct);
            entity.IsAvailable = dto.IsAvailable;
            await _uow.Dishes.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(id, ct);
            entity.IsDeleted = true;
            await _uow.Dishes.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<Dish> GetOwnedAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.Dishes.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Dish {id} not found.");

            if (entity.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Dish does not belong to your restaurant.");

            return entity;
        }

        private async Task AssertCategoryOwnershipAsync(int categoryId, CancellationToken ct)
        {
            var category = await _uow.Categories.GetByIdAsync(categoryId, ct)
                ?? throw new KeyNotFoundException($"Category {categoryId} not found.");

            if (category.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Category does not belong to your restaurant.");
        }
    }
}