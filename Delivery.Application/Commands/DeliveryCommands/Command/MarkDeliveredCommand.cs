using Delivery.Application.Interfaces.Commands;
using MediatR;

namespace Delivery.Application.Commands.DeliveryCommands.Command
{
    public record MarkDeliveredCommand(string DeliveryId) : IRequest<Unit>;
}