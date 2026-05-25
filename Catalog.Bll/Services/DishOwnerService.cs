using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.Helpers; 
using AutoMapper;
using Catalog.Bll.DTOs.Dish;
using Catalog.Dal.Entities;
using Microsoft.AspNetCore.Http; 

namespace Catalog.Bll.Services
{
    public class DishOwnerService : IDishOwnerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public DishOwnerService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<DishManageDto>> GetAllAsync(CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            
            var dishes = await _uow.Dishes.GetByRestaurantAsync(restaurantId, ct: ct);
            return _mapper.Map<IEnumerable<DishManageDto>>(dishes);
        }

        public async Task<DishManageDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            return _mapper.Map<DishManageDto>(entity);
        }

        public async Task<DishManageDto> CreateAsync(DishCreateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            await AssertCategoryOwnershipAsync(dto.CategoryId, restaurantId, ct);

            var entity = _mapper.Map<Dish>(dto);
            entity.RestaurantId = restaurantId;

            await _uow.Dishes.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var created = await _uow.Dishes.GetWithModifiersAsync(entity.Id, ct);
            return _mapper.Map<DishManageDto>(created);
        }

        public async Task<DishManageDto> UpdateAsync(DishUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(dto.Id, restaurantId, ct);

            if (entity.CategoryId != dto.CategoryId)
                await AssertCategoryOwnershipAsync(dto.CategoryId, restaurantId, ct);

            _mapper.Map(dto, entity);

            await _uow.Dishes.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var updated = await _uow.Dishes.GetWithModifiersAsync(entity.Id, ct);
            return _mapper.Map<DishManageDto>(updated);
        }

        public async Task SetAvailabilityAsync(DishAvailabilityDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(dto.Id, restaurantId, ct);
            
            entity.IsAvailable = dto.IsAvailable;
            await _uow.Dishes.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            
            entity.IsDeleted = true;
            await _uow.Dishes.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }


        private async Task<Dish> GetOwnedAsync(int id, int restaurantId, CancellationToken ct)
        {
            var entity = await _uow.Dishes.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Dish {id} not found.");

            if (entity.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Dish does not belong to your restaurant.");

            return entity;
        }

        private async Task AssertCategoryOwnershipAsync(int categoryId, int restaurantId, CancellationToken ct)
        {
            var category = await _uow.Categories.GetByIdAsync(categoryId, ct)
                ?? throw new KeyNotFoundException($"Category {categoryId} not found.");

            if (category.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Category does not belong to your restaurant.");
        }
    }
}