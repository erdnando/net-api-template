using System.ComponentModel.DataAnnotations;

namespace netapi_template.DTOs;

/// <summary>
/// DTO para solicitud de reseteo de intentos de reset de contraseña por parte del administrador
/// </summary>
public class ResetPasswordAttemptsRequestDto
{
    [Required(ErrorMessage = "El email del usuario es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string UserEmail { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "La razón no puede exceder los 500 caracteres")]
    public string? Reason { get; set; }
}

/// <summary>
/// DTO para respuesta de reseteo de intentos
/// </summary>
public class ResetPasswordAttemptsResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int TokensRemoved { get; set; }
    public DateTime ActionTimestamp { get; set; }
    public string AdminUser { get; set; } = string.Empty;
}

/// <summary>
/// DTO para obtener estadísticas de intentos de reset
/// </summary>
public class PasswordResetStatsDto
{
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int AttemptsLast24Hours { get; set; }
    public int TotalActiveTokens { get; set; }
    public DateTime? LastResetAttempt { get; set; }
    public bool IsBlocked { get; set; }
    public int MaxAllowedAttempts { get; set; }
}

/// <summary>
/// DTO para solicitud de estadísticas de múltiples usuarios
/// </summary>
public class GetPasswordResetStatsRequestDto
{
    public List<string>? UserEmails { get; set; }
    public bool IncludeAllUsers { get; set; } = false;
    public bool OnlyBlockedUsers { get; set; } = false;
}

/// <summary>
/// DTO para respuesta de estadísticas generales
/// </summary>
public class PasswordResetStatsResponseDto
{
    public List<PasswordResetStatsDto> UserStats { get; set; } = new();
    public int TotalUsersChecked { get; set; }
    public int BlockedUsers { get; set; }
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// DTO para limpiar tokens expirados del sistema
/// </summary>
public class CleanExpiredTokensResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TokensRemoved { get; set; }
    public DateTime ActionTimestamp { get; set; }
    public string AdminUser { get; set; } = string.Empty;
}
