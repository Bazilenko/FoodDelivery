namespace Auth.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string userId, string token, CancellationToken ct = default);
    Task SendPasswordResetAsync(string email, string userId, string token, CancellationToken ct = default);
}