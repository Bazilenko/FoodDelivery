namespace Auth.DTOs.Responses;
public record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,       
    UserDto User);