using AutoMapper;
using Catalog.Bll.DTOs.Contact;
using Catalog.Dal.Entities;
using Catalog.Dal.Enums;
using Catalog.Bll.Mapping.Extensions;

namespace Catalog.Bll.Mapper.Profiles
{
    public class ContactProfile : Profile
    {
        public ContactProfile() {
            CreateMap<Contact, ContactDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<ContactCreateDto, Contact>()
                .IgnoreBaseEntityProperties() 
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<ContactType>(src.Type, true)))
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore());

            CreateMap<ContactUpdateDto, Contact>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.Type,opt => opt.MapFrom(src => Enum.Parse<ContactType>(src.Type, true)));
        }
    }
}
