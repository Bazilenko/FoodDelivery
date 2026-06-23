namespace Orders.Shared.DTOs;

public class TopDishDto
{
    public int DishId { get; set; }

    public string DishNameSnapshot { get; set; } = string.Empty;

    public string CategorySnapshot { get; set; } = string.Empty;

    public int TotalQuantityOrdered { get; set; }

    public decimal TotalRevenue { get; set; }

    public int AppearanceInOrders { get; set; }
}