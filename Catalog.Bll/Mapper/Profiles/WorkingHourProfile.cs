using AutoMapper;
using Catalog.Dal.Entities;
using Catalog.Bll.DTOs.WorkingHour;
using Catalog.Bll.Mapping.Extensions;

namespace Catalog.Bll.Mapper.Profiles
{
    public class WorkingHourProfile : Profile
    {
        public WorkingHourProfile() {
            CreateMap<WorkingHour, WorkingHourDto>()
                .ForMember(dest => dest.OpeningTime,
                    opt => opt.MapFrom(src => src.OpeningTime))
                .ForMember(dest => dest.ClosingTime,
                    opt => opt.MapFrom(src => src.ClosingTime));
            
            CreateMap<WorkingHourCreateDto, WorkingHour>()
                .IgnoreBaseEntityProperties() 
                .ForMember(dest => dest.OpeningTime,
                    opt => opt.MapFrom(src => src.OpeningTime))
                .ForMember(dest => dest.ClosingTime,
                    opt => opt.MapFrom(src => src.ClosingTime))
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore());

            
            CreateMap<WorkingHourUpdateDto, WorkingHour>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.OpeningTime,
                    opt => opt.MapFrom(src => src.OpeningTime))
                .ForMember(dest => dest.ClosingTime,
                    opt => opt.MapFrom(src => src.ClosingTime))
                .ForMember(dest => dest.DayOfWeek, opt => opt.Ignore()) 
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

                CreateMap<WorkingHourDto, WorkingHour>()
                .IgnoreAuditProperties() 
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore())
                .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => src.OpeningTime))
                .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => src.ClosingTime))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            
        }
    }
}
