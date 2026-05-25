using AutoMapper;
using Orders.Dal.Entities;
using Orders.Dal.Enums;
using Orders.Dal.UoW.Interfaces;
using Orders.Bll.DTOs.Order;
using Orders.Bll.Services.Interfaces;

namespace Orders.Bll.Services;
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(int customerId)
    {
        var orders = await _uow.Orders.GetOrdersByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetRestaurantOrdersAsync(int restaurantId)
    {
        var orders = await _uow.Orders.GetOrdersByRestaurantIdAsync(restaurantId);
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto?> GetOrderByIdForCustomerAsync(int orderId, int customerId)
    {
        var order = await _uow.Orders.GetFullOrderDetailsAsync(orderId);

        if (order == null || order.CustomerId != customerId)
            return null;

        return _mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto dto, int customerId)
    {
        _uow.BeginTransaction();
        try
        {
            var order = _mapper.Map<Order>(dto);
            order.CustomerId = customerId;
            order.Status = OrderStatus.Pending;
            order.CreatedAt = DateTime.UtcNow;
            order.DeliveryFee = 55.00m;

            var orderId = await _uow.Orders.AddAsync(order);
            decimal totalAmount = order.DeliveryFee;

            foreach (var dish in order.OrderDishes)
            {
                dish.OrderId = orderId;
                var dishId = await _uow.OrderDishes.AddAsync(dish);

                totalAmount += (dish.PriceAtTimeOfOrder * dish.Quantity);

                foreach (var option in dish.OrderDishOptions)
                {
                    option.OrderDishId = dishId;
                    await _uow.OrderDishOptions.AddAsync(option);

                    totalAmount += (option.PriceAtTimeOfOrder * dish.Quantity);
                }
            }

            order.Id = orderId;
            order.TotalAmount = totalAmount;
            await _uow.Orders.ReplaceAsync(order);

            await _uow.StatusHistory.AddAsync(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = OrderStatus.Pending,
                Comment = "Замовлення прийнято"
            });

            _uow.Commit();

            var result = await _uow.Orders.GetFullOrderDetailsAsync(orderId);
            return _mapper.Map<OrderResponseDto>(result);
        }
        catch (Exception)
        {
            _uow.Rollback();
            throw;
        }
    }


    public async Task UpdateOrderStatusAsync(int orderId, int restaurantId, UpdateOrderStatusRequestDto dto)
    {
        var order = await _uow.Orders.GetAsync(orderId);
        if (order == null || order.RestaurantId != restaurantId)
        {
            throw new UnauthorizedAccessException("Ви не маєте доступу до цього замовлення.");
        }

        _uow.BeginTransaction();
        try
        {
            await _uow.Orders.UpdateStatusAsync(orderId, dto.Status);

            await _uow.StatusHistory.AddAsync(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = dto.Status,
                Comment = dto.Comment ?? $"Статус змінено на {dto.Status}"
            });

            _uow.Commit();
        }
        catch
        {
            _uow.Rollback();
            throw;
        }
    }
}