using AutoMapper;
using Catalog.Bll.DTOs.Contact;
using Catalog.Bll.Services.Interfaces;
using Catalog.Bll.Helpers; 
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Catalog.Bll.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public ContactService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _uow = uow;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync(CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            
            var contacts = await _uow.Contacts.GetByRestaurantAsync(restaurantId, ct);
            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<ContactDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            return _mapper.Map<ContactDto>(entity);
        }

        public async Task<ContactDto> CreateAsync(ContactCreateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);

            var entity = _mapper.Map<Contact>(dto);
            entity.RestaurantId = restaurantId;

            await _uow.Contacts.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<ContactDto>(entity);
        }

        public async Task<ContactDto> UpdateAsync(ContactUpdateDto dto, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(dto.Id, restaurantId, ct);

            _mapper.Map(dto, entity);

            await _uow.Contacts.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<ContactDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var restaurantId = AuthHelper.GetRestaurantIdFromToken(_httpContextAccessor);
            var entity = await GetOwnedAsync(id, restaurantId, ct);
            
            entity.IsDeleted = true;
            await _uow.Contacts.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }


        private async Task<Contact> GetOwnedAsync(int id, int restaurantId, CancellationToken ct)
        {
            var entity = await _uow.Contacts.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Contact {id} not found.");

            if (entity.RestaurantId != restaurantId)
                throw new UnauthorizedAccessException("Contact does not belong to your restaurant.");

            return entity;
        }
    }
}