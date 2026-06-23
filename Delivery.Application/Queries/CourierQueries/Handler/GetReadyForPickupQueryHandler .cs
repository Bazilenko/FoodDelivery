using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Domain.Enums;
using Delivery.Application.Helpers;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class GetReadyForPickupQueryHandler : IRequestHandler<GetReadyForPickupQuery, IEnumerable<DeliveryDto>>
    {
        private readonly IDeliveryRepository _repo;

        public GetReadyForPickupQueryHandler(IDeliveryRepository repo) => _repo = repo;

        public async Task<IEnumerable<DeliveryDto>> Handle(GetReadyForPickupQuery _, CancellationToken ct)
        {
            var deliveries = await _repo.GetByStatusAsync(DeliveryStatus.Pending, ct);
            return deliveries.Select(d => d.ToDto());
        }
    }

}