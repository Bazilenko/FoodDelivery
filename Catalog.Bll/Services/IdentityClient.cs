using System.Net.Http.Json;

using Catalog.Bll.Services.Interfaces;

namespace Catalog.Bll.Services
{
    public class IdentityClient : IIdentityClient
    {
        private readonly HttpClient _httpClient;

        public IdentityClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LinkUserToRestaurantAsync(int userId, int restaurantId, CancellationToken ct = default)
        {
            var payload = new { UserId = userId, RestaurantId = restaurantId };

            var response = await HttpClientJsonExtensions.PostAsJsonAsync(
                _httpClient,
                "users/internal/user-restaurant",
                payload,
                ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                throw new HttpRequestException($"Failed to link user to restaurant. Status: {response.StatusCode}, Error: {error}");
            }
        }
    }
}