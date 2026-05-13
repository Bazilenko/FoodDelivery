namespace Catalog.Bll.DTOs.WorkingHour
{
    public record WorkingHourCreateDto(
        int DayOfWeek,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        bool IsClosed);
}