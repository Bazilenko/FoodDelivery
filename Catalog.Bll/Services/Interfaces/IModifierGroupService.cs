using Catalog.Bll.DTOs.ModifierGroup;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IModifierGroupService
    {
        Task<IEnumerable<ModifierGroupDto>> GetByDishAsync(int dishId, CancellationToken ct = default);
        Task<ModifierGroupDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ModifierGroupDto> CreateAsync(int dishId, ModifierGroupCreateDto dto, CancellationToken ct = default);
        Task<ModifierGroupDto> UpdateAsync(ModifierGroupUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}