using System.ComponentModel.DataAnnotations;

namespace netapi_template.DTOs;

/// <summary>
/// DTOs para operaciones de utilidad y administración
/// </summary>

// ========================================
// 🔄 Reset de Intentos de Contraseña (Utils)
// ========================================

public class UtilsResetPasswordAttemptsRequestDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string Email { get; set; } = string.Empty;
}

public class UtilsResetPasswordAttemptsResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TokensRemoved { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

// ========================================
// 📊 Estadísticas de Reset (Utils)
// ========================================

public class UtilsPasswordResetStatsDto
{
    public List<UtilsUserPasswordResetStatsDto> UserStats { get; set; } = new();
    public int TotalUsers { get; set; }
    public int TotalAttempts { get; set; }
    public int UsersAtLimit { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class UtilsUserPasswordResetStatsDto
{
    public string Email { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public DateTime? LastAttempt { get; set; }
    public bool IsAtLimit { get; set; }
    public int HoursUntilReset { get; set; }
}

// ========================================
// 🧹 Limpieza de Tokens Expirados
// ========================================

public class CleanupExpiredTokensResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TokensRemoved { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}

// ========================================
// 🛡️ Logs de Seguridad
// ========================================

public class SecurityAuditLogDto
{
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string TargetEmail { get; set; } = string.Empty;
    public string ClientIp { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

// ========================================
// 🔧 Configuración del Sistema
// ========================================

public class SystemConfigDto
{
    public int MaxResetRequestsPerDay { get; set; }
    public int TokenExpirationMinutes { get; set; }
    public bool SmtpSimulateMode { get; set; }
    public string FrontendUrl { get; set; } = string.Empty;
}

// ========================================
// 📈 Estado General del Sistema
// ========================================

public class SystemHealthDto
{
    public bool DatabaseConnection { get; set; }
    public bool EmailService { get; set; }
    public int ActiveUsers { get; set; }
    public int PendingResetTokens { get; set; }
    public int ExpiredTokens { get; set; }
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
}

// ========================================
// 🎯 Respuesta Base para Operaciones Utils
// ========================================

public class UtilsOperationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Data { get; set; } = new();
}

// ========================================
// 🔍 Búsqueda de Usuarios (Utils)
// ========================================

public class UtilsSearchUsersResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UtilsSearchUsersDataDto Data { get; set; } = new();
}

public class UtilsSearchUsersDataDto
{
    public string[] Users { get; set; } = Array.Empty<string>();
}
