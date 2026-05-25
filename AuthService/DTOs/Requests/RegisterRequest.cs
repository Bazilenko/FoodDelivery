namespace Auth.DTOs.Requests;
public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string? FirstName,
    string? LastName);