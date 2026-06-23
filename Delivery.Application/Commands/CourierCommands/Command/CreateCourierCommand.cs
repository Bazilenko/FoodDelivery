using MediatR;

namespace Delivery.Application.Commands.CourierCommands.Command
{
    public record CreateCourierCommand(
        string Name,
        string Email,
        string PhoneNumber,
        string UserId
    ) : IRequest<string>;
    
}
