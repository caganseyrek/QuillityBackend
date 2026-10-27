using Microsoft.Extensions.Configuration;
using Quillity.Common;
using System.Net;
using System.Net.Mail;

namespace Quillity.Infrastructure;

public class EmailSender
{
    private readonly SmtpClient _smtpClient;
    private readonly EmailSenderConfig _config;

    public EmailSender(IConfiguration configuration)
    {
        _config = new EmailSenderConfig(configuration);
        _smtpClient = new SmtpClient
        {
            Host = _config.Host,
            Port = _config.Port,
            EnableSsl = _config.EnableSsl,
            Credentials = new NetworkCredential(_config.UserName, _config.Password)
        };
    }

    public async Task SendAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config.UserName),
            Subject = emailMessage.Subject,
            Body = emailMessage.Body,
            IsBodyHtml = emailMessage.IsBodyHtml
        };
        mailMessage.To.Add(emailMessage.To);

        await _smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}

internal class EmailSenderConfig
{
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public EmailSenderConfig(IConfiguration configuration)
    {
        Host = configuration["EmailSmtp:Host"] ?? throw new Exception("Email host value is invalid");
        Port = int.Parse(configuration["EmailSmtp:Port"] ?? throw new Exception("Email port value is invalid"));
        EnableSsl = bool.Parse(configuration["EmailSmtp:EnableSsl"] ?? throw new Exception("Email SSL value is invalid"));
        UserName = configuration["EmailSmtp:Username"] ?? throw new Exception("Email username value is invalid");
        Password = configuration["EmailSmtp:Password"] ?? throw new Exception("Email password value is invalid");
    }
}