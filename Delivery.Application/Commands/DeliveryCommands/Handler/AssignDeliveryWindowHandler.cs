using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Application.Helpers;
using Delivery.Domain.Interfaces.Services;

namespace Delivery.Application.Commands.DeliveryCommands.Handler
{
    public class AssignDeliveryWindowCommandHandler : IRequestHandler<AssignDeliveryWindowCommand, DeliveryDto>
    {
        private readonly IDeliveryService _service;

        public AssignDeliveryWindowCommandHandler(IDeliveryService service) => _service = service;

        public async Task<DeliveryDto> Handle(AssignDeliveryWindowCommand cmd, CancellationToken ct)
        {
            var delivery = await _service.AssignDeliveryWindowAsync(cmd.DeliveryId, cmd.Window, ct);
            return delivery.ToDto();
        }
    }
}
