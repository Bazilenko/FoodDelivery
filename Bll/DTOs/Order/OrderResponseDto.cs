using  Orders.Dal.Enums;
using  Orders.Bll.DTOs.OrderDish;
namespace Orders.Bll.DTOs.Order;
public class OrderResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int RestaurantId { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal DeliveryFee { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<OrderDishResponseDto> Dishes { get; set; } = new();
}