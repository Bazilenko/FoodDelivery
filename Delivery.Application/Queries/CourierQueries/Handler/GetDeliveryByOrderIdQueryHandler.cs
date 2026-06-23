using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.Helpers;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class GetDeliveryByOrderIdQueryHandler : IRequestHandler<GetDeliveryByOrderIdQuery, DeliveryDto?>
    {
        private readonly IDeliveryRepository _repo;

        public GetDeliveryByOrderIdQueryHandler(IDeliveryRepository repo) => _repo = repo;

        public async Task<DeliveryDto?> Handle(GetDeliveryByOrderIdQuery query, CancellationToken ct)
        {
            var delivery = await _repo.GetByOrderIdAsync(query.OrderId, ct);
            return delivery?.ToDto();
        }
    }
}