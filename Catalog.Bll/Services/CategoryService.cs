using AutoMapper;
using Catalog.Bll.DTOs.Category;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;

namespace Catalog.Bll.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRestaurantContext _restaurantContext;

        public CategoryService(IUnitOfWork uow, IMapper mapper, IRestaurantContext restaurantContext)
        {
            _uow = uow;
            _mapper = mapper;
            _restaurantContext = restaurantContext;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        {
            var categories = await _uow.Categories.GetByRestaurantAsync(_restaurantContext.RestaurantId, ct);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken ct = default)
            => _mapper.Map<CategoryDto>(await GetOwnedAsync(id, ct));

        public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Category>(dto);
            entity.RestaurantId = _restaurantContext.RestaurantId;

            await _uow.Categories.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto> UpdateAsync(CategoryUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(dto.Id, ct);

            _mapper.Map(dto, entity);

            await _uow.Categories.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(id, ct);
            entity.IsDeleted = true;
            await _uow.Categories.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<Category> GetOwnedAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.Categories.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Category {id} not found.");

            if (entity.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Category does not belong to your restaurant.");

            return entity;
        }
    }
}
