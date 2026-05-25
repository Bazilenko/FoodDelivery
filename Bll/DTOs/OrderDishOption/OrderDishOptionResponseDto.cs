namespace Orders.Bll.DTOs.OrderDishOption;
public class OrderDishOptionResponseDto
{
    public int Id { get; set; }
    public int OptionId { get; set; }

    public string OptionNameSnapshot { get; set; } = string.Empty;
    public decimal PriceAtTimeOfOrder { get; set; }
}