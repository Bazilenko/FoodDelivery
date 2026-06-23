using Delivery.Application.DTOs;
using MediatR;

namespace Delivery.Application.Queries.CourierQueries.Query
{
    public record GetDeliveryHistoryByCourierQuery(string CourierId) : IRequest<IEnumerable<DeliveryDto>>;
}