using AutoMapper;
using Catalog.Bll.DTOs.Restaurant;
using Catalog.Bll.Mapping.Extensions;
using Catalog.Dal.Entities;

namespace Catalog.Bll.Mapper.Profiles
{
    public class RestaurantProfile : Profile
    {
        public RestaurantProfile() {
             CreateMap<Restaurant, RestaurantDetailDto>()
                .ForMember(dest => dest.Cuisines,
                    opt => opt.MapFrom(src => src.RestaurantCuisines.Select(rc => rc.Cuisine)))
                .ForMember(dest => dest.Addresses,
                    opt => opt.MapFrom(src => src.Addresses))
                .ForMember(dest => dest.Contacts,
                    opt => opt.MapFrom(src => src.Contacts))
                .ForMember(dest => dest.WorkingHours,
                    opt => opt.MapFrom(src => src.WorkingHours))
            // IsOpen is computed — ignored here, set by the service after mapping
                .ForMember(dest => dest.IsOpen, opt => opt.Ignore());

            CreateMap<Restaurant, RestaurantProfileDto>()
                .ForMember(dest => dest.Cuisines,
                    opt => opt.MapFrom(src => src.RestaurantCuisines.Select(rc => rc.Cuisine)))
                .ForMember(dest => dest.Addresses,
                    opt => opt.MapFrom(src => src.Addresses))
                .ForMember(dest => dest.Contacts,
                    opt => opt.MapFrom(src => src.Contacts))
                .ForMember(dest => dest.WorkingHours,
                    opt => opt.MapFrom(src => src.WorkingHours));

            CreateMap<RestaurantUpdateDto, Restaurant>()
            .IgnoreBaseEntityProperties()
            .ForMember(dest => dest.Rating, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.Contacts, opt => opt.Ignore())
            .ForMember(dest => dest.WorkingHours, opt => opt.Ignore())
            .ForMember(dest => dest.RestaurantCuisines, opt => opt.Ignore())
            .ForMember(dest => dest.Dishes, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore());
        }
    }
}
