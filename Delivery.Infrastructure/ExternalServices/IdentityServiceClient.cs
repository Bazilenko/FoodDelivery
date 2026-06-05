using Microsoft.Extensions.Logging;  
using System.Net.Http.Json;  

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
 
        // POST /users/{userId}/roles  → AssignRoleRequest { Role = "Courier" }
        public async Task AssignCourierRoleAsync(string userId, CancellationToken ct = default)
        {
            var body = JsonContent.Create(new { role = "Courier" });
            var response = await _http.PostAsync($"users/{userId}/roles", body, ct);
 
            if (!response.IsSuccessStatusCode)
                _logger.LogError(
                    "Identity service returned {Code} when assigning Courier role to user {UserId}",
                    response.StatusCode, userId);
        }
    }
}
