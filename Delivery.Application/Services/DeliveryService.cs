using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Domain.Interfaces.Services;
using Delivery.Domain.ValueObjects;

namespace Delivery.Application.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _repository;

        public DeliveryService(IDeliveryRepository repository)
        {
            _repository = repository;
        }

        public async Task<Domain.Entities.Delivery> GetDeliveryByIdAsync(string id, CancellationToken ct = default)
        {
            return await _repository.GetByIdAsync(id, ct)
                ?? throw new Exception($"Delivery not found: {id}");
        }

        public async Task<Domain.Entities.Delivery> CreateDeliveryAsync(CreateDeliveryCommand cmd, CancellationToken ct = default)
        {
            var delivery = new Domain.Entities.Delivery(
                cmd.OrderId,
                cmd.Pickup,
                cmd.Dropoff,
                cmd.RestaurantName,
                cmd.RestaurantAddress,
                cmd.DeliveryAddress,
                cmd.DeliveryFee);

            await _repository.AddAsync(delivery, ct);
            return delivery;
        }

        public async Task<Domain.Entities.Delivery> AssignCourierToDeliveryAsync(string deliveryId, string courierId, CancellationToken ct = default)
        {
            var delivery = await GetDeliveryByIdAsync(deliveryId, ct);
            return delivery;
        }

        public async Task<Domain.Entities.Delivery> AssignDeliveryWindowAsync(string deliveryId, DeliveryWindow window, CancellationToken ct = default)
        {
            var delivery = await GetDeliveryByIdAsync(deliveryId, ct);
            delivery.AssignWindow(window);
            await _repository.SaveAsync(delivery, ct);
            return delivery;
        }

        public async Task SaveAsync(Domain.Entities.Delivery delivery, CancellationToken ct = default)
        {
            await _repository.SaveAsync(delivery, ct);
        }
    }
}
