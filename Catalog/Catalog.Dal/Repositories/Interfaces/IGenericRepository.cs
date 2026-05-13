using System.Linq.Expressions;
using Catalog.Dal.Entities;
using Catalog.Dal.Specifications.Interfaces;

namespace Catalog.Dal.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task<T?> GetEntityWithSpecification(ISpecification<T> specification);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        Task<IEnumerable<T>> ListAsync(ISpecification<T> specification);
       
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(
                int pageNumber,
                int pageSize,
                Expression<Func<T, TKey>> orderBy,
                bool descending = false,
                CancellationToken ct = default);

        /// <summary>
        /// Marks the entity as Deleted in the change tracker without calling SaveChanges.
        /// Use when you want to batch multiple deletes into one SaveChanges round-trip.
        /// </summary>
        void MarkDeleted(T entity);
    }

        
    
}
