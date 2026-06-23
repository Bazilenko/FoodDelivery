using Delivery.Domain.Entities;
using Delivery.Application.DTOs;

namespace Delivery.Domain.Interfaces.Services
{
    public interface ICourierService
    {
        Task<Courier> GetCourierByIdAsync(string id, CancellationToken ct = default);
        Task<Courier> CreateCourierAsync(string name, string email, string phoneNumber, string userId, CancellationToken ct = default);
        Task UpdateCourierAsync(string id, string name, string email, string phoneNumber, CancellationToken ct = default);
        Task DeleteCourierAsync(string id, CancellationToken ct = default);
        Task<CourierStatsDto> GetStatsAsync(string courierId, CancellationToken ct = default);
    }
}
