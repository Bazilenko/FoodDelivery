using Auth.Entities;
using Auth.Services.Interfaces;
using Auth.Context;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using AuthService.Entities;

 
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly ApplicationDbContext _db;
    private readonly SymmetricSecurityKey _signingKey;
 
    public JwtTokenService(IOptions<JwtSettings> settings, ApplicationDbContext db)
    {
        _settings = settings.Value;
        _db = db;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
    }
 
    public string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles, int? restaurantId = null)
{
    var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email!),
        new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
        new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
    };

    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    if (restaurantId.HasValue)
    {
        claims.Add(new Claim("restaurant_id", restaurantId.Value.ToString()));
    }

    var token = new JwtSecurityToken(
        issuer:   _settings.Issuer,
        audience: _settings.Audience,
        claims:   claims,
        notBefore: DateTime.UtcNow,
        expires:  DateTime.UtcNow.AddMinutes(_settings.AccessTokenLifetimeMinutes),
        signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(token);
}
 
    public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId, CancellationToken ct = default)
    {
        var token = new RefreshToken
        {
            UserId    = userId,
            Token     = GenerateSecureToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays),
            CreatedAt = DateTime.UtcNow
        };
 
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync(ct);
        return token;
    }
 
    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default)
        => await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, ct);
 
    public async Task RevokeRefreshTokenAsync(
        RefreshToken token, string reason, string? replacedBy = null, CancellationToken ct = default)
    {
        token.RevokedAt      = DateTime.UtcNow;
        token.RevokedReason  = reason;
        token.ReplacedByToken = replacedBy;
        await _db.SaveChangesAsync(ct);
    }
 
    public async Task RevokeAllUserRefreshTokensAsync(int userId, string reason, CancellationToken ct = default)
    {
        var activeTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);
 
        foreach (var token in activeTokens)
        {
            token.RevokedAt     = DateTime.UtcNow;
            token.RevokedReason = reason;
        }
 
        await _db.SaveChangesAsync(ct);
    }
 
    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = _signingKey,
                ValidateIssuer           = true,
                ValidIssuer              = _settings.Issuer,
                ValidateAudience         = true,
                ValidAudience            = _settings.Audience,
                ValidateLifetime         = false  // allow expired tokens for refresh flow
            };
 
            return new JwtSecurityTokenHandler()
                .ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }
 
    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}