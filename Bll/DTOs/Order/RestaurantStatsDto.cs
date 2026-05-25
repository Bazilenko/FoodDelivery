namespace Orders.Bll.DTOs.Order;
public class RestaurantStatsDto
{
    public decimal TotalRevenue { get; set; }
    public int OrdersCount { get; set; }
    public decimal AverageCheck { get; set; }
    
    public IEnumerable<dynamic>? DailyBreakdown { get; set; } 
}