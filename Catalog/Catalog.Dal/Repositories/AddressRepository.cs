using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Dal.Context;
using Microsoft.EntityFrameworkCore;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.Repositories
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(MyDbContext dbContext) : base(dbContext){ }

    public async Task<IEnumerable<Address>> GetByRestaurantAsync(int restaurantId, CancellationToken ct = default)
        {
            return await _dbSet
            .Where(a => a.RestaurantId == restaurantId)
            .ToListAsync(ct);
        }
    }
}
