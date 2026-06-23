using MediatR;
using Delivery.Application.DTOs;

namespace Delivery.Application.Queries.CourierQueries.Query
{
    public record GetDeliveryByOrderIdQuery(int OrderId) : IRequest<DeliveryDto?>;
}