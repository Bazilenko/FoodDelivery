using System;
using MediatR;
using Delivery.Domain.ValueObjects;

namespace Delivery.Application.Commands.DeliveryCommands.Command
{
    public record CreateDeliveryCommand(
        int OrderId,
        GeoCoordinate Pickup,
        GeoCoordinate Dropoff,
        string RestaurantName,
        string RestaurantAddress,
        string DeliveryAddress,
        decimal DeliveryFee
    ) : IRequest<string>;
}

