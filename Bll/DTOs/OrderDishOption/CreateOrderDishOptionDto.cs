namespace Orders.Bll.DTOs.OrderDishOption;

public class CreateOrderDishOptionDto
{
    public int OptionId { get; set; }
    public string OptionNameSnapshot { get; set; } = string.Empty;
    public decimal PriceAtTimeOfOrder { get; set; }
}