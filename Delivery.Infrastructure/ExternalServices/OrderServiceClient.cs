using Microsoft.Extensions.Logging;  
using System.Net.Http.Json;  

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
 
        public async Task NotifyPickedUpAsync(int orderId, CancellationToken ct = default)
            => await PutStatusAsync(orderId, "OutForDelivery", ct);
 
        public async Task NotifyDeliveredAsync(int orderId, CancellationToken ct = default)
            => await PutStatusAsync(orderId, "Delivered", ct);
 
        public async Task NotifyFailedAsync(int orderId, string reason, CancellationToken ct = default)
            => await PutStatusAsync(orderId, "Failed", ct);
 
        private async Task PutStatusAsync(int orderId, string status, CancellationToken ct)
        {
            var body = JsonContent.Create(new { status, comment = $"Updated by delivery service." });
            var response = await _http.PutAsync($"api/orders/{orderId}/status", body, ct);
 
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Order service returned {Code} when updating order {OrderId} to {Status}",
                    response.StatusCode, orderId, status);
 
            }
        }
    }
}