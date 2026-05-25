namespace Auth.DTOs.Requests;
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);