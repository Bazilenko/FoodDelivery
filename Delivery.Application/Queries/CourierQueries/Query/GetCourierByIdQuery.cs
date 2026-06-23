using MediatR;
using Delivery.Application.DTOs;

namespace Delivery.Application.Queries.CourierQueries.Query
{
    public record GetCourierByIdQuery(string CourierId) : IRequest<CourierDto?>;
}
