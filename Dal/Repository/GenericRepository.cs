using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Dommel;
using Orders.Dal.Context.Interfaces;
using Orders.Dal.Entities;
using Orders.Dal.Repository.Interfaces;

namespace Orders.Dal.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly IDapperContext _context;
        private readonly string _tableName;

        public GenericRepository(IDapperContext context)
        {
            _context = context;

            var tableAttr = typeof(T).GetCustomAttribute<TableAttribute>();
            _tableName = tableAttr?.Name ?? typeof(T).Name;
        }

        public async Task<int> AddAsync(T entity)
        {
            var id = await _context.Connection.InsertAsync(entity, transaction: _context.Transaction);
            return Convert.ToInt32(id);
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> items)
        {
            int count = 0;
            foreach (var item in items)
            {
                await _context.Connection.InsertAsync(item, transaction: _context.Transaction);
                count++;
            }
            return count;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            await _context.Connection.DeleteAsync(entity, transaction: _context.Transaction);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Connection.GetAllAsync<T>(_context.Transaction);
        }

        public async Task<T> GetAsync(int id)
        {
            var result = await _context.Connection.GetAsync<T>(id, _context.Transaction);

            if (result == null)
                throw new KeyNotFoundException($"{typeof(T).Name} with id [{id}] could not be found.");

            return result;
        }

        public async Task ReplaceAsync(T entity)
        {
            await _context.Connection.UpdateAsync(entity, transaction: _context.Transaction);
        }
    }
}
