using MediatR;
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Application.Interfaces.Commands;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Interfaces.Services;

namespace Delivery.Application.Commands.DeliveryCommands.Handler
{

    public class CreateDeliveryCommandHandler : IRequestHandler<CreateDeliveryCommand, string>
    {
        private readonly IDeliveryService _service;

        public CreateDeliveryCommandHandler(IDeliveryService service) => _service = service;

        public async Task<string> Handle(CreateDeliveryCommand cmd, CancellationToken ct)
        {
            var delivery = await _service.CreateDeliveryAsync(cmd, ct);
            return delivery.Id;
        }
    }
}
