using Delivery.Domain.Common;
using Delivery.Domain.Enums;
using Delivery.Domain.Exceptions;
using Delivery.Domain.Value_Objects;
using Delivery.Domain.ValueObjects;

namespace Delivery.Domain.Entities
{
    public class Delivery : BaseEntity
    {
        public int OrderId { get; private set; }
        public Courier Courier { get; private set; }
        public GeoCoordinate PickUpLocation { get; private set; }
        public GeoCoordinate DropOffLocation { get; private set; }
        public DeliveryWindow TimeWindow { get; private set; }
        public Money? DeliveryCost { get; private set; }
        public DeliveryStatus Status { get; private set; }

        public string RestaurantName { get; private set; }
        public string RestaurantAddress { get; private set; }
        public string DeliveryAddress { get; private set; }
        public decimal DeliveryFee { get; private set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        private Delivery() { }

        public Delivery(
           int orderId,
           GeoCoordinate pickup,
           GeoCoordinate dropoff,
           string restaurantName,
           string restaurantAddress,
           string deliveryAddress,
           decimal deliveryFee)
        {
            if (orderId <= 0)
                throw new DomainException("OrderId must be positive.", "InvalidValue");

            OrderId = orderId;
            PickUpLocation = pickup ?? throw new DomainException("Pickup cannot be null.", "InvalidValue");
            DropOffLocation = dropoff ?? throw new DomainException("Dropoff cannot be null.", "InvalidValue");
            RestaurantName = restaurantName;
            RestaurantAddress = restaurantAddress;
            DeliveryAddress = deliveryAddress;
            DeliveryFee = deliveryFee;
            Status = DeliveryStatus.Pending;
        }


        public void AssignWindow(DeliveryWindow window)
        {
            if (Status != DeliveryStatus.Pending)
                throw new DomainException(
                    $"Cannot assign window in status '{Status}'.", "InvalidStatus");

            TimeWindow = window ?? throw new DomainException("Window cannot be null.", "InvalidValue");
            Touch();
        }

        public void AssignCourier(Courier courier)
        {
            if (Courier != null)
                throw new DomainException("Courier already assigned.", "CourierException");

            if (Status != DeliveryStatus.Pending)
                throw new DomainException(
                    $"Cannot assign courier in status '{Status}'.", "InvalidStatus");

            Courier = courier ?? throw new DomainException("Courier cannot be null.", "CourierException");
            Touch();
        }

        public void CalculateCost(decimal baseRatePerKm)
        {
            if (baseRatePerKm <= 0)
                throw new DomainException("Base rate must be positive.", "InvalidValue");

            double distance = PickUpLocation.DistanceTo(DropOffLocation);
            DeliveryCost = new Money((decimal)distance * baseRatePerKm, "UAH");
            Touch();
        }

        public void MarkInTransit()
        {
            if (Status != DeliveryStatus.Pending)
                throw new DomainException(
                    $"Cannot mark InTransit from '{Status}'. Must be Pending.", "InvalidStatus");

            if (Courier == null)
                throw new DomainException(
                    "Cannot start delivery without an assigned courier.", "CourierException");

            Status = DeliveryStatus.InTransit;
            PickedUpAt = DateTime.UtcNow;
            Touch();
        }



        public void MarkDelivered()
        {
            if (Status != DeliveryStatus.InTransit)
                throw new DomainException(
                    $"Cannot mark Delivered from '{Status}'. Must be InTransit.", "InvalidStatus");

            Status = DeliveryStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            Touch();
        }

        public void MarkFailed(string reason)
        {
            if (Status == DeliveryStatus.Delivered)
                throw new DomainException("Cannot fail an already delivered order.", "InvalidStatus");

            Status = DeliveryStatus.Failed;
            Touch();
        }
    }
}