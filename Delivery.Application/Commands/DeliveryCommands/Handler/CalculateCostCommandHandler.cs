
using Delivery.Application.Commands.DeliveryCommands.Command;
using Delivery.Application.Interfaces.Commands;
using Delivery.Domain.Exceptions;
using Delivery.Application.Interfaces.Repositories;
using MediatR;

namespace Delivery.Application.Commands.DeliveryCommands.Handler
{
    public class CalculateCostCommandHandler : ICommandHandler<CalculateCostCommand>
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public CalculateCostCommandHandler(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<Unit> Handle(CalculateCostCommand request, CancellationToken cancellationToken)
        {
            var repo = _deliveryRepository;

            var delivery = await repo.GetByIdAsync(request.DeliveryId, cancellationToken)
                ?? throw new DomainException("Delivery not found", "NotFound");

            delivery.CalculateCost(request.BaseRatePerKm);

            return Unit.Value;
        }
    }
}
