namespace Orders.Shared.DTOs;

public record TopCustomerDto
{
    public int CustomerId { get; init; }
    public int TotalOrders { get; init; }
    public decimal TotalSpend { get; init; }
    public decimal AverageOrderValue { get; init; }
    public DateTime FirstOrderAt { get; init; }
    public DateTime LastOrderAt { get; init; }
}