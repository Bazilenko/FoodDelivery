using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.Helpers;
using Delivery.Application.Queries.CourierQueries.Query;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class GetCourierByIdQueryHandler : IRequestHandler<GetCourierByIdQuery, CourierDto?>
    {
        private readonly ICourierRepository _repo;

        public GetCourierByIdQueryHandler(ICourierRepository repo) => _repo = repo;

        public async Task<CourierDto?> Handle(GetCourierByIdQuery query, CancellationToken ct)
        {
            var courier = await _repo.GetByIdAsync(query.CourierId, ct);
            return courier?.ToDto();
        }
    }

}
