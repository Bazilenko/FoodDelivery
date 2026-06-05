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

        private Delivery() { }

        public Delivery(int orderId, GeoCoordinate pickup, GeoCoordinate dropoff)
        {
            if (orderId <= 0)
                throw new DomainException("OrderId must be positive.", "InvalidValue");

            OrderId = orderId;
            PickUpLocation = pickup ?? throw new DomainException("Pickup location cannot be null.", "InvalidValue");
            DropOffLocation = dropoff ?? throw new DomainException("Dropoff location cannot be null.", "InvalidValue");
            Status = DeliveryStatus.Pending;
        }

        public void AssignWindow(DeliveryWindow window)
        {
            if (Status != DeliveryStatus.Pending)
                throw new DomainException("Can only assign window while delivery is Pending.", "InvalidStatus");

            TimeWindow = window ?? throw new DomainException("Window cannot be null.", "InvalidValue");
            Touch();
        }

        public void AssignCourier(Courier courier)
        {
            if (Courier != null)
                throw new DomainException("Courier already assigned.", "CourierException");

            if (Status != DeliveryStatus.Pending)
                throw new DomainException("Can only assign courier while delivery is Pending.", "InvalidStatus");

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
                    $"Cannot mark as InTransit from status '{Status}'.", "InvalidStatus");

            if (Courier == null)
                throw new DomainException("Cannot start delivery without an assigned courier.", "CourierException");

            Status = DeliveryStatus.InTransit;
            Touch();
        }


        public void MarkDelivered()
        {
            if (Status != DeliveryStatus.InTransit)
                throw new DomainException(
                    $"Cannot mark as Delivered from status '{Status}'. Must be InTransit first.", "InvalidStatus");

            Status = DeliveryStatus.Delivered;
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