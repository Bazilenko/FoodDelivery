using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.Helpers;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class GetAllDeliveriesQueryHandler : IRequestHandler<GetAllDeliveriesQuery, IEnumerable<DeliveryDto>>
    {
        private readonly IDeliveryRepository _repo;

        public GetAllDeliveriesQueryHandler(IDeliveryRepository repo) => _repo = repo;

        public async Task<IEnumerable<DeliveryDto>> Handle(GetAllDeliveriesQuery _, CancellationToken ct)
        {
            var all = await _repo.GetByTimeRangeAsync(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddDays(1), ct);
            return all.Select(d => d.ToDto());
        }
    }
}
