using AutoMapper;
using Orders.Dal.Entities;
using Orders.Bll.DTOs.Order;
using Orders.Bll.DTOs.OrderDish;
using Orders.Bll.DTOs.OrderDishOption;

namespace Orders.Bll.Mapper.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<CreateOrderRequestDto, Order>()
                .ForMember(dest => dest.OrderDishes, opt => opt.MapFrom(src => src.Dishes));

            CreateMap<CreateOrderDishDto, OrderDish>()
                .ForMember(dest => dest.OrderDishOptions, opt => opt.MapFrom(src => src.Options));

            CreateMap<CreateOrderDishOptionDto, OrderDishOption>();

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.Dishes, opt => opt.MapFrom(src => src.OrderDishes));

            CreateMap<OrderDish, OrderDishResponseDto>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.OrderDishOptions));

            CreateMap<OrderDishOption, OrderDishOptionResponseDto>();

            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>();
        }
    }
}
