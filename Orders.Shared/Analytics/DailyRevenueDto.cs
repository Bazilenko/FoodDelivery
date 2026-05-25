namespace Orders.Shared.DTOs;

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int OrdersCount { get; set; }
}