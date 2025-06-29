using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netapi_template.Data;
using netapi_template.Models;

namespace netapi_template.Controllers;

/// <summary>
/// Controller para diagnósticos y monitoreo del sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,admin")]
public class DiagnosticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DiagnosticsController> _logger;
    private readonly IWebHostEnvironment _environment;

    public DiagnosticsController(
        ApplicationDbContext context, 
        ILogger<DiagnosticsController> logger,
        IWebHostEnvironment environment)
    {
        _context = context;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Obtiene el estado actual de la base de datos (solo para admins)
    /// </summary>
    [HttpGet("database-state")]
    public async Task<IActionResult> GetDatabaseState()
    {
        try
        {
            var result = new
            {
                DatabaseConnection = await _context.Database.CanConnectAsync(),
                Users = new
                {
                    Total = await _context.Users.CountAsync(),
                    Active = await _context.Users.CountAsync(u => u.Status == UserStatus.Active),
                    ByStatus = await _context.Users
                        .GroupBy(u => u.Status)
                        .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                        .ToListAsync(),
                    NotDeleted = await _context.Users.CountAsync(u => !u.IsDeleted)
                },
                PasswordResetTokens = new
                {
                    Total = await _context.PasswordResetTokens.CountAsync(),
                    Last24Hours = await _context.PasswordResetTokens
                        .CountAsync(t => t.CreatedAt > DateTime.UtcNow.AddDays(-1)),
                    Expired = await _context.PasswordResetTokens
                        .CountAsync(t => t.ExpiresAt < DateTime.UtcNow),
                    Valid = await _context.PasswordResetTokens
                        .CountAsync(t => t.ExpiresAt > DateTime.UtcNow && !t.UsedAt.HasValue)
                },
                Roles = await _context.Roles
                    .Where(r => !r.IsDeleted)
                    .Select(r => new { r.Id, r.Name, UserCount = r.Users.Count })
                    .ToListAsync(),
                Timestamp = DateTime.UtcNow
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database state");
            return StatusCode(500, new { Error = "Error al obtener el estado de la base de datos" });
        }
    }

    /// <summary>
    /// Endpoint de salud básico (disponible sin autenticación)
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHealthStatus()
    {
        try
        {
            var dbConnection = await _context.Database.CanConnectAsync();
            var status = dbConnection ? "Healthy" : "Unhealthy";
            
            var response = new
            {
                Status = status,
                DatabaseConnection = dbConnection,
                Timestamp = DateTime.UtcNow,
                Environment = _environment.EnvironmentName
            };

            return dbConnection ? Ok(response) : StatusCode(503, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health status");
            return StatusCode(503, new { 
                Status = "Unhealthy", 
                Error = "Health check failed",
                Timestamp = DateTime.UtcNow 
            });
        }
    }
}
