namespace Catalog.Bll.DTOs.Address
{
    public record AddressDto(
        int Id,
        string City,
        string Street,
        string BuildingNumber,
        string? PostalCode,
        decimal Latitude,
        decimal Longitude);
}
