using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.Domain.Common;

namespace Delivery.Domain.Interfaces.Repositories
{
    public interface IRepository <T> where T : BaseEntity
    {
        /// <summary>
        /// Знаходить сутність за її ID.
        /// Увага: Через кінцеву узгодженість, метод може не знайти нещодавно створену сутність.
        /// </summary>
        Task<T?> GetByIdAsync(string id, CancellationToken ct = default);

        Task<T> AddAsync(T entity, CancellationToken ct = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

     


        /// <summary>
        /// Оновлює існуючу сутність.
        /// </summary>
        /// <exception cref="Domain.Exceptions.ConcurrencyException">
        /// Викидається, якщо сутність була змінена іншим процесом.
        /// </exception>
        Task UpdateAsync(T entity, CancellationToken ct = default);

        Task DeleteAsync(string id, CancellationToken ct = default);
    }
}
