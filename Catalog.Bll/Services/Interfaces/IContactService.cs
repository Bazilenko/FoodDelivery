using Catalog.Bll.DTOs.Contact;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllAsync(CancellationToken ct = default);
        Task<ContactDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ContactDto> CreateAsync(ContactCreateDto dto, CancellationToken ct = default);
        Task<ContactDto> UpdateAsync(ContactUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
