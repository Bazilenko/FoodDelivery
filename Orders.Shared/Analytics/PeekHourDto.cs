namespace Orders.Shared.DTOs;

public record PeakHourDto(
    int Hour,
    int OrderCount,
    decimal Revenue);