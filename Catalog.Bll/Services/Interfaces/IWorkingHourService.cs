using Catalog.Bll.DTOs.WorkingHour;

namespace Catalog.Bll.Services.Interfaces
{
    
    public interface IWorkingHourService
    {
        /// <summary>Full week schedule for the restaurant.</summary>
        Task<IEnumerable<WorkingHourDto>> GetWeekScheduleAsync(CancellationToken ct = default);
        Task<WorkingHourDto> GetByDayAsync(int dayOfWeek, CancellationToken ct = default);
        Task<WorkingHourDto> CreateAsync(WorkingHourCreateDto dto, CancellationToken ct = default);
        Task<WorkingHourDto> UpdateAsync(WorkingHourUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}