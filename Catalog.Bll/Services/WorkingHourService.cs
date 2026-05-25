using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.DTOs.WorkingHour;
using Catalog.Bll.Helpers;
using Catalog.Dal.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Catalog.Bll.Services
{
    public class WorkingHourService : IWorkingHourService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkingHourService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<WorkingHourDto>> GetWeekScheduleAsync(CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var hours = await _uow.WorkingHours.GetByRestaurantAsync(restaurantId, ct);
            return _mapper.Map<IEnumerable<WorkingHourDto>>(hours);
        }

        public async Task<WorkingHourDto> GetByDayAsync(int dayOfWeek, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var entity = await _uow.WorkingHours.GetByDayAsync(restaurantId, dayOfWeek, ct)
                ?? throw new KeyNotFoundException($"No schedule found for day {dayOfWeek}.");

            return _mapper.Map<WorkingHourDto>(entity);
        }

        public async Task<WorkingHourDto> CreateAsync(WorkingHourCreateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var entity = _mapper.Map<WorkingHour>(dto);
            entity.RestaurantId = restaurantId;

            await _uow.WorkingHours.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<WorkingHourDto>(entity);
        }

        public async Task<List<WorkingHourDto>> UpdateAsync(WorkingHoursUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var results = new List<WorkingHour>();

            foreach (var hourDto in dto.WorkingHours)
            {
                if (hourDto.Id.HasValue && hourDto.Id.Value > 0)
                {
                    var existing = await _uow.WorkingHours.GetByIdAsync(hourDto.Id.Value);
                    if (existing != null && existing.RestaurantId == restaurantId)
                    {
                        _mapper.Map(hourDto, existing);
                        await _uow.WorkingHours.UpdateAsync(existing);
                        results.Add(existing);
                    }
                }
                else
                {
                    var newHour = _mapper.Map<WorkingHour>(hourDto);
                    newHour.RestaurantId = restaurantId;
                    await _uow.WorkingHours.AddAsync(newHour);
                    results.Add(newHour);
                }
            }

            await _uow.SaveChangesAsync();
            return _mapper.Map<List<WorkingHourDto>>(results);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);

            entity.IsDeleted = true;
            await _uow.WorkingHours.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }


        private async Task<WorkingHour> GetOwnedAsync(int id, int restaurantId, CancellationToken ct)
        {
            var entity = await _uow.WorkingHours.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"WorkingHours {id} not found.");

            if (entity.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Working hours do not belong to your restaurant.");

            return entity;
        }
    }
}