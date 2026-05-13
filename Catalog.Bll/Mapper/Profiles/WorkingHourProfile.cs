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
                    opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.OpeningTime)))
                .ForMember(dest => dest.ClosingTime,
                    opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.ClosingTime)));
            
            CreateMap<WorkingHourCreateDto, WorkingHour>()
                .IgnoreBaseEntityProperties() 
                .ForMember(dest => dest.OpeningTime,
                    opt => opt.MapFrom(src => src.OpeningTime.ToTimeSpan()))
                .ForMember(dest => dest.ClosingTime,
                    opt => opt.MapFrom(src => src.ClosingTime.ToTimeSpan()))
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore());

            
            CreateMap<WorkingHourUpdateDto, WorkingHour>()
                .IgnoreAuditProperties()
                .ForMember(dest => dest.OpeningTime,
                    opt => opt.MapFrom(src => src.OpeningTime.ToTimeSpan()))
                .ForMember(dest => dest.ClosingTime,
                    opt => opt.MapFrom(src => src.ClosingTime.ToTimeSpan()))
                .ForMember(dest => dest.DayOfWeek, opt => opt.Ignore()) 
                .ForMember(dest => dest.RestaurantId, opt => opt.Ignore())
                .ForMember(dest => dest.Restaurant, opt => opt.Ignore());

            
        }
    }
}
