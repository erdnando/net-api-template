using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Services.Interfaces;

namespace netapi_template.Services;

/// <summary>
/// Implementación del servicio de utilidades y administración
/// </summary>
public class UtilsService : IUtilsService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UtilsService> _logger;
    private readonly IEmailService _emailService;

    public UtilsService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<UtilsService> logger,
        IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _emailService = emailService;
    }

    // ========================================
    // 🔄 Gestión de Reset de Contraseñas
    // ========================================

    public async Task<UtilsResetPasswordAttemptsResponseDto> ResetPasswordAttemptsAsync(string email, string adminEmail, string clientIp)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return new UtilsResetPasswordAttemptsResponseDto
                {
                    Success = false,
                    Message = "Usuario no encontrado",
                    UserEmail = email
                };
            }

            // Obtener tokens de las últimas 24 horas
            var tokensToDelete = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && t.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .ToListAsync();

            var tokensCount = tokensToDelete.Count;

            if (tokensToDelete.Any())
            {
                _context.PasswordResetTokens.RemoveRange(tokensToDelete);
                await _context.SaveChangesAsync();
            }

            // Log de auditoría
            LogSecurityAudit(new SecurityAuditLogDto
            {
                Action = "PASSWORD_RESET_ATTEMPTS_RESET",
                Description = $"Admin reset password attempts for user: {email}",
                AdminEmail = adminEmail,
                TargetEmail = email,
                ClientIp = clientIp,
                AdditionalData = new Dictionary<string, object>
                {
                    ["tokens_removed"] = tokensCount,
                    ["user_id"] = user.Id
                }
            });

            _logger.LogInformation("Password reset attempts cleared for user {Email} by admin {AdminEmail}. Tokens removed: {Count}", 
                email, adminEmail, tokensCount);

            return new UtilsResetPasswordAttemptsResponseDto
            {
                Success = true,
                Message = tokensCount > 0 
                    ? $"Intentos de reset reiniciados exitosamente. Se eliminaron {tokensCount} tokens."
                    : "No había intentos de reset pendientes para este usuario.",
                TokensRemoved = tokensCount,
                UserEmail = email
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password attempts for user {Email}", email);
            return new UtilsResetPasswordAttemptsResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                UserEmail = email
            };
        }
    }

    public async Task<UtilsPasswordResetStatsDto> GetPasswordResetStatsAsync()
    {
        try
        {
            var maxDaily = _configuration.GetValue<int>("PasswordResetSettings:MaxResetRequestsPerDay");
            _logger.LogInformation("Getting password reset stats with maxDaily: {MaxDaily}", maxDaily);
            
            var since = DateTime.UtcNow.AddDays(-1);
            _logger.LogInformation("Looking for tokens created after: {Since}", since);
            
            // First get the raw data without complex calculations
            var rawStats = await _context.PasswordResetTokens
                .Include(t => t.User)
                .Where(t => t.CreatedAt > since)
                .GroupBy(t => new { t.User!.Email, t.UserId })
                .Select(g => new {
                    Email = g.Key.Email,
                    AttemptCount = g.Count(),
                    LastAttempt = g.Max(t => t.CreatedAt),
                    IsAtLimit = g.Count() >= maxDaily
                })
                .OrderByDescending(s => s.AttemptCount)
                .ToListAsync();

            _logger.LogInformation("Found {Count} raw stats", rawStats.Count);
            
            // Then calculate HoursUntilReset in memory to avoid SQL translation issues
            var userStats = rawStats.Select(s => new UtilsUserPasswordResetStatsDto
            {
                Email = s.Email,
                AttemptCount = s.AttemptCount,
                LastAttempt = s.LastAttempt,
                IsAtLimit = s.IsAtLimit,
                HoursUntilReset = Math.Max(0, 24 - (int)(DateTime.UtcNow - s.LastAttempt).TotalHours)
            }).ToList();
            
            var result = new UtilsPasswordResetStatsDto
            {
                UserStats = userStats,
                TotalUsers = userStats.Count,
                TotalAttempts = userStats.Sum(s => s.AttemptCount),
                UsersAtLimit = userStats.Count(s => s.IsAtLimit),
                GeneratedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("Stats result: TotalUsers={TotalUsers}, TotalAttempts={TotalAttempts}, UsersAtLimit={UsersAtLimit}", 
                result.TotalUsers, result.TotalAttempts, result.UsersAtLimit);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting password reset stats");
            return new UtilsPasswordResetStatsDto
            { 
                UserStats = new List<UtilsUserPasswordResetStatsDto>(),
                GeneratedAt = DateTime.UtcNow
            };
        }
    }

    public async Task<CleanupExpiredTokensResponseDto> CleanupExpiredTokensAsync(string adminEmail, string clientIp)
    {
        try
        {
            var expiredTokens = await _context.PasswordResetTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            var tokenCount = expiredTokens.Count;

            if (expiredTokens.Any())
            {
                _context.PasswordResetTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }

            // Log de auditoría
            LogSecurityAudit(new SecurityAuditLogDto
            {
                Action = "EXPIRED_TOKENS_CLEANUP",
                Description = $"Admin cleaned up {tokenCount} expired password reset tokens",
                AdminEmail = adminEmail,
                ClientIp = clientIp,
                AdditionalData = new Dictionary<string, object>
                {
                    ["tokens_removed"] = tokenCount
                }
            });

            _logger.LogInformation("Cleaned up {Count} expired password reset tokens by admin {AdminEmail}", 
                tokenCount, adminEmail);

            return new CleanupExpiredTokensResponseDto
            {
                Success = true,
                Message = tokenCount > 0 
                    ? $"Se eliminaron {tokenCount} tokens expirados exitosamente."
                    : "No había tokens expirados para limpiar.",
                TokensRemoved = tokenCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired tokens");
            return new CleanupExpiredTokensResponseDto
            {
                Success = false,
                Message = "Error interno del servidor"
            };
        }
    }

    // ========================================
    // 🛡️ Auditoría y Seguridad
    // ========================================

    public bool LogSecurityAudit(SecurityAuditLogDto auditLog)
    {
        try
        {
            _logger.LogInformation("Security Audit: {Action} - {Description} - Admin: {AdminEmail} - IP: {ClientIp}", 
                auditLog.Action, auditLog.Description, auditLog.AdminEmail, auditLog.ClientIp);
            // Aquí podrías guardar en una tabla específica de auditoría si la tienes
            // Por ahora, se registra en logs
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging security audit");
            return false;
        }
    }

    public async Task<List<SecurityAuditLogDto>> GetRecentSecurityLogsAsync(int hours = 24, int limit = 100)
    {
        // Esta implementación dependería de tener una tabla de logs de auditoría
        // Por ahora retorna una lista vacía como placeholder
        await Task.CompletedTask;
        return new List<SecurityAuditLogDto>();
    }

    // ========================================
    // 🔧 Configuración del Sistema
    // ========================================

    public Task<SystemConfigDto> GetSystemConfigAsync()
    {
        var config = new SystemConfigDto
        {
            MaxResetRequestsPerDay = _configuration.GetValue<int>("PasswordResetSettings:MaxResetRequestsPerDay"),
            TokenExpirationMinutes = _configuration.GetValue<int>("PasswordResetSettings:TokenExpirationMinutes"),
            SmtpSimulateMode = _configuration.GetValue<bool>("SmtpSettings:SimulateMode"),
            FrontendUrl = _configuration.GetValue<string>("AppSettings:FrontendBaseUrl") ?? ""
        };
        
        return Task.FromResult(config);
    }

    public async Task<SystemHealthDto> GetSystemHealthAsync()
    {
        var health = new SystemHealthDto();

        try
        {
            // Test database connection
            health.DatabaseConnection = await _context.Database.CanConnectAsync();
            
            // Get system stats - Fix: Use Status enum instead of IsActive computed property
            health.ActiveUsers = await _context.Users.CountAsync(u => u.Status == UserStatus.Active);
            health.PendingResetTokens = await _context.PasswordResetTokens
                .CountAsync(t => t.ExpiresAt > DateTime.UtcNow);
            health.ExpiredTokens = await _context.PasswordResetTokens
                .CountAsync(t => t.ExpiresAt < DateTime.UtcNow);

            // Test email service (basic check)
            health.EmailService = true; // Placeholder - podrías implementar un test real
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking system health");
            health.DatabaseConnection = false;
        }

        return health;
    }

    // ========================================
    // 🔍 Validaciones
    // ========================================

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<string[]> SearchUsersByEmailAsync(string partialEmail)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(partialEmail) || partialEmail.Length < 2)
            {
                return Array.Empty<string>();
            }

            var searchTerm = partialEmail.ToLower().Trim();

            var users = await _context.Users
                .Where(u => u.Email.ToLower().Contains(searchTerm) && 
                           u.Status == UserStatus.Active) // Solo usuarios activos
                .OrderBy(u => u.Email)
                .Take(15) // Limitar a 15 resultados
                .Select(u => u.Email)
                .ToArrayAsync();

            _logger.LogInformation("Searched users with email containing '{SearchTerm}', found {Count} results", 
                searchTerm, users.Length);

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users by email: {PartialEmail}", partialEmail);
            return Array.Empty<string>();
        }
    }
}
