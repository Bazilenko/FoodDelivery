using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class DishRepository : GenericRepository<Dish>, IDishRepository
    {
        public DishRepository(MyDbContext dbContext) : base(dbContext) { }
        public async Task<Dish?> GetWithModifiersAsync(int dishId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(d => d.ModifierGroups)
                    .ThenInclude(mg => mg.DishOptions)
                .FirstOrDefaultAsync(d => d.Id == dishId, ct);
        }
        public async Task<IEnumerable<Dish>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(d => d.CategoryId == categoryId)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Dish>> GetByRestaurantAsync(int restaurantId, bool availableOnly = false, CancellationToken ct = default)
        {
            var query = _dbSet.Where(d => d.RestaurantId == restaurantId);

            if (availableOnly)
                query = query.Where(d => d.IsAvailable);

            return await query
                .Include(d => d.Category)
                .ToListAsync(ct);
        }

        public async Task<Dish?> GetDetailAsync(int dishId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(d => d.Category)
                .Include(d => d.ModifierGroups)
                    .ThenInclude(g => g.DishOptions)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == dishId, ct);
        }
    }
}
