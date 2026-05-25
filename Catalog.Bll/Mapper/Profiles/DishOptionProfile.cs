using Catalog.Bll.DTOs.DishOption;
using Catalog.Dal.Entities;
using Catalog.Bll.Mapping.Extensions;
using AutoMapper;

namespace Catalog.Bll.Mapper.Profiles
{
    public class DishOptionProfile : Profile
    {
        public DishOptionProfile() {
            CreateMap<DishOption, DishOptionDto>();

            CreateMap<DishOptionCreateDto, DishOption>()
                .IgnoreBaseEntityProperties()
                .ForMember(dest => dest.ModifierGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.ModifierGroup, opt => opt.Ignore());
 
            CreateMap<DishOptionUpdateDto, DishOption>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.ModifierGroupId, opt => opt.Ignore())
                .ForMember(dest => dest.ModifierGroup, opt => opt.Ignore());
        }
    }
}