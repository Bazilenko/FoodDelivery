using MediatR;
using Delivery.Application.DTOs;

namespace Delivery.Application.Queries.CourierQueries.Query
{
    public record GetActiveByCourierQuery(string CourierId) : IRequest<DeliveryDto?>;
}