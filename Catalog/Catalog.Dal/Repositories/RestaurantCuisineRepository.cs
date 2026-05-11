using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class RestaurantCuisineRepository : GenericRepository<RestaurantCuisine>, IRestaurantCuisineRepository
    {
        public RestaurantCuisineRepository(MyDbContext dbContext) : base(dbContext){}
        public async Task<bool> ExistsAsync(int restaurantId, int cuisineId, CancellationToken ct = default)
        {
            return await _dbSet
                .AnyAsync(rc => rc.RestaurantId == restaurantId && rc.CuisineId == cuisineId, ct);
        }

        public async Task<IEnumerable<RestaurantCuisine>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(rc => rc.RestaurantId == restaurantId)
                .Include(rc => rc.Cuisine)
                .ToListAsync(ct);
        }

    }
}
