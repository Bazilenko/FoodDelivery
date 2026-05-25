using Catalog.Dal.Enums;
using Catalog.Dal.Entities;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<IEnumerable<Contact>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default);
        Task<IEnumerable<Contact>> GetByTypeAsync(int restaurantId, ContactType type, CancellationToken ct = default);
    }
}
