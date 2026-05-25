using Orders.Dal.Enums;

namespace Orders.Bll.DTOs.Order;

public class OrderStatusHistoryDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}