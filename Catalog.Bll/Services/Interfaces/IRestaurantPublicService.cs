using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Bll.DTOs.Dish;
using Catalog.Bll.DTOs.Pagination;
using Catalog.Bll.DTOs.Restaurant;
using Catalog.Dal.UOW.Interfaces;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IRestaurantPublicService
    {
        /// <summary>Public detail page (addresses, contacts, working hours, cuisines).</summary>
        Task<RestaurantDetailDto> GetDetailAsync(int restaurantId, CancellationToken ct = default);
 
        /// <summary>Full menu: categories each containing their available dishes.</summary>
        Task<IEnumerable<CategoryWithDishesDto>> GetMenuAsync(int restaurantId, CancellationToken ct = default);

    }
}
