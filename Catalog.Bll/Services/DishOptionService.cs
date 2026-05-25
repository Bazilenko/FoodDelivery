using Catalog.Bll.DTOs.DishOption;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.Helpers; 
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http; 

namespace Catalog.Bll.Services
{
    public class DishOptionService : IDishOptionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public DishOptionService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<DishOptionDto>> GetByModifierGroupAsync(int modifierGroupId, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            await AssertModifierGroupOwnershipAsync(modifierGroupId, restaurantId, ct);
            
            var options = await _uow.DishOptions.GetByModifierGroupAsync(modifierGroupId, ct);
            return _mapper.Map<IEnumerable<DishOptionDto>>(options);
        }

        public async Task<DishOptionDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            return _mapper.Map<DishOptionDto>(entity);
        }

        public async Task<DishOptionDto> CreateAsync(int modifierGroupId, DishOptionCreateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            await AssertModifierGroupOwnershipAsync(modifierGroupId, restaurantId, ct);

            var entity = _mapper.Map<DishOption>(dto);
            entity.ModifierGroupId = modifierGroupId;

            await _uow.DishOptions.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<DishOptionDto>(entity);
        }

        public async Task<DishOptionDto> UpdateAsync(DishOptionUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(dto.Id, restaurantId, ct);

            _mapper.Map(dto, entity);

            await _uow.DishOptions.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<DishOptionDto>(entity);
        }

        public async Task SetAvailabilityAsync(int id, bool isAvailable, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            
            entity.IsAvailable = isAvailable;
            await _uow.DishOptions.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            
            entity.IsDeleted = true;
            await _uow.DishOptions.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }


        private async Task AssertModifierGroupOwnershipAsync(int modifierGroupId, int restaurantId, CancellationToken ct)
        {
            var mg = await _uow.ModifierGroups.GetByIdAsync(modifierGroupId, ct)
                ?? throw new KeyNotFoundException($"ModifierGroup {modifierGroupId} not found.");

            var dish = await _uow.Dishes.GetByIdAsync(mg.DishId, ct);

            if (dish?.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Modifier group does not belong to your restaurant.");
        }

        private async Task<DishOption> GetOwnedAsync(int id, int restaurantId, CancellationToken ct)
        {
            var entity = await _uow.DishOptions.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"DishOption {id} not found.");

            var mg = await _uow.ModifierGroups.GetByIdAsync(entity.ModifierGroupId, ct);
            var dish = mg is not null ? await _uow.Dishes.GetByIdAsync(mg.DishId, ct) : null;

            if (dish?.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Dish option does not belong to your restaurant.");

            return entity;
        }
    }
}