using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivery.Infrastructure.ExternalServices
{
    public interface IOrderServiceClient
    {
        Task NotifyPickedUpAsync(int orderId, CancellationToken ct = default);
        Task NotifyDeliveredAsync(int orderId, CancellationToken ct = default);
        Task NotifyFailedAsync(int orderId, string reason, CancellationToken ct = default);
    }
}