using Auth.Entities;
using System.Security.Claims;

namespace Auth.Services.Interfaces;
 
public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles, int? restaurantId = null);
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId, CancellationToken ct = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(RefreshToken token, string reason, string? replacedBy = null, CancellationToken ct = default);
    Task RevokeAllUserRefreshTokensAsync(int userId, string reason, CancellationToken ct = default);
    ClaimsPrincipal? ValidateAccessToken(string token);
}