public class OrderStatusHistoryDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? Comment { get; set; }
}