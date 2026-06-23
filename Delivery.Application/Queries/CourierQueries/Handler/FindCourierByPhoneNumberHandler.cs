using MediatR;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Domain.Enums;
using Delivery.Application.Helpers;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class FindCourierByPhoneNumberQueryHandler : IRequestHandler<FindCourierByPhoneNumberQuery, CourierDto?>
    {
        private readonly ICourierRepository _repo;

        public FindCourierByPhoneNumberQueryHandler(ICourierRepository repo) => _repo = repo;

        public async Task<CourierDto?> Handle(FindCourierByPhoneNumberQuery query, CancellationToken ct)
        {
            var courier = await _repo.GetByPhoneNumberAsync(query.PhoneNumber, ct);
            return courier?.ToDto();
        }
    }
}
