namespace Auth.DTOs.Requests;
public record UpdateProfileRequest(
    string? FirstName,
    string? LastName,
    string? Username);