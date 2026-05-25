namespace Auth.DTOs.Requests;
public record ResetPasswordRequest(
    string UserId,
    string Token,
    string NewPassword);