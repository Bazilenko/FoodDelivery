using Catalog.Bll.DTOs.ModifierGroup;
using Catalog.Dal.Entities;
using Catalog.Bll.Mapping.Extensions;
using AutoMapper;

namespace Catalog.Bll.Mapper.Profiles
{
    public class ModifierGroupProfile : Profile
    {
        public ModifierGroupProfile() {
             CreateMap<ModifierGroup, ModifierGroupDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.DishOptions));

            CreateMap<ModifierGroupCreateDto, ModifierGroup>()
                .IgnoreBaseEntityProperties()
                .ForMember(dest => dest.DishId, opt => opt.Ignore())
                .ForMember(dest => dest.Dish, opt => opt.Ignore())
                .ForMember(dest => dest.DishOptions, opt => opt.Ignore());
 
             CreateMap<ModifierGroupUpdateDto, ModifierGroup>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.DishId, opt => opt.Ignore())
                .ForMember(dest => dest.Dish, opt => opt.Ignore())
                .ForMember(dest => dest.DishOptions, opt => opt.Ignore());
        }
    }
}