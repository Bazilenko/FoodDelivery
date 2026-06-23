using Delivery.Domain.ValueObjects;

namespace Delivery.Application.DTOs
{
    public class DeliveryDto
    {
        public string Id { get; set; }
        public int OrderId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal DeliveryFee { get; set; }
        public string Status { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public CourierDto Courier { get; set; }
        public double DistanceKm { get; set; }
        public int EstimatedMinutes { get; set; }
    }
}