using MediatR;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Domain.Exceptions;
using Delivery.Application.Interfaces.ExternalServices;

namespace Delivery.Application.Commands.DeliveryCommands.Handler
{
    public class MarkFailedCommandHandler : IRequestHandler<MarkFailedCommand, Unit>
    {
        private readonly IDeliveryRepository _repo;
        private readonly IOrderServiceClient _orderClient;

        public MarkFailedCommandHandler(IDeliveryRepository repo, IOrderServiceClient orderClient)
        {
            _repo = repo;
            _orderClient = orderClient;
        }

        public async Task<Unit> Handle(MarkFailedCommand cmd, CancellationToken ct)
        {
            var delivery = await _repo.GetByIdAsync(cmd.DeliveryId, ct)
                ?? throw new DeliveryNotFoundException(cmd.DeliveryId);

            delivery.MarkFailed(cmd.Reason);
            await _repo.SaveAsync(delivery, ct);
            await _orderClient.NotifyFailedAsync(delivery.OrderId, cmd.Reason, ct);

            return Unit.Value;
        }
    }
}