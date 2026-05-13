using Catalog.Bll.DTOs.Address;

namespace Catalog.Bll.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAsync(CancellationToken ct = default);
        Task<AddressDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AddressDto> CreateAsync(AddressCreateDto dto, CancellationToken ct = default);
        Task<AddressDto> UpdateAsync(AddressUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
