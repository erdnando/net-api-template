using netapi_template.DTOs;

namespace netapi_template.Services.Interfaces;

/// <summary>
/// Servicio para operaciones de utilidad y administración del sistema
/// </summary>
public interface IUtilsService
{
    // ========================================
    // 🔄 Gestión de Reset de Contraseñas
    // ========================================
    
    /// <summary>
    /// Reinicia el contador de intentos de reset para un usuario específico
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <param name="adminEmail">Email del administrador que ejecuta la acción</param>
    /// <param name="clientIp">IP del cliente</param>
    /// <returns>Resultado de la operación</returns>
    Task<UtilsResetPasswordAttemptsResponseDto> ResetPasswordAttemptsAsync(string email, string adminEmail, string clientIp);

    /// <summary>
    /// Obtiene estadísticas de intentos de reset de contraseña
    /// </summary>
    /// <returns>Estadísticas detalladas</returns>
    Task<UtilsPasswordResetStatsDto> GetPasswordResetStatsAsync();

    /// <summary>
    /// Limpia tokens de reset expirados del sistema
    /// </summary>
    /// <param name="adminEmail">Email del administrador</param>
    /// <param name="clientIp">IP del cliente</param>
    /// <returns>Resultado de la limpieza</returns>
    Task<CleanupExpiredTokensResponseDto> CleanupExpiredTokensAsync(string adminEmail, string clientIp);

    // ========================================
    // 🛡️ Auditoría y Seguridad
    // ========================================
    
    /// <summary>
    /// Registra una acción administrativa en el log de seguridad
    /// </summary>
    /// <param name="auditLog">Datos del log de auditoría</param>
    /// <returns>True si se registró correctamente</returns>
    Task<bool> LogSecurityAuditAsync(SecurityAuditLogDto auditLog);

    /// <summary>
    /// Obtiene logs de seguridad recientes
    /// </summary>
    /// <param name="hours">Horas hacia atrás a consultar</param>
    /// <param name="limit">Límite de registros</param>
    /// <returns>Lista de logs de seguridad</returns>
    Task<List<SecurityAuditLogDto>> GetRecentSecurityLogsAsync(int hours = 24, int limit = 100);

    // ========================================
    // 🔧 Configuración del Sistema
    // ========================================
    
    /// <summary>
    /// Obtiene la configuración actual del sistema
    /// </summary>
    /// <returns>Configuración del sistema</returns>
    Task<SystemConfigDto> GetSystemConfigAsync();

    /// <summary>
    /// Obtiene el estado de salud del sistema
    /// </summary>
    /// <returns>Estado de salud del sistema</returns>
    Task<SystemHealthDto> GetSystemHealthAsync();

    // ========================================
    // 🔍 Validaciones
    // ========================================
    
    /// <summary>
    /// Valida si un usuario existe en el sistema
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <returns>True si el usuario existe</returns>
    Task<bool> UserExistsAsync(string email);

    /// <summary>
    /// Busca usuarios por email parcial para autocomplete
    /// </summary>
    /// <param name="partialEmail">Email parcial para buscar</param>
    /// <returns>Array de emails que coinciden</returns>
    Task<string[]> SearchUsersByEmailAsync(string partialEmail);

    /// <summary>
    /// Valida si un usuario tiene permisos de administrador
    /// </summary>
    /// <param name="email">Email del usuario</param>
    /// <returns>True si es administrador</returns>
    Task<bool> IsAdminUserAsync(string email);
}
