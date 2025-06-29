using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace netapi_template.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly IWebHostEnvironment _environment;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _logger = logger;
        _environment = environment;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        var simulateMode = smtpSettings.GetValue<bool>("SimulateMode");
        
        // Si SimulateMode está explícitamente en false, enviar email real incluso en desarrollo
        if (simulateMode)
        {
            await SimulateEmailAsync(to, subject, body);
            return;
        }

        // Enviar email real
        await SendRealEmailAsync(to, subject, body, smtpSettings);
    }

    private async Task SimulateEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("=== EMAIL SIMULATION ===");
        _logger.LogInformation("To: {Email}", to);
        _logger.LogInformation("Subject: {Subject}", subject);
        _logger.LogInformation("Body: {Body}", body);
        _logger.LogInformation("========================");

        // Simular delay de red
        await Task.Delay(500);
        
        _logger.LogInformation("Email simulated successfully to {Email}", to);
    }

    private async Task SendRealEmailAsync(string to, string subject, string body, IConfigurationSection smtpSettings)
    {
        try
        {
            var host = smtpSettings["Host"] ?? throw new InvalidOperationException("SMTP Host not configured");
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var username = smtpSettings["Username"] ?? throw new InvalidOperationException("SMTP Username not configured");
            var password = smtpSettings["Password"] ?? throw new InvalidOperationException("SMTP Password not configured");
            var fromEmail = smtpSettings["FromEmail"] ?? username;
            var fromName = smtpSettings["FromName"] ?? "Sistema";

            using var message = new MailMessage();
            message.From = new MailAddress(fromEmail, fromName);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var client = new SmtpClient(host, port);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Email}", to);
            throw;
        }
    }
}
