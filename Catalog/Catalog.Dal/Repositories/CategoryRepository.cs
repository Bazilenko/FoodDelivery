using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MyDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Category>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(c => c.RestaurantId == restaurantId)
                .ToListAsync(ct);
        }

        public async Task<Category?> GetWithDishesAsync(int categoryId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(c => c.Dishes)
                .FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        }
        public async Task<IEnumerable<Category>> GetMenuByRestaurantIdAsync(
            int restaurantId,
            CancellationToken ct = default)
        {
            return await _dbSet
                .Where(c => c.RestaurantId == restaurantId)
                .Include(c => c.Dishes)
                .AsNoTracking()
                .ToListAsync(ct);
        }

    }
}
