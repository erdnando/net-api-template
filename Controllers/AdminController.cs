using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Services.Interfaces;

namespace netapi_template.Controllers;

/// <summary>
/// Controller para operaciones administrativas de seguridad
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Solo usuarios autenticados
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ApplicationDbContext context,
        ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Reinicia el contador de intentos de reset para un usuario específico
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("reset-password-attempts/{email}")]
    public async Task<IActionResult> ResetPasswordAttempts(string email)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            // Eliminar todos los tokens de reset de las últimas 24 horas para este usuario
            var tokensToDelete = await _context.PasswordResetTokens
                .Where(t => t.UserId == user.Id && t.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .ToListAsync();

            if (tokensToDelete.Any())
            {
                _context.PasswordResetTokens.RemoveRange(tokensToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin reset password attempts for user {Email}. Tokens removed: {Count}", 
                    email, tokensToDelete.Count);

                _logger.LogInformation("Password reset attempts cleared for user {Email}. Tokens removed: {Count}", 
                    email, tokensToDelete.Count);

                return Ok(new { 
                    message = $"Intentos de reset reiniciados para {email}", 
                    tokensRemoved = tokensToDelete.Count 
                });
            }

            return Ok(new { message = "No había intentos de reset pendientes para este usuario" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password attempts for user {Email}", email);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Limpia todos los tokens de reset expirados del sistema
    /// </summary>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("cleanup-expired-tokens")]
    public async Task<IActionResult> CleanupExpiredTokens()
    {
        try
        {
            var expiredTokens = await _context.PasswordResetTokens
                .Where(t => t.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _context.PasswordResetTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin cleaned up {Count} expired password reset tokens", expiredTokens.Count);

                _logger.LogInformation("Cleaned up {Count} expired password reset tokens", expiredTokens.Count);

                return Ok(new { 
                    message = "Tokens expirados eliminados", 
                    tokensRemoved = expiredTokens.Count 
                });
            }

            return Ok(new { message = "No había tokens expirados para limpiar" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired tokens");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de intentos de reset por usuario
    /// </summary>
    /// <returns>Estadísticas de reset attempts</returns>
    [HttpGet("password-reset-stats")]
    public async Task<IActionResult> GetPasswordResetStats()
    {
        try
        {
            var stats = await _context.PasswordResetTokens
                .Include(t => t.User)
                .Where(t => t.CreatedAt > DateTime.UtcNow.AddDays(-1))
                .GroupBy(t => t.User!.Email)
                .Select(g => new
                {
                    Email = g.Key,
                    AttemptCount = g.Count(),
                    LastAttempt = g.Max(t => t.CreatedAt)
                })
                .OrderByDescending(s => s.AttemptCount)
                .ToListAsync();

            return Ok(new { 
                message = "Estadísticas de intentos de reset en las últimas 24 horas",
                stats = stats,
                totalUsers = stats.Count,
                totalAttempts = stats.Sum(s => s.AttemptCount)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting password reset stats");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
