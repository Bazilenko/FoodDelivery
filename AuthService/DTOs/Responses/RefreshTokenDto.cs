namespace Auth.DTOs.Responses;
public record RefreshTokenDto(
    int Id,
    string Token,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime? RevokedAt,
    bool IsActive);