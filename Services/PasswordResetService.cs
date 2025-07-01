using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Models;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services;


public class PasswordResetService : IPasswordResetService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ISecurityLoggerService _securityLogger;
    private readonly ILogger<PasswordResetService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PasswordResetService(
        ApplicationDbContext context,
        IEmailService emailService,
        IConfiguration configuration,
        ISecurityLoggerService securityLogger,
        ILogger<PasswordResetService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _emailService = emailService;
        _configuration = configuration;
        _securityLogger = securityLogger;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool success, string message)> InitiatePasswordResetAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);

        if (user == null)
        {
            // Siempre devolver un mensaje genérico para evitar enumerar usuarios
            return (true, "Si el correo existe en nuestro sistema, recibirás un email con instrucciones.");
        }

        if (user.Status != UserStatus.Active)
        {
            _securityLogger.LogSecurityEvent(
                "PASSWORD_RESET_ATTEMPT_INACTIVE_USER",
                $"Password reset attempted for inactive user: {email}",
                GetClientIp());
            return (true, "Si el correo existe en nuestro sistema, recibirás un email con instrucciones.");
        }

        // Verificar límite diario
        var resetCount = await _context.PasswordResetTokens
            .CountAsync(t => t.UserId == user.Id && 
                           t.CreatedAt > DateTime.UtcNow.AddDays(-1));

        var maxDaily = _configuration.GetValue<int>("PasswordResetSettings:MaxResetRequestsPerDay");
        if (resetCount >= maxDaily)
        {
            _securityLogger.LogSecurityEvent(
                "PASSWORD_RESET_LIMIT_EXCEEDED",
                $"Password reset limit exceeded for user: {email}",
                GetClientIp());
            return (false, "Has excedido el límite de solicitudes de reset por día. Intenta mañana.");
        }

        // Generar y guardar token
        var token = GenerateSecureToken();
        var expirationMinutes = _configuration.GetValue<int>("PasswordResetSettings:TokenExpirationMinutes");

        var resetToken = new PasswordResetToken
        {
            Token = token,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        _context.PasswordResetTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        // Enviar email
        var baseUrl = GetBaseUrl();
        var resetUrl = $"{baseUrl}/reset-password?token={token}";
        var emailBody = GenerateResetEmailBody(user.FirstName, resetUrl, expirationMinutes);
        
        // Log para debug
        _logger.LogInformation("Generated reset URL: {ResetUrl}", resetUrl);
        _logger.LogInformation("Base URL from config: {BaseUrl}", baseUrl);
        
        try
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Recuperación de Contraseña",
                emailBody);

            _securityLogger.LogSecurityEvent(
                "PASSWORD_RESET_INITIATED",
                $"Password reset initiated for user: {email}",
                GetClientIp());

            return (true, "Si el correo existe en nuestro sistema, recibirás un email con instrucciones.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", email);
            return (false, "Error al enviar el email. Por favor, intenta más tarde.");
        }
    }

    public async Task<(bool success, string message)> ValidateAndResetPasswordAsync(string token, string newPassword)
    {
        var resetToken = await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

        if (resetToken == null || resetToken.User == null)
        {
            _securityLogger.LogSecurityEvent(
                "INVALID_RESET_TOKEN_ATTEMPT",
                "Invalid password reset token attempt",
                GetClientIp());
            return (false, "Token inválido o expirado.");
        }

        if (!resetToken.IsValid)
        {
            _securityLogger.LogSecurityEvent(
                "EXPIRED_RESET_TOKEN_ATTEMPT",
                $"Expired password reset token attempt for user: {resetToken.User.Email}",
                GetClientIp());
            return (false, "Token inválido o expirado.");
        }

        // Actualizar contraseña
        resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        resetToken.User.UpdatedAt = DateTime.UtcNow;
        
        // Marcar token como usado
        resetToken.UsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _securityLogger.LogSecurityEvent(
            "PASSWORD_RESET_SUCCESSFUL",
            $"Password reset successful for user: {resetToken.User.Email}",
            GetClientIp());

        return (true, "Contraseña actualizada exitosamente.");
    }

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToHexString(randomBytes).ToLower();
    }

    private string GenerateResetEmailBody(string userName, string resetUrl, int expirationMinutes)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Recuperación de Contraseña</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background-color: #f8f9fa; padding: 30px; border-radius: 10px;'>
        <h2 style='color: #2c3e50; text-align: center; margin-bottom: 30px;'>Recuperación de Contraseña</h2>
        
        <p style='font-size: 16px; margin-bottom: 20px;'>Hola <strong>{userName}</strong>,</p>
        
        <p style='font-size: 16px; margin-bottom: 20px;'>
            Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.
        </p>
        
        <p style='font-size: 16px; margin-bottom: 30px;'>
            Para continuar con el proceso, haz clic en el siguiente botón:
        </p>
        
        <div style='text-align: center; margin: 40px 0;'>
            <a href='{resetUrl}' 
               style='display: inline-block; 
                      background-color: #4CAF50; 
                      color: white !important; 
                      padding: 15px 30px; 
                      text-decoration: none; 
                      border-radius: 5px; 
                      font-size: 16px; 
                      font-weight: bold;
                      text-align: center;'>
                Restablecer Contraseña
            </a>
        </div>
        
        <p style='font-size: 14px; margin-bottom: 20px; color: #666;'>
            Si el botón no funciona, puedes copiar y pegar el siguiente enlace en tu navegador:
        </p>
        
        <p style='font-size: 14px; margin-bottom: 30px; word-break: break-all; background-color: #f1f1f1; padding: 10px; border-radius: 5px;'>
            <a href='{resetUrl}' style='color: #007bff; text-decoration: underline;'>{resetUrl}</a>
        </p>
        
        <p style='font-size: 14px; margin-bottom: 20px; color: #e74c3c;'>
            <strong>⚠️ Este enlace expirará en {expirationMinutes} minutos.</strong>
        </p>
        
        <p style='font-size: 14px; margin-bottom: 30px; color: #666;'>
            Si no solicitaste este cambio, puedes ignorar este correo. Tu contraseña no será modificada.
        </p>
        
        <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
        
        <p style='font-size: 14px; color: #666; text-align: center;'>
            Saludos,<br>
            <strong>El Equipo de Soporte</strong><br>
            Sistema V1.0
        </p>
    </div>
</body>
</html>";
    }

    private string GetClientIp()
    {
        if (_httpContextAccessor.HttpContext == null) return "unknown";
        return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string GetBaseUrl()
    {
        // Intentar obtener directamente de la variable de entorno
        var frontendBaseUrl = Environment.GetEnvironmentVariable("FRONTEND_BASE_URL");
        
        _logger.LogInformation("Frontend base URL from environment: {FrontendBaseUrl}", frontendBaseUrl ?? "NULL");
        
        if (!string.IsNullOrEmpty(frontendBaseUrl))
        {
            return frontendBaseUrl;
        }
        
        // Fallback: intentar desde configuración
        frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"];
        _logger.LogInformation("Frontend base URL from config: {FrontendBaseUrl}", frontendBaseUrl ?? "NULL");
        
        if (!string.IsNullOrEmpty(frontendBaseUrl) && !frontendBaseUrl.StartsWith("${"))
        {
            return frontendBaseUrl;
        }
        
        // Fallback final para desarrollo
        _logger.LogWarning("Frontend base URL not configured, using fallback: http://localhost:3000");
        return "http://localhost:3000";
    }
}
