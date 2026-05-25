using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using AutoMapper;
using Catalog.Bll.DTOs.Cuisine;
using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.DTOs.Pagination;
using Catalog.Bll.DTOs.Address;
using Catalog.Dal.Entities;

namespace Catalog.Bll.Services
{
    public class CuisineService : ICuisineService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CuisineService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CuisineDto>> GetAllAsync(CancellationToken ct = default)
        {
            var cuisines = await _uow.Cuisines.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<CuisineDto>>(cuisines);
        }

        public async Task<PagedResult<RestaurantCardDto>> GetRestaurantsByCuisineAsync(
            int cuisineId, int page, int pageSize, CancellationToken ct = default)
        {
            var (items, total) = await _uow.Restaurants.GetPagedByCuisineAsync(
                cuisineId, page, pageSize, ct);

            var now = DateTime.UtcNow;
            var dtos = items.Select(r => new RestaurantCardDto(
                r.Id,
                r.Name,
                r.ImageUrl,
                r.Rating,
                r.DeliveryRadiusKm,
                IsOpenNow(r.WorkingHours, now),
                r.RestaurantCuisines.Select(rc => rc.Cuisine.Name),
                _mapper.Map<AddressDto>(r.Addresses.FirstOrDefault())));

            return new PagedResult<RestaurantCardDto>(dtos, total, page, pageSize);
        }

        public async Task<CuisineDto> CreateAsync(CuisineCreateDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Cuisine>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _uow.Cuisines.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CuisineDto>(entity);
        }

        public async Task<CuisineDto> UpdateAsync(CuisineUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _uow.Cuisines.GetByIdAsync(dto.Id, ct)
                ?? throw new KeyNotFoundException($"Cuisine {dto.Id} not found.");
            entity.Name = dto.Name;
            entity.ImageUrl = dto.ImageUrl;
            entity.UpdatedAt = DateTime.UtcNow;
            await _uow.Cuisines.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<CuisineDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _uow.Cuisines.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Cuisine {id} not found.");
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await _uow.Cuisines.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private static bool IsOpenNow(IEnumerable<WorkingHour> workingHours, DateTime utcNow)
        {
            var today = workingHours.FirstOrDefault(w => w.DayOfWeek == (int)utcNow.DayOfWeek);
            if (today is null || today.IsClosed) return false;

            var currentTime = TimeOnly.FromDateTime(utcNow);
            var open = TimeOnly.FromTimeSpan(today.OpeningTime);
            var close = TimeOnly.FromTimeSpan(today.ClosingTime);

            return open <= close
                ? currentTime >= open && currentTime <= close
                : currentTime >= open || currentTime <= close;
        }
    }
}