using Delivery.Application.Interfaces.Repositories;
using Delivery.Domain.Enums;
using Delivery.Infrastructure.Mongo;
using MongoDB.Driver;

namespace Delivery.Infrastructure.Repository
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly IMongoCollection<Domain.Entities.Delivery> _collection;

        public DeliveryRepository(MongoDbContext context)
        {
            _collection = context.Deliveries;
        }

        public async Task<Domain.Entities.Delivery?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        public async Task<Domain.Entities.Delivery?> GetByOrderIdAsync(int orderId, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.OrderId, orderId);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<Domain.Entities.Delivery>> GetByStatusAsync(DeliveryStatus status, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.Status, status);
            return await _collection.Find(filter).ToListAsync(ct);
        }

        public async Task<IEnumerable<Domain.Entities.Delivery>> GetByCourierIdAsync(string courierId, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.Courier.Id, courierId);
            return await _collection.Find(filter).ToListAsync(ct);
        }

        public async Task<IEnumerable<Domain.Entities.Delivery>> GetByCourierAndStatusAsync(string courierId, DeliveryStatus status, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.And(
                Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.Courier.Id, courierId),
                Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.Status, status));
            return await _collection.Find(filter).ToListAsync(ct);
        }

        public async Task<int> GetActiveDeliveryCountByCourierAsync(string courierId, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.And(
                Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.Courier.Id, courierId),
                Builders<Domain.Entities.Delivery>.Filter.Nin(d => d.Status, new[] { DeliveryStatus.Delivered, DeliveryStatus.Failed }));
            return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
        }

        public async Task<IEnumerable<Domain.Entities.Delivery>> GetByTimeRangeAsync(DateTime start, DateTime end, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.And(
                Builders<Domain.Entities.Delivery>.Filter.Gte(d => d.CreatedAt, start),
                Builders<Domain.Entities.Delivery>.Filter.Lt(d => d.CreatedAt, end));
            return await _collection.Find(filter).ToListAsync(ct);
        }

        public async Task SaveAsync(Domain.Entities.Delivery delivery, CancellationToken ct = default)
        {
            var filter = Builders<Domain.Entities.Delivery>.Filter.Eq(d => d.Id, delivery.Id);
            await _collection.ReplaceOneAsync(filter, delivery, cancellationToken: ct);
        }

        public async Task<string> AddAsync(Domain.Entities.Delivery delivery, CancellationToken ct = default)
        {
            await _collection.InsertOneAsync(delivery, cancellationToken: ct);
            return delivery.Id;
        }
    }
}
