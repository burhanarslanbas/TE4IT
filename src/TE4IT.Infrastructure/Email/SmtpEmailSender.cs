using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Infrastructure.Options;

namespace TE4IT.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions _opt = options.Value;

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        using var client = new SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.EnableSsl,
            Credentials = new NetworkCredential(_opt.Username, _opt.Password)
        };

        using var msg = new MailMessage
        {
            From = new MailAddress(_opt.From),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        msg.To.Add(to);

        await client.SendMailAsync(msg, ct);
    }
}