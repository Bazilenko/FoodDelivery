namespace Catalog.Bll.DTOs.WorkingHour
{
    public record WorkingHourUpdateDto(
        int Id,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        bool IsClosed);
}