namespace Catalog.Bll.DTOs.WorkingHour
{
    public record WorkingHourCreateDto(
        int DayOfWeek,
        TimeSpan OpeningTime,
        TimeSpan ClosingTime,
        bool IsClosed);
}