using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Dal.Context;
using Catalog.Dal.Repositories.Interfaces;
using Catalog.Dal.Repositories;
using Catalog.Dal.UOW.Interfaces;

namespace Catalog.Dal.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private MyDbContext _dbContext { get; }
        public ICategoryRepository? _categories;
        public IDishRepository? _dishes;
        public IRestaurantRepository? _restaurants;
        public IContactRepository? _contacts;
        public IAddressRepository? _addresses;
        public IDishOptionRepository? _dishOptions;
        public ICuisineRepository? _cuisines;
        public IModifierGroupRepository? _modifierGroups;
        public IRestaurantCuisineRepository? _restaurantCuisines;
        public IWorkingHourRepository? _workingHours;

        public UnitOfWork(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_dbContext);
        public IDishRepository Dishes => _dishes ??= new DishRepository(_dbContext);
        public IRestaurantRepository Restaurants => _restaurants ??= new RestaurantRepository(_dbContext);
        public IContactRepository Contacts => _contacts ??= new ContactRepository(_dbContext);
        public IAddressRepository Addresses => _addresses ??= new AddressRepository(_dbContext);
        public IDishOptionRepository DishOptions => _dishOptions ??= new DishOptionRepository(_dbContext);
        public ICuisineRepository Cuisines => _cuisines ??= new CuisineRepository(_dbContext);
        public IModifierGroupRepository ModifierGroups => _modifierGroups ??= new ModifierGroupRepository(_dbContext);
        public IRestaurantCuisineRepository RestaurantCuisines => _restaurantCuisines ??= new RestaurantCuisineRepository(_dbContext);
        public IWorkingHourRepository WorkingHours => _workingHours ??= new WorkingHourRepository(_dbContext);

        public void Dispose()
        {
            _dbContext.DisposeAsync();
            GC.SuppressFinalize(this);
        }
        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _dbContext.SaveChangesAsync(ct);
    }
}
