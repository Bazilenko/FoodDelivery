using MediatR;

namespace Delivery.Application.Commands.DeliveryCommands.Command
{
    public record MarkInTransitCommand(string DeliveryId) : IRequest<Unit>;
}