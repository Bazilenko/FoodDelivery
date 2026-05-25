using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Enums;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class WorkingHourRepository : GenericRepository<WorkingHour>, IWorkingHourRepository
    {
        public WorkingHourRepository(MyDbContext dbContext) : base(dbContext){}
        public async Task<IEnumerable<WorkingHour>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(w => w.RestaurantId == restaurantId)
                .OrderBy(w => w.DayOfWeek)
                .ToListAsync(ct);
        }

        public async Task<WorkingHour?> GetByDayAsync(int restaurantId, int dayOfWeek, CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(w => w.RestaurantId == restaurantId && w.DayOfWeek == dayOfWeek, ct);
            
        }

    }
}
