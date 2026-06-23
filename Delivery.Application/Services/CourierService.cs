using Delivery.Domain.Entities;
using Delivery.Application.DTOs;
using Delivery.Application.Interfaces.Repositories;
using Delivery.Domain.Interfaces.Services;

namespace Delivery.Application.Services
{
    public class CourierService : ICourierService
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IDeliveryRepository _deliveryRepository;

        public CourierService(ICourierRepository courierRepository, IDeliveryRepository deliveryRepository)
        {
            _courierRepository = courierRepository;
            _deliveryRepository = deliveryRepository;
        }

        public async Task<Courier> GetCourierByIdAsync(string id, CancellationToken ct = default)
        {
            return await _courierRepository.GetByIdAsync(id, ct)
                ?? throw new Exception($"Courier not found: {id}");
        }

        public async Task<Courier> CreateCourierAsync(string name, string email, string phoneNumber, string userId, CancellationToken ct = default)
        {
            var courier = new Courier(name, email, phoneNumber, userId);
            await _courierRepository.AddAsync(courier, ct);
            return courier;
        }

        public async Task UpdateCourierAsync(string id, string name, string email, string phoneNumber, CancellationToken ct = default)
        {
            var courier = await GetCourierByIdAsync(id, ct);
            courier.Update(name, email, phoneNumber);
            await _courierRepository.SaveAsync(courier, ct);
        }

        public async Task DeleteCourierAsync(string id, CancellationToken ct = default)
        {
            await _courierRepository.DeleteAsync(id, ct);
        }

        public async Task<CourierStatsDto> GetStatsAsync(string courierId, CancellationToken ct = default)
        {
            var deliveries = await _deliveryRepository.GetByCourierIdAsync(courierId, ct);
            
            var completed = deliveries.Where(d => d.Status == Domain.Enums.DeliveryStatus.Delivered).ToList();
            var today = DateTime.UtcNow.Date;
            var todayDeliveries = completed.Where(d => d.DeliveredAt?.Date == today).ToList();
            
            var avgTime = 0.0;
            if (completed.Any())
            {
                var times = completed
                    .Where(d => d.PickedUpAt.HasValue && d.DeliveredAt.HasValue)
                    .Select(d => (d.DeliveredAt.Value - d.PickedUpAt.Value).TotalMinutes)
                    .ToList();
                
                if (times.Any())
                    avgTime = Math.Round(times.Average());
            }
            
            return new CourierStatsDto
            {
                TotalDeliveries = completed.Count,
                TotalEarned = completed.Sum(d => d.DeliveryFee),
                ActiveDeliveries = deliveries.Count(d => d.Status == Domain.Enums.DeliveryStatus.InTransit),
                AverageDeliveryTimeMinutes = avgTime,
                TodayDeliveries = todayDeliveries.Count,
                TodayEarned = todayDeliveries.Sum(d => d.DeliveryFee)
            };
        }
    }
}
