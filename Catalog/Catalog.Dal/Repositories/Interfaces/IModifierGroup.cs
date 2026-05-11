
using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IModifierGroupRepository : IGenericRepository<ModifierGroup>
    {
        Task<IEnumerable<ModifierGroup>> GetByDishAsync(int dishId, CancellationToken ct = default);
        Task<ModifierGroup?> GetWithOptionsAsync(int modifierGroupId, CancellationToken ct = default);
    }
}
