using Catalog.Bll.DTOs.Dish;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IDishPublicService
    {
        /// <summary>Click on a dish card — returns full detail with modifier groups.</summary>
        Task<DishDetailDto> GetDetailAsync(int dishId, CancellationToken ct = default);
    }
 
}
