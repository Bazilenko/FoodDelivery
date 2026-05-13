using Catalog.Bll.DTOs.Category;

namespace Catalog.Bll.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct = default);
        Task<CategoryDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<CategoryDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default);
        Task<CategoryDto> UpdateAsync(CategoryUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
