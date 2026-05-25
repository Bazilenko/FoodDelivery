namespace Catalog.Bll.DTOs.Address
{
    public record AddressUpdateDto(
        int Id,
        string City,
        string Street,
        string BuildingNumber,
        string? PostalCode,
        decimal Latitude,
        decimal Longitude);
}
