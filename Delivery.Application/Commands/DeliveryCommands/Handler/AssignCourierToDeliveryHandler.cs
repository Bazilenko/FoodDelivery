using Delivery.Application.DTOs;
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Application.Helpers;
using Delivery.Domain.Interfaces.Services;
using MediatR;

namespace Delivery.Application.Commands.DeliveryCommands.Handler
{
     public class AssignCourierToDeliveryCommandHandler : IRequestHandler<AssignCourierToDeliveryCommand, DeliveryDto>
    {
        private readonly IDeliveryService _service;

        public AssignCourierToDeliveryCommandHandler(IDeliveryService service) => _service = service;

        public async Task<DeliveryDto> Handle(AssignCourierToDeliveryCommand cmd, CancellationToken ct)
        {
            var delivery = await _service.AssignCourierToDeliveryAsync(cmd.DeliveryId, cmd.CourierId, ct);
            return delivery.ToDto();
        }
    }
}
