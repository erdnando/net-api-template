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
        var resetUrl = $"{GetBaseUrl()}/reset-password?token={token}";
        var emailBody = GenerateResetEmailBody(user.FirstName, resetUrl, expirationMinutes);
        
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
            <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Recuperación de Contraseña</h2>
                    <p>Hola {userName},</p>
                    <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p>
                    <p>Para continuar con el proceso, haz clic en el siguiente enlace:</p>
                    <p><a href='{resetUrl}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Restablecer Contraseña</a></p>
                    <p>Este enlace expirará en {expirationMinutes} minutos.</p>
                    <p>Si no solicitaste este cambio, puedes ignorar este correo.</p>
                    <p>Saludos,<br>El Equipo de Soporte</p>
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
        // Obtener URL del frontend desde configuración
        var frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"];
        
        if (!string.IsNullOrEmpty(frontendBaseUrl))
        {
            return frontendBaseUrl;
        }
        
        // Fallback para desarrollo si no está configurado
        return "http://localhost:3000";
    }
}
