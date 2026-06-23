using Delivery.Application.DTOs;
using MediatR;

namespace Delivery.Application.Commands.DeliveryCommands.Command
{
    public record AssignCourierToDeliveryCommand(
        string DeliveryId,
        string CourierId
    ) : IRequest<DeliveryDto>;
}
