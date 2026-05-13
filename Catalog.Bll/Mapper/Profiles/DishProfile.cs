using AutoMapper;
using Catalog.Bll.DTOs.Dish;
using Catalog.Dal.Entities;
using Catalog.Bll.Mapping.Extensions;

namespace Catalog.Bll.Mapper.Profiles
{
    public class DishProfile : Profile
    {
        public DishProfile() {
            CreateMap<Dish, DishCardDto>();


            CreateMap<Dish, DishDetailDto>()
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.ModifierGroups,
                    opt => opt.MapFrom(src => src.ModifierGroups));

            CreateMap<Dish, DishManageDto>()
                .ForMember(dest => dest.Category,
                    opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.ModifierGroups,
                    opt => opt.MapFrom(src => src.ModifierGroups));

            CreateMap<DishCreateDto, Dish>()
                .IgnoreBaseEntityProperties()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.ModifierGroups, opt => opt.Ignore());

            CreateMap<DishUpdateDto, Dish>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.ModifierGroups, opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable, opt => opt.Ignore());

        }

    }
}
