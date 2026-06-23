using MediatR;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Domain.Exceptions;
using Delivery.Application.Interfaces.ExternalServices;

namespace Delivery.Application.Commands.DeliveryCommands.Handler
{
    public class MarkInTransitCommandHandler : IRequestHandler<MarkInTransitCommand, Unit>
    {
        private readonly IDeliveryRepository _repo;
        private readonly IOrderServiceClient _orderClient;

        public MarkInTransitCommandHandler(IDeliveryRepository repo, IOrderServiceClient orderClient)
        {
            _repo = repo;
            _orderClient = orderClient;
        }

        public async Task<Unit> Handle(MarkInTransitCommand cmd, CancellationToken ct)
        {
            var delivery = await _repo.GetByIdAsync(cmd.DeliveryId, ct)
                ?? throw new DeliveryNotFoundException(cmd.DeliveryId);

            delivery.MarkInTransit();
            await _repo.SaveAsync(delivery, ct);
            await _orderClient.NotifyPickedUpAsync(delivery.OrderId, ct);

            return Unit.Value;
        }
    }
}