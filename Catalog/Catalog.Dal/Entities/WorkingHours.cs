namespace Catalog.Dal.Entities
{
    public class WorkingHours : BaseEntity
    {
        public int RestaurantId {get; set;}
        public int DayOfWeek {get; set;}
        public DateTime OpeningTime {get; set;}
        public DateTime ClosingTime {get; set;}
        public bool IsClosed {get; set;}

        public Restaurant Restaurant {get; set;} = null!;

    }
}