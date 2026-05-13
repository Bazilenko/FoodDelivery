using Catalog.Bll.Mapping.Extensions;
using AutoMapper;
using Catalog.Bll.DTOs.Address;
using Catalog.Dal.Entities;

namespace Catalog.Bll.Mapper.Profiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile() {
            CreateMap<Address, AddressDto>();

            CreateMap<AddressCreateDto, Address>()
                .IgnoreBaseEntityProperties()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore()) 
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore());

            CreateMap<AddressUpdateDto, Address>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore());
        }
    }
}
