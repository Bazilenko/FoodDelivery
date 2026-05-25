namespace Auth.DTOs.Requests;
public record VerifyEmailRequest(
    string UserId,
    string Token);