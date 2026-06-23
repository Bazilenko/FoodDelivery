using Microsoft.Extensions.Logging;  
using System.Net.Http.Json;  
using Delivery.Application.Interfaces.ExternalServices;

namespace Delivery.Infrastructure.ExternalServices
{
    public class IdentityServiceClient : IIdentityServiceClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<IdentityServiceClient> _logger;

        public IdentityServiceClient(HttpClient http, ILogger<IdentityServiceClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task AssignCourierRoleAsync(string userId, CancellationToken ct = default)
        {
            var body = JsonContent.Create(new { role = "Courier" });
            var response = await _http.PostAsync($"users/{userId}/roles", body, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Identity service returned {Code} assigning Courier role to user {Id}",
                    response.StatusCode, userId);
            }
        }
    }
}
