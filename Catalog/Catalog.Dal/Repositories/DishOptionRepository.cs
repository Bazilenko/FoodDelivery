using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class DishOptionRepository : GenericRepository<DishOption>, IDishOptionRepository
    {
        public DishOptionRepository(MyDbContext dbContext) : base(dbContext){ }

        public async Task<IEnumerable<DishOption>> GetAvailableByModifierGroupAsync(int modifierGroupId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(o => o.ModifierGroupId == modifierGroupId && o.IsAvailable)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<DishOption>> GetByModifierGroupAsync(int modifierGroupId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(o => o.ModifierGroupId == modifierGroupId)
                .ToListAsync(ct);
        }
    }
}
