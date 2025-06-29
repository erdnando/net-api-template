using Microsoft.AspNetCore.Http;

namespace netapi_template.Services;

public interface ISecurityLoggerService
{
    void LogFailedLogin(string email, string ipAddress, string userAgent = "");
    void LogSuccessfulLogin(string email, string ipAddress, string userAgent = "");
    void LogUnauthorizedAccess(string endpoint, string ipAddress, string userAgent = "");
    void LogPasswordChange(string email, string ipAddress, bool success);
    void LogAccountLockout(string email, string ipAddress);
    void LogSuspiciousActivity(string email, string activity, string ipAddress);
    void LogSecurityEvent(string eventType, string description, string ipAddress);
}

public class SecurityLoggerService : ISecurityLoggerService
{
    private readonly ILogger<SecurityLoggerService> _logger;
    
    public SecurityLoggerService(ILogger<SecurityLoggerService> logger)
    {
        _logger = logger;
    }
    
    public void LogFailedLogin(string email, string ipAddress, string userAgent = "")
    {
        _logger.LogWarning("SECURITY: Failed login attempt for {Email} from {IP} using {UserAgent}", 
            email, ipAddress, userAgent);
    }
    
    public void LogSuccessfulLogin(string email, string ipAddress, string userAgent = "")
    {
        _logger.LogInformation("SECURITY: Successful login for {Email} from {IP} using {UserAgent}", 
            email, ipAddress, userAgent);
    }
    
    public void LogUnauthorizedAccess(string endpoint, string ipAddress, string userAgent = "")
    {
        _logger.LogWarning("SECURITY: Unauthorized access to {Endpoint} from {IP} using {UserAgent}", 
            endpoint, ipAddress, userAgent);
    }
    
    public void LogPasswordChange(string email, string ipAddress, bool success)
    {
        if (success)
        {
            _logger.LogInformation("SECURITY: Password changed successfully for {Email} from {IP}", 
                email, ipAddress);
        }
        else
        {
            _logger.LogWarning("SECURITY: Failed password change attempt for {Email} from {IP}", 
                email, ipAddress);
        }
    }
    
    public void LogAccountLockout(string email, string ipAddress)
    {
        _logger.LogWarning("SECURITY: Account lockout triggered for {Email} from {IP}", 
            email, ipAddress);
    }
    
    public void LogSuspiciousActivity(string email, string activity, string ipAddress)
    {
        _logger.LogWarning("SECURITY: Suspicious activity detected for {Email}: {Activity} from {IP}", 
            email, activity, ipAddress);
    }
    
    public void LogSecurityEvent(string eventType, string description, string ipAddress)
    {
        _logger.LogWarning("SECURITY EVENT: {EventType} - {Description} from {IP}", 
            eventType, description, ipAddress);
    }
}

// Extension method to get client IP address
public static class HttpContextExtensions
{
    public static string GetClientIpAddress(this HttpContext context)
    {
        string? ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ip))
        {
            ip = context.Connection.RemoteIpAddress?.ToString();
        }
        return ip ?? "Unknown";
    }
    
    public static string GetUserAgent(this HttpContext context)
    {
        return context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
    }
}
