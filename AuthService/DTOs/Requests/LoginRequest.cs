namespace Auth.DTOs.Requests;
public record LoginRequest(
    string Email,
    string Password);