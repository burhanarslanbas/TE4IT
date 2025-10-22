using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Infrastructure.Email.Models;
using TE4IT.Infrastructure.Options;

namespace TE4IT.Infrastructure.Email.Services;

/// <summary>
/// Gelişmiş email gönderim servisi
/// </summary>
public sealed class EmailSenderService : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailSenderService> _logger;

    public EmailSenderService(IOptions<EmailOptions> options, ILogger<EmailSenderService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var emailData = new EmailData
        {
            To = to,
            Subject = subject,
            HtmlBody = htmlBody,
            Headers = new Dictionary<string, string>
            {
                { "X-Mailer", "TE4IT Platform" },
                { "X-Priority", "3" },
                { "X-MSMail-Priority", "Normal" }
            }
        };

        await SendEmailAsync(emailData, ct);
    }

    /// <summary>
    /// Gelişmiş email gönderimi
    /// </summary>
    public async Task SendEmailAsync(EmailData emailData, CancellationToken ct = default)
    {
        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.Username, _options.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = 30000 // 30 saniye timeout
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_options.From, "TE4IT Platform"), // Display name ekle
            Subject = emailData.Subject,
            Body = emailData.HtmlBody,
            IsBodyHtml = true,
            Priority = MailPriority.Normal,
            DeliveryNotificationOptions = DeliveryNotificationOptions.None
        };

        // To adresini ekle
        message.To.Add(emailData.To);

        // Headers ekle
        foreach (var header in emailData.Headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }

        // Text body varsa ekle
        if (!string.IsNullOrEmpty(emailData.TextBody))
        {
            var textView = AlternateView.CreateAlternateViewFromString(emailData.TextBody, null, "text/plain");
            message.AlternateViews.Add(textView);
        }

        try
        {
            _logger.LogInformation("Email gönderiliyor: {To}, Subject: {Subject}", emailData.To, emailData.Subject);
            await client.SendMailAsync(message, ct);
            _logger.LogInformation("Email başarıyla gönderildi: {To}", emailData.To);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "SMTP hatası: {To}, {Message}", emailData.To, ex.Message);
            throw new InvalidOperationException($"Email gönderimi başarısız: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email gönderimi sırasında beklenmeyen hata: {To}, {Message}", emailData.To, ex.Message);
            throw new InvalidOperationException($"Email gönderimi sırasında beklenmeyen hata: {ex.Message}", ex);
        }
    }
}
