using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using netapi_template.Attributes;
using netapi_template.DTOs;
using netapi_template.Models;
using netapi_template.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace netapi_template.Controllers;

/// <summary>
/// Controller para operaciones de utilidad y administración del sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación
public class UtilsController : ControllerBase
{
    private readonly IUtilsService _utilsService;
    private readonly ILogger<UtilsController> _logger;

    public UtilsController(
        IUtilsService utilsService,
        ILogger<UtilsController> logger)
    {
        _utilsService = utilsService;
        _logger = logger;
    }

    // ========================================
    // 🔄 Gestión de Reset de Contraseñas
    // ========================================

    /// <summary>
    /// Reinicia el contador de intentos de reset de contraseña para un usuario específico
    /// </summary>
    /// <param name="request">Datos del usuario a reiniciar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("reset-password-attempts")]
    [SwaggerOperation(
        Summary = "Reiniciar intentos de reset de contraseña",
        Description = "Elimina todos los tokens de reset de contraseña de las últimas 24 horas para un usuario específico. Solo administradores pueden usar esta función."
    )]
    [SwaggerResponse(200, "Intentos reiniciados exitosamente", typeof(UtilsResetPasswordAttemptsResponseDto))]
    [SwaggerResponse(400, "Datos inválidos")]
    [SwaggerResponse(401, "No autorizado")]
    [SwaggerResponse(403, "Permisos insuficientes")]
    [SwaggerResponse(404, "Usuario no encontrado")]
    public async Task<ActionResult<UtilsResetPasswordAttemptsResponseDto>> ResetPasswordAttempts(
        [FromBody] UtilsResetPasswordAttemptsRequestDto request)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            }
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _utilsService.ResetPasswordAttemptsAsync(request.Email, userEmail, clientIp);
            if (result.Success)
            {
                _logger.LogInformation("Password reset attempts cleared for user {TargetEmail} by user {UserEmail}", 
                    request.Email, userEmail);
                return Ok(result);
            }
            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing reset password attempts request");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de intentos de reset de contraseña
    /// </summary>
    /// <returns>Estadísticas detalladas</returns>
    [HttpGet("password-reset-stats")]
    [SwaggerOperation(
        Summary = "Obtener estadísticas de reset de contraseña",
        Description = "Devuelve estadísticas detalladas sobre los intentos de reset de contraseña en las últimas 24 horas"
    )]
    [SwaggerResponse(200, "Estadísticas obtenidas exitosamente", typeof(UtilsPasswordResetStatsDto))]
    [SwaggerResponse(401, "No autorizado")]
    [SwaggerResponse(403, "Permisos insuficientes")]
    public async Task<ActionResult<UtilsPasswordResetStatsDto>> GetPasswordResetStats()
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }
            var stats = await _utilsService.GetPasswordResetStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting password reset stats");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Limpia tokens de reset de contraseña expirados
    /// </summary>
    /// <returns>Resultado de la limpieza</returns>
    [HttpPost("cleanup-expired-tokens")]
    [SwaggerOperation(
        Summary = "Limpiar tokens expirados",
        Description = "Elimina todos los tokens de reset de contraseña que han expirado del sistema"
    )]
    [SwaggerResponse(200, "Limpieza realizada exitosamente", typeof(CleanupExpiredTokensResponseDto))]
    [SwaggerResponse(401, "No autorizado")]
    [SwaggerResponse(403, "Permisos insuficientes")]
    public async Task<ActionResult<CleanupExpiredTokensResponseDto>> CleanupExpiredTokens()
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var result = await _utilsService.CleanupExpiredTokensAsync(userEmail, clientIp);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up expired tokens");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ========================================
    // 🔧 Configuración y Estado del Sistema
    // ========================================

    /// <summary>
    /// Obtiene la configuración actual del sistema
    /// </summary>
    /// <returns>Configuración del sistema</returns>
    [HttpGet("system-config")]
    [SwaggerOperation(
        Summary = "Obtener configuración del sistema",
        Description = "Devuelve la configuración actual del sistema (sin datos sensibles)"
    )]
    [SwaggerResponse(200, "Configuración obtenida exitosamente", typeof(SystemConfigDto))]
    [SwaggerResponse(401, "No autorizado")]
    [SwaggerResponse(403, "Permisos insuficientes")]
    public async Task<ActionResult<SystemConfigDto>> GetSystemConfig()
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }
            var config = await _utilsService.GetSystemConfigAsync();
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system config");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene el estado de salud del sistema
    /// </summary>
    /// <returns>Estado de salud del sistema</returns>
    [HttpGet("system-health")]
    [SwaggerOperation(
        Summary = "Obtener estado de salud del sistema",
        Description = "Devuelve el estado actual de salud del sistema incluyendo conexiones y estadísticas"
    )]
    [SwaggerResponse(200, "Estado obtenido exitosamente", typeof(SystemHealthDto))]
    [SwaggerResponse(401, "No autorizado")]
    [SwaggerResponse(403, "Permisos insuficientes")]
    public async Task<ActionResult<SystemHealthDto>> GetSystemHealth()
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }
            var health = await _utilsService.GetSystemHealthAsync();
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system health");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // ========================================
    // 🔍 Validaciones y Utilities
    // ========================================

    /// <summary>
    /// Verifica si un usuario existe en el sistema
    /// </summary>
    /// <param name="email">Email del usuario a verificar</param>
    /// <returns>True si el usuario existe</returns>
    [HttpGet("user-exists")]
    [SwaggerOperation(
        Summary = "Verificar existencia de usuario",
        Description = "Verifica si un usuario existe en el sistema sin revelar información sensible"
    )]
    [SwaggerResponse(200, "Verificación realizada")]
    [SwaggerResponse(401, "No autorizado")]
    [SwaggerResponse(403, "Permisos insuficientes")]
    public async Task<ActionResult<UtilsOperationResponseDto>> CheckUserExists([FromQuery] string email)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }
            var exists = await _utilsService.UserExistsAsync(email);
            return Ok(new UtilsOperationResponseDto
            {
                Success = true,
                Operation = "USER_EXISTS_CHECK",
                Message = exists ? "Usuario encontrado" : "Usuario no encontrado",
                Data = new Dictionary<string, object>
                {
                    ["exists"] = exists,
                    ["email"] = email
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user existence");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Busca usuarios por email parcial para autocomplete
    /// </summary>
    /// <param name="email">Email parcial para buscar</param>
    /// <returns>Lista de emails que coinciden</returns>
    [HttpGet("search-users")]
    [SwaggerOperation(
        Summary = "Buscar usuarios por email",
        Description = "Busca usuarios por email parcial para funcionalidad de autocomplete. Solo para administradores."
    )]
    [SwaggerResponse(200, "Búsqueda realizada exitosamente", typeof(UtilsSearchUsersResponseDto))]
    [SwaggerResponse(400, "Parámetros inválidos")]
    [SwaggerResponse(401, "No autorizado")]  
    [SwaggerResponse(403, "Permisos insuficientes")]
    public async Task<ActionResult<UtilsSearchUsersResponseDto>> SearchUsers([FromQuery] string email)
    {
        try
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }
            if (string.IsNullOrWhiteSpace(email) || email.Length < 2)
            {
                return BadRequest(new { message = "El email debe tener al menos 2 caracteres" });
            }
            var users = await _utilsService.SearchUsersByEmailAsync(email);
            return Ok(new UtilsSearchUsersResponseDto
            {
                Success = true,
                Message = $"Se encontraron {users.Length} usuarios",
                Data = new UtilsSearchUsersDataDto
                {
                    Users = users
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users by email: {Email}", email);
            return StatusCode(500, new UtilsSearchUsersResponseDto
            {
                Success = false,
                Message = "Error interno del servidor",
                Data = new UtilsSearchUsersDataDto
                {
                    Users = Array.Empty<string>()
                }
            });
        }
    }
}
