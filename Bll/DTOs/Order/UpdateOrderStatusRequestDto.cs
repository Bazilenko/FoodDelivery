using Orders.Dal.Enums;

public class UpdateOrderStatusRequestDto
{
    public OrderStatus Status { get; set; }
    public string? Comment { get; set; }
}