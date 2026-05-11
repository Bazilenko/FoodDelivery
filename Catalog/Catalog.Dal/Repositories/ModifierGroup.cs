using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class ModifierGroupRepository : GenericRepository<ModifierGroup>, IModifierGroupRepository
    {
        public ModifierGroupRepository(MyDbContext dbContext) : base(dbContext){}
        public async Task<IEnumerable<ModifierGroup>> GetByDishAsync(int dishId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(mg => mg.DishId == dishId)
                .Include(mg => mg.DishOptions)
                .ToListAsync(ct);
        }

        public async Task<ModifierGroup?> GetWithOptionsAsync(int modifierGroupId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(mg => mg.DishOptions)
                .FirstOrDefaultAsync(mg => mg.Id == modifierGroupId, ct);
        }

    }
}
