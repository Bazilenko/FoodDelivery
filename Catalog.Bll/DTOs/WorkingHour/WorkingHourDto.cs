namespace Catalog.Bll.DTOs.WorkingHour
{
    public class WorkingHourDto
    {
        public int? Id { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsClosed { get; set; }
    }
}