namespace Catalog.Bll.DTOs.WorkingHour
{
    public record WorkingHourUpdateDto(
        int Id,
        TimeSpan OpeningTime,
        TimeSpan ClosingTime,
        bool IsClosed);
}