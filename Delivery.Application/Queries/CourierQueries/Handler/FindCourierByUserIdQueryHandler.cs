using MediatR;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Application.DTOs;
using Delivery.Application.Queries.CourierQueries.Query;

namespace Delivery.Application.Queries.CourierQueries.Handler
{
    public class FindCourierByUserIdQueryHandler 
    : IRequestHandler<FindCourierByUserIdQuery, CourierDto?>
{
    private readonly ICourierRepository _courierRepository;

    public FindCourierByUserIdQueryHandler(ICourierRepository courierRepository)
    {
        _courierRepository = courierRepository;
    }

    public async Task<CourierDto?> Handle(
        FindCourierByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var courier = await _courierRepository.FindByUserIdAsync(
            request.UserId,
            cancellationToken
        );

        if (courier is null)
            return null;

        return new CourierDto
        {
            Id = courier.Id,
            UserId = courier.UserId,
            Name = courier.Name,
            Email = courier.Email,
            PhoneNumber = courier.PhoneNumber
        };
    }
}
}