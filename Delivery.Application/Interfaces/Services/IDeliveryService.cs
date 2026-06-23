using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Domain.ValueObjects;

namespace Delivery.Domain.Interfaces.Services
{

    public interface IDeliveryService
    {
        Task<Entities.Delivery> GetDeliveryByIdAsync(string id, CancellationToken ct = default);
        Task<Entities.Delivery> CreateDeliveryAsync(CreateDeliveryCommand cmd, CancellationToken ct = default);
        Task<Entities.Delivery> AssignCourierToDeliveryAsync(string deliveryId, string courierId, CancellationToken ct = default);
        Task<Entities.Delivery> AssignDeliveryWindowAsync(string deliveryId, DeliveryWindow window, CancellationToken ct = default);
        Task SaveAsync(Entities.Delivery delivery, CancellationToken ct = default);
    }
}
