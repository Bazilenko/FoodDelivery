using Delivery.Application.Commands.CourierCommands.Command;
using Delivery.Domain.Interfaces.Services;
using MediatR;

namespace Delivery.Application.Commands.CourierCommands.Handlers
{
    public class UpdateCourierCommandHandler : IRequestHandler<UpdateCourierCommand, Unit>
    {
        private readonly ICourierService _service;

        public UpdateCourierCommandHandler(ICourierService service) => _service = service;

        public async Task<Unit> Handle(UpdateCourierCommand cmd, CancellationToken ct)
        {
            await _service.UpdateCourierAsync(cmd.CourierId, cmd.Name, cmd.Email, cmd.PhoneNumber, ct);
            return Unit.Value;
        }
    }
}
