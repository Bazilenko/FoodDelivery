using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Catalog.Dal.Context;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories.Interfaces;
using Catalog.Dal.Specifications.Interfaces;
using Catalog.Dal.Specifications;

namespace Catalog.Dal.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly MyDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;
        public GenericRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet.ToListAsync(ct);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _dbSet.FindAsync(id, ct);
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
        }

        public async Task<T?> GetEntityWithSpecification(ISpecification<T> specification)
        {
            return await SpecificationEvaluator<T>
                .GetQuery(_dbSet, specification)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> ListAsync(ISpecification<T> specification)
        {
            return await SpecificationEvaluator<T>
                .GetQuery(_dbSet, specification)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _dbSet.AddRangeAsync(entities, ct);
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(
            int pageNumber,
            int pageSize,
            Expression<Func<T, TKey>> orderBy,
            bool descending = false,
            CancellationToken ct = default)
        {
            var query = descending
                ? _dbSet.OrderByDescending(orderBy)
                : _dbSet.OrderBy(orderBy);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public void MarkDeleted(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
