
using Delivery.Application.Commands.CourierCommands.Command;
using Delivery.Application.Interfaces.Commands;
using Delivery.Domain.Interfaces.Services;
using MediatR;

namespace Delivery.Application.Commands.CourierCommands.Handler
{
    public class DeleteCourierCommandHandler : IRequestHandler<DeleteCourierCommand, Unit>
    {
        private readonly ICourierService _service;

        public DeleteCourierCommandHandler(ICourierService service) => _service = service;

        public async Task<Unit> Handle(DeleteCourierCommand cmd, CancellationToken ct)
        {
            await _service.DeleteCourierAsync(cmd.CourierId, ct);
            return Unit.Value;
        }
    }
}
