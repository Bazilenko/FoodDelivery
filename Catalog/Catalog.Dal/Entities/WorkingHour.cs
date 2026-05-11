namespace Catalog.Dal.Entities
{
    public class WorkingHour : BaseEntity
    {
        public int RestaurantId {get; set;}
        public int DayOfWeek {get; set;}
        public TimeSpan OpeningTime {get; set;}
        public TimeSpan ClosingTime {get; set;}
        public bool IsClosed {get; set;}

        public Restaurant Restaurant {get; set;} = null!;

    }
}