using Catalog.Dal.UOW.Interfaces;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.DTOs.WorkingHour;
using Catalog.Dal.Entities;
using AutoMapper;


namespace Catalog.Bll.Services
{
    public class WorkingHoursService : IWorkingHourService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRestaurantContext _restaurantContext;

        public WorkingHoursService(IUnitOfWork uow, IMapper mapper, IRestaurantContext restaurantContext)
        {
            _uow = uow;
            _mapper = mapper;
            _restaurantContext = restaurantContext;
        }

        public async Task<IEnumerable<WorkingHourDto>> GetWeekScheduleAsync(CancellationToken ct = default)
        {
            var hours = await _uow.WorkingHours.GetByRestaurantAsync(_restaurantContext.RestaurantId, ct);
            return _mapper.Map<IEnumerable<WorkingHourDto>>(hours);
        }

        public async Task<WorkingHourDto> GetByDayAsync(int dayOfWeek, CancellationToken ct = default)
        {
            var entity = await _uow.WorkingHours.GetByDayAsync(_restaurantContext.RestaurantId, dayOfWeek, ct)
                ?? throw new KeyNotFoundException($"No schedule found for day {dayOfWeek}.");

            return _mapper.Map<WorkingHourDto>(entity);
        }

        public async Task<WorkingHourDto> CreateAsync(WorkingHourCreateDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<WorkingHour>(dto);
            entity.RestaurantId = _restaurantContext.RestaurantId;

            await _uow.WorkingHours.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<WorkingHourDto>(entity);
        }

        public async Task<WorkingHourDto> UpdateAsync(WorkingHourUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(dto.Id, ct);

            _mapper.Map(dto, entity);

            await _uow.WorkingHours.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<WorkingHourDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(id, ct);
            entity.IsDeleted = true;
            await _uow.WorkingHours.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<WorkingHour> GetOwnedAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.WorkingHours.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"WorkingHours {id} not found.");

            if (entity.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Working hours do not belong to your restaurant.");

            return entity;
        }
    }
}