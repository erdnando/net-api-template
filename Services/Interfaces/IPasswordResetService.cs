namespace netapi_template.Services.Interfaces;

public interface IPasswordResetService
{
    Task<(bool success, string message)> InitiatePasswordResetAsync(string email);
    Task<(bool success, string message)> ValidateAndResetPasswordAsync(string token, string newPassword);
}
