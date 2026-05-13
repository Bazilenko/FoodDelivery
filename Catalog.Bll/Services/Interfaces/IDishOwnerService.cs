using Catalog.Bll.DTOs.Dish;

namespace Catalog.Bll.Services.Interfaces
{
    
    public interface IDishOwnerService
    {
        Task<IEnumerable<DishManageDto>> GetAllAsync(CancellationToken ct = default);
        Task<DishManageDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<DishManageDto> CreateAsync(DishCreateDto dto, CancellationToken ct = default);
        Task<DishManageDto> UpdateAsync(DishUpdateDto dto, CancellationToken ct = default);
        Task SetAvailabilityAsync(DishAvailabilityDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}