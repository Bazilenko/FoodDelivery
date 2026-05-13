using Catalog.Bll.DTOs.DishOption;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using AutoMapper;

namespace Catalog.Bll.Services
{
    public class DishOptionService : IDishOptionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRestaurantContext _restaurantContext;

        public DishOptionService(IUnitOfWork uow, IMapper mapper, IRestaurantContext restaurantContext)
        {
            _uow = uow;
            _mapper = mapper;
            _restaurantContext = restaurantContext;
        }

        public async Task<IEnumerable<DishOptionDto>> GetByModifierGroupAsync(int modifierGroupId, CancellationToken ct = default)
        {
            await AssertModifierGroupOwnershipAsync(modifierGroupId, ct);
            var options = await _uow.DishOptions.GetByModifierGroupAsync(modifierGroupId, ct);
            return _mapper.Map<IEnumerable<DishOptionDto>>(options);
        }

        public async Task<DishOptionDto> GetByIdAsync(int id, CancellationToken ct = default)
            => _mapper.Map<DishOptionDto>(await GetOwnedAsync(id, ct));

        public async Task<DishOptionDto> CreateAsync(int modifierGroupId, DishOptionCreateDto dto, CancellationToken ct = default)
        {
            await AssertModifierGroupOwnershipAsync(modifierGroupId, ct);

            var entity = _mapper.Map<DishOption>(dto);
            entity.ModifierGroupId = modifierGroupId;

            await _uow.DishOptions.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<DishOptionDto>(entity);
        }

        public async Task<DishOptionDto> UpdateAsync(DishOptionUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(dto.Id, ct);

            _mapper.Map(dto, entity);

            await _uow.DishOptions.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<DishOptionDto>(entity);
        }

        public async Task SetAvailabilityAsync(int id, bool isAvailable, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(id, ct);
            entity.IsAvailable = isAvailable;
            await _uow.DishOptions.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(id, ct);
            entity.IsDeleted = true;
            await _uow.DishOptions.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task AssertModifierGroupOwnershipAsync(int modifierGroupId, CancellationToken ct)
        {
            var mg = await _uow.ModifierGroups.GetByIdAsync(modifierGroupId, ct)
                ?? throw new KeyNotFoundException($"ModifierGroup {modifierGroupId} not found.");

            var dish = await _uow.Dishes.GetByIdAsync(mg.DishId, ct);

            if (dish?.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Modifier group does not belong to your restaurant.");
        }

        private async Task<DishOption> GetOwnedAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.DishOptions.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"DishOption {id} not found.");

            var mg = await _uow.ModifierGroups.GetByIdAsync(entity.ModifierGroupId, ct);
            var dish = mg is not null ? await _uow.Dishes.GetByIdAsync(mg.DishId, ct) : null;

            if (dish?.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Dish option does not belong to your restaurant.");

            return entity;
        }
    }
}