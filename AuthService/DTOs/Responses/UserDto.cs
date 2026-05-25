namespace Auth.DTOs.Responses;
public record UserDto(
    int Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    bool EmailConfirmed,
    DateTime CreatedAt,
    IEnumerable<string> Roles);