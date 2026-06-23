using Delivery.Domain.Entities;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Infrastructure.Mongo;
using MongoDB.Driver;

namespace Delivery.Infrastructure.Repository
{
    public class CourierRepository : ICourierRepository
    {
        private readonly IMongoCollection<Courier> _collection;

        public CourierRepository(MongoDbContext context)
        {
            _collection = context.Couriers;
        }

        public async Task<Courier?> GetByIdAsync(string id, CancellationToken ct = default)
        {
            var filter = Builders<Courier>.Filter.Eq(c => c.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        public async Task<Courier?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        {
            var filter = Builders<Courier>.Filter.Eq(c => c.PhoneNumber, phoneNumber);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<Courier>> GetAllAsync(CancellationToken ct = default)
        {
            return await _collection.Find(_ => true).ToListAsync(ct);
        }

        public async Task<string> AddAsync(Courier courier, CancellationToken ct = default)
        {
            await _collection.InsertOneAsync(courier, cancellationToken: ct);
            return courier.Id;
        }

        public async Task SaveAsync(Courier courier, CancellationToken ct = default)
        {
            var filter = Builders<Courier>.Filter.Eq(c => c.Id, courier.Id);
            await _collection.ReplaceOneAsync(filter, courier, cancellationToken: ct);
        }

        public async Task DeleteAsync(string id, CancellationToken ct = default)
        {
            var filter = Builders<Courier>.Filter.Eq(c => c.Id, id);
            await _collection.DeleteOneAsync(filter, ct);
        }

        public async Task<Courier?> FindByUserIdAsync(string userId, CancellationToken ct)
        {
            return await _collection
                .Find(c => c.UserId == userId)
                .FirstOrDefaultAsync(ct);
        }
    }
}
