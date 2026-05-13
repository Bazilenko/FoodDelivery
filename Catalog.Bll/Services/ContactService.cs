using AutoMapper;
using Catalog.Bll.DTOs.Contact;
using Catalog.Bll.Services.Interfaces;
using Catalog.Dal.Entities;
using Catalog.Dal.UOW.Interfaces;

namespace Catalog.Bll.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IRestaurantContext _restaurantContext;

        public ContactService(IUnitOfWork uow, IMapper mapper, IRestaurantContext restaurantContext)
        {
            _uow = uow;
            _mapper = mapper;
            _restaurantContext = restaurantContext;
        }

        public async Task<IEnumerable<ContactDto>> GetAllAsync(CancellationToken ct = default)
        {
            var contacts = await _uow.Contacts.GetByRestaurantAsync(_restaurantContext.RestaurantId, ct);
            return _mapper.Map<IEnumerable<ContactDto>>(contacts);
        }

        public async Task<ContactDto> GetByIdAsync(int id, CancellationToken ct = default)
            => _mapper.Map<ContactDto>(await GetOwnedAsync(id, ct));

        public async Task<ContactDto> CreateAsync(ContactCreateDto dto, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Contact>(dto);
            entity.RestaurantId = _restaurantContext.RestaurantId;

            await _uow.Contacts.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<ContactDto>(entity);
        }

        public async Task<ContactDto> UpdateAsync(ContactUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(dto.Id, ct);

            _mapper.Map(dto, entity);

            await _uow.Contacts.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<ContactDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await GetOwnedAsync(id, ct);
            entity.IsDeleted = true;
            await _uow.Contacts.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<Contact> GetOwnedAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.Contacts.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Contact {id} not found.");

            if (entity.RestaurantId != _restaurantContext.RestaurantId)
                throw new UnauthorizedAccessException("Contact does not belong to your restaurant.");

            return entity;
        }
    }
}
