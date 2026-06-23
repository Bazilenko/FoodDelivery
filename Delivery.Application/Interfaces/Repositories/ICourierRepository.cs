using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delivery.Domain.Entities;

namespace Delivery.Application.Interfaces.Repositories
{
    public interface ICourierRepository
    {
        Task<Courier?> GetByIdAsync(string id, CancellationToken ct = default);
        Task<Courier?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);
        Task<IEnumerable<Courier>> GetAllAsync(CancellationToken ct = default);
        Task<string> AddAsync(Courier courier, CancellationToken ct = default);
        Task SaveAsync(Courier courier, CancellationToken ct = default);
        Task DeleteAsync(string id, CancellationToken ct = default);
        Task<Courier?> FindByUserIdAsync(string userId, CancellationToken ct);
    }
}
