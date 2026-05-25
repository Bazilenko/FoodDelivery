using Orders.Bll.DTOs.OrderDish;

namespace Dal.DTOs.Order
{
    public class OrderDetailsDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    
    public List<OrderDishDto> Dishes { get; set; } = [];
    public List<OrderStatusHistoryDto> History { get; set; } = [];
}
}
