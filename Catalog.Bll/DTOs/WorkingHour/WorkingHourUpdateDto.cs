namespace Catalog.Bll.DTOs.WorkingHour
{
    public record WorkingHoursUpdateDto(
        int Id,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        bool IsClosed);
}