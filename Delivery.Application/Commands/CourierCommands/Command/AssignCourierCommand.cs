using ICommand = Delivery.Application.Interfaces.Commands.ICommand;

namespace Delivery.Application.Commands.CourierCommands.Command
{
        public record AssignCourierCommand(string DeliveryId, string CourierId) : ICommand;
    
}
