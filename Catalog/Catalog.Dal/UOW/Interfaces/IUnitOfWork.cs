using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Dal.Context;
using Catalog.Dal.Repositories.Interfaces;

namespace Catalog.Dal.UOW.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IDishRepository Dishes { get; }
        IRestaurantRepository Restaurants { get; }
        IContactRepository Contacts { get; }
        IAddressRepository Addresses { get; }
        IDishOptionRepository DishOptions { get; }
        ICuisineRepository Cuisines {get;}
        IModifierGroupRepository ModifierGroups {get;}
        IRestaurantCuisineRepository RestaurantCuisines {get;}
        IWorkingHourRepository WorkingHours {get;}

        Task SaveChangesAsync(CancellationToken ct = default);
        void Dispose();
        ValueTask DisposeAsync();

    }
}
