using Delivery.Application.DTOs;
using Delivery.Domain.ValueObjects;
using MediatR;

namespace Delivery.Application.Commands.DeliveryCommands.Command
{
    public record AssignDeliveryWindowCommand(
         string DeliveryId,
         DeliveryWindow Window
     ) : IRequest<DeliveryDto>;
}
