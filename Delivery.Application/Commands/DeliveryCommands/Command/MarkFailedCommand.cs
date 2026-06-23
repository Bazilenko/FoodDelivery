using Delivery.Application.Interfaces.Commands;
using MediatR;

namespace Delivery.Application.Commands.DeliveryCommands.Command
{
    public record MarkFailedCommand(string DeliveryId, string Reason) : IRequest<Unit>;
}