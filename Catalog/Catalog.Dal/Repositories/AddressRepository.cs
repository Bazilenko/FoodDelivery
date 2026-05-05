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
        private readonly MyDbContext _dbContext;
        public AddressRepository(MyDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Address>> GetAddressesByCityAsync(string city)
        {
            return await _dbContext.Addresses
                  .Where(a => a.City.ToLower() == city.ToLower())
                  .ToListAsync();
        }

    public async Task<IEnumerable<Address>> GetAddressesByRestaurantIdAsync(int restaurantId)
        {
            return await _dbContext.Addresses
                .Where(a => a.RestaurantId == restaurantId) 
                .ToListAsync();
        }
    }
}
