namespace Catalog.Bll.DTOs.WorkingHour
{
    public record WorkingHourDto(
        int Id,
        int DayOfWeek,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        bool IsClosed);
}