using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Dal.Repositories
{
    public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
    {

        public RestaurantRepository(MyDbContext dbContext) : base(dbContext) { }

        public async Task<Restaurant?> GetWithFullDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(r => r.Addresses)
                .Include(r => r.Contacts)
                .Include(r => r.WorkingHours)
                .Include(r => r.Categories)
                .Include(r => r.RestaurantCuisines)
                    .ThenInclude(rc => rc.Cuisine)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
        }

        public async Task<IEnumerable<Restaurant>> GetByCityAsync(string city, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.Addresses.Any(a => a.City == city))
                .Include(r => r.Addresses)
                .ToListAsync(ct);
        }
        public async Task<IEnumerable<Restaurant>> GetByCuisineAsync(int cuisineId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(r => r.RestaurantCuisines.Any(rc => rc.CuisineId == cuisineId))
                .Include(r => r.Addresses)
                .Include(r => r.WorkingHours)
                .Include(r => r.RestaurantCuisines)
                    .ThenInclude(rc => rc.Cuisine)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<(IEnumerable<Restaurant> Items, int TotalCount)> GetPagedByCuisineAsync(
        int cuisineId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        {
            var query = _dbSet
                .Where(r => r.RestaurantCuisines.Any(rc => rc.CuisineId == cuisineId))
                .OrderBy(r => r.Name);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(r => r.Addresses)
                .Include(r => r.WorkingHours)
                .Include(r => r.RestaurantCuisines)
                    .ThenInclude(rc => rc.Cuisine)
                .ToListAsync(ct);

            return (items, totalCount);
        }
    }
}
