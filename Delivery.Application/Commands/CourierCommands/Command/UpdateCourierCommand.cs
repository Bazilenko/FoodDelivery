using MediatR;

namespace Delivery.Application.Commands.CourierCommands.Command
{
    public record UpdateCourierCommand(
        string CourierId,
        string Name,
        string Email,
        string PhoneNumber
    ) : IRequest<Unit>;
}
