using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.Application.Commands.CourierCommands.Command;
using Delivery.Application.Interfaces.Commands;
using Delivery.Domain.Interfaces.Repositories;
using Delivery.Domain.Interfaces.Services;
using MediatR;

namespace Delivery.Application.Commands.CourierCommands.Handlers
{
    public class AssignCourierCommandHandler
    : ICommandHandler<AssignCourierCommand>
    {
        private readonly ICourierService _courierService;
        private readonly IDeliveryService _deliveryService;

        public AssignCourierCommandHandler(ICourierService courierService, IDeliveryService deliveryService)
        {
            _courierService = courierService;
            _deliveryService = deliveryService;
        }

        public async Task<Unit> Handle(AssignCourierCommand command, CancellationToken cancellationToken)
        {
            var courier = await _courierService.GetCourierByIdAsync(command.DeliveryId);
            var delivery = await _deliveryService.GetDeliveryByIdAsync(command.CourierId);

            delivery.AssignCourier(courier);

            return Unit.Value;
        }
    }
}
