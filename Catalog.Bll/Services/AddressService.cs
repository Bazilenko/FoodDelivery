using AutoMapper;
using Catalog.Bll.DTOs.Address;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;

namespace Catalog.Bll.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRestaurantContext _restaurantContext;

        public AddressService(IUnitOfWork uow, IMapper mapper, IRestaurantContext restaurantContext)
        {
            _uow = uow;
            _mapper = mapper;
            _restaurantContext = restaurantContext;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAsync(CancellationToken ct = default)
        {
            var addresses = await _uow.Addresses.GetByRestaurantAsync(_restaurantContext.RestaurantId, ct);
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto> GetByIdAsync(int id, CancellationToken ct = default)
            => _mapper.Map<AddressDto>(await GetOwnedAsync(id, ct));

        public async Task<AddressDto> CreateAsync(AddressCreateDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Address>(dto);
            entity.RestaurantId = _restaurantContext.RestaurantId;

            await _uow.Addresses.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<AddressDto>(entity);
        }

        public async Task<AddressDto> UpdateAsync(AddressUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(dto.Id, ct);

            _mapper.Map(dto, entity);

            await _uow.Addresses.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<AddressDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(id, ct);
            entity.IsDeleted = true;
            await _uow.Addresses.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<Address> GetOwnedAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.Addresses.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Address {id} not found.");

            if (entity.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Address does not belong to your restaurant.");

            return entity;
        }
    }
}

