namespace Catalog.Bll.DTOs.Address
{
    public record AddressCreateDto(
        string City,
        string Street,
        string BuildingNumber,
        string? PostalCode,
        decimal Latitude,
        decimal Longitude);
}
