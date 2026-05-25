namespace Orders.Shared.DTOs;

public record CustomerRetentionDto(
    int TotalCustomers,
    int NewCustomers,
    int ReturningCustomers,
    decimal RetentionRate);