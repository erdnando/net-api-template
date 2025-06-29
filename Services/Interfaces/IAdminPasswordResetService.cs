using netapi_template.DTOs;

namespace netapi_template.Services.Interfaces;

/// <summary>
/// Servicio para administración de intentos de reset de contraseña
/// </summary>
public interface IAdminPasswordResetService
{
    /// <summary>
    /// Resetea los intentos de reset de contraseña para un usuario específico
    /// </summary>
    /// <param name="userEmail">Email del usuario</param>
    /// <param name="adminUserId">ID del administrador que realiza la acción</param>
    /// <param name="reason">Razón opcional del reseteo</param>
    /// <returns>Resultado de la operación</returns>
    Task<ResetPasswordAttemptsResponseDto> ResetUserPasswordAttemptsAsync(string userEmail, int adminUserId, string? reason = null);

    /// <summary>
    /// Obtiene estadísticas de intentos de reset para usuarios específicos
    /// </summary>
    /// <param name="request">Solicitud con parámetros de filtro</param>
    /// <returns>Estadísticas de intentos de reset</returns>
    Task<PasswordResetStatsResponseDto> GetPasswordResetStatsAsync(GetPasswordResetStatsRequestDto request);

    /// <summary>
    /// Limpia todos los tokens de reset expirados del sistema
    /// </summary>
    /// <param name="adminUserId">ID del administrador que realiza la acción</param>
    /// <returns>Resultado de la operación de limpieza</returns>
    Task<CleanExpiredTokensResponseDto> CleanExpiredTokensAsync(int adminUserId);

    /// <summary>
    /// Obtiene estadísticas de un usuario específico
    /// </summary>
    /// <param name="userEmail">Email del usuario</param>
    /// <returns>Estadísticas del usuario o null si no existe</returns>
    Task<PasswordResetStatsDto?> GetUserPasswordResetStatsAsync(string userEmail);
}
