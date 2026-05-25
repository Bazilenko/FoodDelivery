using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IDishOptionRepository : IGenericRepository<DishOption>
    {
        Task<IEnumerable<DishOption>> GetByModifierGroupAsync(int modifierGroupId, CancellationToken ct = default);
        Task<IEnumerable<DishOption>> GetAvailableByModifierGroupAsync(int modifierGroupId, CancellationToken ct = default);
    }
}
