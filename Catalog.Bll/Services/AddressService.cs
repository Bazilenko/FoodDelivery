using AutoMapper;
using Catalog.Bll.DTOs.Address;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.Helpers;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Catalog.Bll.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAsync(CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var addresses = await _uow.Addresses.GetByRestaurantAsync(restaurantId, ct);
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            return _mapper.Map<AddressDto>(entity);
        }

        public async Task<AddressDto> CreateAsync(AddressCreateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var entity = _mapper.Map<Address>(dto);
            entity.RestaurantId = restaurantId;

            await _uow.Addresses.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<AddressDto>(entity);
        }

        public async Task<AddressDto> UpdateAsync(AddressUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(dto.Id, restaurantId, ct);

            _mapper.Map(dto, entity);

            await _uow.Addresses.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<AddressDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            
            entity.IsDeleted = true;
            await _uow.Addresses.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<Address> GetOwnedAsync(int id, int restaurantId, CancellationToken ct)
        {
            var entity = await _uow.Addresses.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Address {id} not found.");

            if (entity.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Address does not belong to your restaurant.");

            return entity;
        }
    }
}