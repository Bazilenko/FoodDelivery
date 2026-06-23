using MediatR;
using Delivery.Application.DTOs;

namespace Delivery.Application.Queries.CourierQueries.Query
{
    public record GetAllDeliveriesQuery : IRequest<IEnumerable<DeliveryDto>>;
}
