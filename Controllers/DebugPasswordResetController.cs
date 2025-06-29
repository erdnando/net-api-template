using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Models;

namespace netapi_template.Controllers;

/// <summary>
/// Debug controller for password reset tokens - ONLY for development
/// </summary>
[ApiController]
[Route("api/debug/password-reset")]
[Produces("application/json")]
public class DebugPasswordResetController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DebugPasswordResetController> _logger;

    public DebugPasswordResetController(
        ApplicationDbContext context, 
        IWebHostEnvironment environment,
        ILogger<DebugPasswordResetController> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Get all password reset tokens - DEBUG ONLY
    /// </summary>
    [HttpGet("tokens")]
    public async Task<IActionResult> GetAllTokens()
    {
        // Solo disponible en desarrollo
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var tokens = await _context.PasswordResetTokens
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Token,
                t.UserId,
                UserEmail = t.User!.Email,
                t.CreatedAt,
                t.ExpiresAt,
                t.UsedAt,
                t.IsUsed,
                t.IsExpired,
                t.IsValid,
                MinutesUntilExpiration = t.IsExpired ? 0 : (int)(t.ExpiresAt - DateTime.UtcNow).TotalMinutes
            })
            .ToListAsync();

        return Ok(new { 
            message = "DEBUG: Password reset tokens",
            count = tokens.Count,
            tokens = tokens
        });
    }

    /// <summary>
    /// Get reset tokens for a specific user - DEBUG ONLY
    /// </summary>
    [HttpGet("tokens/user/{email}")]
    public async Task<IActionResult> GetTokensForUser(string email)
    {
        // Solo disponible en desarrollo
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
        {
            return NotFound($"User with email {email} not found");
        }

        var tokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == user.Id)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Token,
                t.CreatedAt,
                t.ExpiresAt,
                t.UsedAt,
                t.IsUsed,
                t.IsExpired,
                t.IsValid,
                MinutesUntilExpiration = t.IsExpired ? 0 : (int)(t.ExpiresAt - DateTime.UtcNow).TotalMinutes,
                ResetUrl = $"{Request.Scheme}://{Request.Host}/reset-password?token={t.Token}"
            })
            .ToListAsync();

        return Ok(new { 
            message = $"DEBUG: Password reset tokens for {email}",
            userEmail = email,
            count = tokens.Count,
            tokens = tokens
        });
    }

    /// <summary>
    /// Clean expired tokens - DEBUG ONLY
    /// </summary>
    [HttpDelete("tokens/expired")]
    public async Task<IActionResult> CleanExpiredTokens()
    {
        // Solo disponible en desarrollo
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var expiredTokens = await _context.PasswordResetTokens
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        _context.PasswordResetTokens.RemoveRange(expiredTokens);
        var deletedCount = await _context.SaveChangesAsync();

        _logger.LogInformation("DEBUG: Deleted {Count} expired password reset tokens", deletedCount);

        return Ok(new { 
            message = "DEBUG: Expired tokens cleaned",
            deletedCount = deletedCount
        });
    }
}
