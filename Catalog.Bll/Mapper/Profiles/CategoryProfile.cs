using AutoMapper;
using Catalog.Bll.DTOs.Category;
using Catalog.Dal.Entities;
using Catalog.Bll.Mapping.Extensions;

namespace Catalog.Bll.Mapper.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile() {
            CreateMap<Category, CategoryDto>();

            CreateMap<CategoryCreateDto, Category>()
                .IgnoreBaseEntityProperties()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.Dishes, opt => opt.Ignore());
 
            CreateMap<CategoryUpdateDto, Category>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.Dishes, opt => opt.Ignore());
        }
    }
}
