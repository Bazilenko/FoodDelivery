using Delivery.Application.DTOs;

namespace Delivery.Application.Helpers
{
    public static class DeliveryMappings
    {
        public static DeliveryDto ToDto(this Domain.Entities.Delivery delivery)
        {
            return new DeliveryDto
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                RestaurantName = delivery.RestaurantName,
                DeliveryAddress = delivery.DeliveryAddress,
                DeliveryFee = delivery.DeliveryFee,
                Status = delivery.Status.ToString(),
                PickedUpAt = delivery.PickedUpAt,
                DeliveredAt = delivery.DeliveredAt
            };
        }
        public static CourierDto ToDto(this Domain.Entities.Courier courier)
        {
            return new CourierDto
            {
                Id = courier.Id,
                Name = courier.Name,
                Email = courier.Email,
                PhoneNumber = courier.PhoneNumber,
                UserId = courier.UserId
            };
        }
    }
}
