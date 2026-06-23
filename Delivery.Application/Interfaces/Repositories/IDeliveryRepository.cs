using Delivery.Domain.Entities;
using Delivery.Domain.Enums;
using Delivery.Domain.ValueObjects;

namespace Delivery.Application.Interfaces.Repositories
{
    public interface IDeliveryRepository
    {
        Task<Domain.Entities.Delivery?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<Domain.Entities.Delivery?> GetByOrderIdAsync(int orderId, CancellationToken ct = default);
        Task<IEnumerable<Domain.Entities.Delivery>> GetByStatusAsync(DeliveryStatus status, CancellationToken ct = default);
        Task<IEnumerable<Domain.Entities.Delivery>> GetByCourierIdAsync(string courierId, CancellationToken ct = default);
        Task<IEnumerable<Domain.Entities.Delivery>> GetByCourierAndStatusAsync(string courierId, DeliveryStatus status, CancellationToken ct = default);
        Task<IEnumerable<Domain.Entities.Delivery>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<int> GetActiveDeliveryCountByCourierAsync(string courierId, CancellationToken ct = default);
        Task SaveAsync(Domain.Entities.Delivery delivery, CancellationToken ct = default);
        Task<string> AddAsync(Domain.Entities.Delivery delivery, CancellationToken ct = default);
    }
}
