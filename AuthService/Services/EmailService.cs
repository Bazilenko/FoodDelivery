namespace Auth.Services;
 
/// <summary>
/// Mock email service for development — logs to console.
/// Replace with a real provider (SendGrid, MailKit, etc.) in production.
/// </summary>
public class DevEmailService : Auth.Services.Interfaces.IEmailService
{
    private readonly ILogger<DevEmailService> _logger;
    private readonly string _baseUrl;
 
    public DevEmailService(ILogger<DevEmailService> logger, IConfiguration config)
    {
        _logger  = logger;
        _baseUrl = config["App:BaseUrl"] ?? "https://localhost:5001";
    }
 
    public Task SendEmailVerificationAsync(
        string email, string userId, string token, CancellationToken ct = default)
    {
        var link = $"{_baseUrl}/auth/verify-email?userId={userId}&token={Uri.EscapeDataString(token)}";
        _logger.LogInformation(
            "[DEV EMAIL] Verification link for {Email}: {Link}", email, link);
        return Task.CompletedTask;
    }
 
    public Task SendPasswordResetAsync(
        string email, string userId, string token, CancellationToken ct = default)
    {
        var link = $"{_baseUrl}/auth/reset-password?userId={userId}&token={Uri.EscapeDataString(token)}";
        _logger.LogInformation(
            "[DEV EMAIL] Password reset link for {Email}: {Link}", email, link);
        return Task.CompletedTask;
    }
}   