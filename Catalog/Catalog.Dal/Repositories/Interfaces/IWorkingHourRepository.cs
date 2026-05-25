using Catalog.Dal.Entities;
namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IWorkingHourRepository : IGenericRepository<WorkingHour>
    {
        Task<IEnumerable<WorkingHour>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default);
        Task<WorkingHour?> GetByDayAsync(int restaurantId, int dayOfWeek, CancellationToken ct = default);
    }
}