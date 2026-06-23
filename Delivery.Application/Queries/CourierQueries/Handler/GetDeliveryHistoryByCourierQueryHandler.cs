using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Domain.Enums;
using Delivery.Application.Helpers;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class GetDeliveryHistoryByCourierQueryHandler : IRequestHandler<GetDeliveryHistoryByCourierQuery, IEnumerable<DeliveryDto>>
    {
        private readonly IDeliveryRepository _repo;

        public GetDeliveryHistoryByCourierQueryHandler(IDeliveryRepository repo) => _repo = repo;

        public async Task<IEnumerable<DeliveryDto>> Handle(GetDeliveryHistoryByCourierQuery query, CancellationToken ct)
        {
            var deliveries = await _repo.GetByCourierAndStatusAsync(query.CourierId, DeliveryStatus.Delivered, ct);
            return deliveries.Select(d => d.ToDto());
        }
    }
}