using MediatR;

namespace Delivery.Application.Commands.CourierCommands.Command
{
    public record DeleteCourierCommand(string CourierId) : IRequest<Unit>;
}
