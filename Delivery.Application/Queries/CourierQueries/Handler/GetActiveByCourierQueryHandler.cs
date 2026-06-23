using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Domain.Enums;
using Delivery.Application.Helpers;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class GetActiveByCourierQueryHandler : IRequestHandler<GetActiveByCourierQuery, DeliveryDto?>
    {
        private readonly IDeliveryRepository _repo;

        public GetActiveByCourierQueryHandler(IDeliveryRepository repo) => _repo = repo;

        public async Task<DeliveryDto?> Handle(GetActiveByCourierQuery query, CancellationToken ct)
        {
            var deliveries = await _repo.GetByCourierAndStatusAsync(query.CourierId, DeliveryStatus.InTransit, ct);
            return deliveries.FirstOrDefault()?.ToDto();
        }
    }
}