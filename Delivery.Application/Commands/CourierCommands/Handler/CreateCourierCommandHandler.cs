using MediatR;
using Delivery.Application.Commands.CourierCommands.Command;
using Delivery.Domain.Interfaces.Services;
using Delivery.Application.Interfaces.ExternalServices;

namespace Delivery.Application.Commands.CourierCommands.Handlers
{
    public class CreateCourierCommandHandler : IRequestHandler<CreateCourierCommand, string>
    {
        private readonly ICourierService _courierService;
        private readonly IIdentityServiceClient _identityClient;

        public CreateCourierCommandHandler(ICourierService courierService, IIdentityServiceClient identityClient)
        {
            _courierService = courierService;
            _identityClient = identityClient;
        }

        public async Task<string> Handle(CreateCourierCommand cmd, CancellationToken ct)
        {
            var courier = await _courierService.CreateCourierAsync(
                cmd.Name, cmd.Email, cmd.PhoneNumber, cmd.UserId, ct);

            await _identityClient.AssignCourierRoleAsync(cmd.UserId, ct);
            return courier.Id;
        }
    }
}
