using Catalog.Bll.DTOs.DishOption;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IDishOptionService
    {
        Task<IEnumerable<DishOptionDto>> GetByModifierGroupAsync(int modifierGroupId, CancellationToken ct = default);
        Task<DishOptionDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<DishOptionDto> CreateAsync(int modifierGroupId, DishOptionCreateDto dto, CancellationToken ct = default);
        Task<DishOptionDto> UpdateAsync(DishOptionUpdateDto dto, CancellationToken ct = default);
        Task SetAvailabilityAsync(int id, bool isAvailable, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}