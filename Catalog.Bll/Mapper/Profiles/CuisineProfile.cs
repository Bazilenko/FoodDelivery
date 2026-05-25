using Catalog.Bll.DTOs.Cuisine;
using Catalog.Dal.Entities;
using Catalog.Bll.Mapping.Extensions;
using AutoMapper;

namespace Catalog.Bll.Mapper.Profiles
{
    public class CuisineProfile : Profile
    {
        public CuisineProfile() {
            CreateMap<Cuisine, CuisineDto>();

            CreateMap<CuisineCreateDto, Cuisine>()
                .IgnoreBaseEntityProperties()
                .ForMember(dest => dest.RestaurantCuisines, opt => opt.Ignore());

            CreateMap<CuisineUpdateDto, Cuisine>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.RestaurantCuisines, opt => opt.Ignore());
        }
    }
}