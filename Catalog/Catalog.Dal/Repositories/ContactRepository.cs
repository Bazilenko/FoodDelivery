using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Enums;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        public ContactRepository(MyDbContext dbContext) : base(dbContext){}
        public async Task<IEnumerable<Contact>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(c => c.RestaurantId == restaurantId)
                .ToListAsync(ct);
        }

         public async Task<IEnumerable<Contact>> GetByTypeAsync(int restaurantId, ContactType type, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(c => c.RestaurantId == restaurantId && c.Type == type)
                .ToListAsync(ct);
            
        }

    }
}
