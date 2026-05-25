namespace Orders.Bll.DTOs.Analytics;
public record AnalyticsRequest(
    DateTime From,
    DateTime To,
    int TopN = 10);