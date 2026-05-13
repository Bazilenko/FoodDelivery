namespace Catalog.Bll.DTOs.Contact
{
    public record WorkingHoursDto(
        int Id,
        int DayOfWeek,
        TimeOnly OpeningTime,
        TimeOnly ClosingTime,
        bool IsClosed);
}