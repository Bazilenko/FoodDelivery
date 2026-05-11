using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class CuisineRepository : GenericRepository<Cuisine>, ICuisineRepository
    {
        public CuisineRepository(MyDbContext dbContext) : base(dbContext){}
        public async Task<Cuisine?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Name == name, ct);
        }

        public async Task<IEnumerable<Cuisine>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(c => c.RestaurantCuisines.Any(rc => rc.RestaurantId == restaurantId))
                .ToListAsync(ct);
            
        }

    }
}
