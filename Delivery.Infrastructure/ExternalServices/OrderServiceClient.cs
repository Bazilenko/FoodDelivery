using Microsoft.Extensions.Logging;  
using System.Net.Http.Json;  
using Delivery.Application.Interfaces.ExternalServices;

namespace Delivery.Infrastructure.ExternalServices
{
    public class OrderServiceClient : IOrderServiceClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<OrderServiceClient> _logger;

        public OrderServiceClient(HttpClient http, ILogger<OrderServiceClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public Task NotifyPickedUpAsync(int orderId, CancellationToken ct = default)
            => PutStatusAsync(orderId, "OutForDelivery", ct);

        public Task NotifyDeliveredAsync(int orderId, CancellationToken ct = default)
            => PutStatusAsync(orderId, "Delivered", ct);

        public Task NotifyFailedAsync(int orderId, string reason, CancellationToken ct = default)
            => PutStatusAsync(orderId, "Failed", ct);

        private async Task PutStatusAsync(int orderId, string status, CancellationToken ct)
        {
            var body = JsonContent.Create(new { status, comment = "Updated by delivery service." });
            var response = await _http.PutAsync($"api/orders/{orderId}/status", body, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Order service returned {Code} for order {Id} → {Status}",
                    response.StatusCode, orderId, status);
            }
        }
    }
}