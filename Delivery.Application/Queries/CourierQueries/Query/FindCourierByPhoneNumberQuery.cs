using MediatR;
using Delivery.Application.DTOs;

namespace Delivery.Application.Queries.CourierQueries.Query
{
    public record FindCourierByPhoneNumberQuery(string PhoneNumber) : IRequest<CourierDto?>;
}
