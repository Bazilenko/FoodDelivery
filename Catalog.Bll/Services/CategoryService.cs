using AutoMapper;
using Catalog.Bll.DTOs.Category;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.Helpers;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Catalog.Bll.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var categories = await _uow.Categories.GetByRestaurantAsync(restaurantId, ct);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var entity = _mapper.Map<Category>(dto);
            entity.RestaurantId = restaurantId;

            await _uow.Categories.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto> UpdateAsync(CategoryUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(dto.Id, restaurantId, ct);

            _mapper.Map(dto, entity);

            await _uow.Categories.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            
            entity.IsDeleted = true;
            await _uow.Categories.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<Category> GetOwnedAsync(int id, int restaurantId, CancellationToken ct)
        {
            var entity = await _uow.Categories.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Category {id} not found.");

            if (entity.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Category does not belong to your restaurant.");

            return entity;
        }
    }
}