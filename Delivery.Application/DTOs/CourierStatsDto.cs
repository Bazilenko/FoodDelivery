
namespace Delivery.Application.DTOs
{
    public class CourierStatsDto
    {
        public int TotalDeliveries { get; set; }
        public decimal TotalEarned { get; set; }
        public int ActiveDeliveries { get; set; }
        public double AverageDeliveryTimeMinutes { get; set; }
        public int TodayDeliveries { get; set; }
        public decimal TodayEarned { get; set; }
    }
}