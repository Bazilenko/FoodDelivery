using Catalog.Bll.DTOs.ModifierGroup;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.Helpers;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Catalog.Bll.Services
{
    public class ModifierGroupService : IModifierGroupService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModifierGroupService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ModifierGroupDto>> GetByDishAsync(int dishId, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            await AssertDishOwnershipAsync(dishId, restaurantId, ct);
            
            var groups = await _uow.ModifierGroups.GetByDishAsync(dishId, ct);
            return _mapper.Map<IEnumerable<ModifierGroupDto>>(groups);
        }

        public async Task<ModifierGroupDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            return _mapper.Map<ModifierGroupDto>(entity);
        }

        public async Task<ModifierGroupDto> CreateAsync(int dishId, ModifierGroupCreateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            await AssertDishOwnershipAsync(dishId, restaurantId, ct);

            var entity = _mapper.Map<ModifierGroup>(dto);
            entity.DishId = dishId;
            entity.DishOptions = _mapper.Map<List<DishOption>>(dto.Options);

            await _uow.ModifierGroups.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var created = await _uow.ModifierGroups.GetWithOptionsAsync(entity.Id, ct);
            return _mapper.Map<ModifierGroupDto>(created);
        }

        public async Task<ModifierGroupDto> UpdateAsync(ModifierGroupUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(dto.Id, restaurantId, ct);

            _mapper.Map(dto, entity);

            await _uow.ModifierGroups.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var updated = await _uow.ModifierGroups.GetWithOptionsAsync(entity.Id, ct);
            return _mapper.Map<ModifierGroupDto>(updated);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            
            entity.IsDeleted = true;
            await _uow.ModifierGroups.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task AssertDishOwnershipAsync(int dishId, int restaurantId, CancellationToken ct)
        {
            var dish = await _uow.Dishes.GetByIdAsync(dishId, ct)
                ?? throw new KeyNotFoundException($"Dish {dishId} not found.");

            if (dish.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Dish does not belong to your restaurant.");
        }

        private async Task<ModifierGroup> GetOwnedAsync(int id, int restaurantId, CancellationToken ct)
        {
            var entity = await _uow.ModifierGroups.GetWithOptionsAsync(id, ct)
                ?? throw new KeyNotFoundException($"ModifierGroup {id} not found.");

            var dish = await _uow.Dishes.GetByIdAsync(entity.DishId, ct);

            if (dish?.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Modifier group does not belong to your restaurant.");

            return entity;
        }
    }
}