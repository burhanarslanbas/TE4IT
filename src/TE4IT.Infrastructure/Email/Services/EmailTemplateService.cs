using Microsoft.Extensions.Hosting;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Infrastructure.Email.Models;

namespace TE4IT.Infrastructure.Email.Services;

/// <summary>
/// Email şablonları için servis
/// </summary>
public sealed class EmailTemplateService : IEmailTemplateService
{
    private readonly IHostEnvironment _environment;
    private readonly string _templatesPath;

    public EmailTemplateService(IHostEnvironment environment)
    {
        _environment = environment;
        
        // Template dosyalarının gerçek yolunu bul
        var currentDir = Directory.GetCurrentDirectory();
        
        // Önce mevcut dizinde ara
        _templatesPath = Path.Combine(currentDir, "src", "TE4IT.Infrastructure", "Email", "Templates");
        
        // Eğer bu yol yoksa, ContentRootPath'te ara
        if (!Directory.Exists(_templatesPath))
        {
            _templatesPath = Path.Combine(_environment.ContentRootPath, "Email", "Templates");
        }
        
        // Eğer hala yoksa, BaseDirectory'de ara (Azure deployment)
        if (!Directory.Exists(_templatesPath))
        {
            _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email", "Templates");
        }
        
        // Debug için yol bilgisini logla
        Console.WriteLine($"Email Templates Path: {_templatesPath}");
        Console.WriteLine($"Directory Exists: {Directory.Exists(_templatesPath)}");
    }

    public string GetPasswordResetTemplate(string resetLink, string email)
    {
        var template = LoadTemplate("Email", "PasswordReset.html");
        return ReplaceVariables(template, new Dictionary<string, object>
        {
            { "ResetLink", resetLink },
            { "Email", email }
        });
    }

    public string GetWelcomeTemplate(string userName, string email, string appUrl)
    {
        var template = LoadTemplate("Email", "Welcome.html");
        return ReplaceVariables(template, new Dictionary<string, object>
        {
            { "UserName", userName },
            { "Email", email },
            { "AppUrl", appUrl }
        });
    }

    /// <summary>
    /// Template dosyasını yükle - Gömülü resource veya dosya sisteminden
    /// </summary>
    private string LoadTemplate(string category, string fileName)
    {
        // Önce dosya sisteminden dene
        var templatePath = Path.Combine(_templatesPath, category, fileName);
        
        if (File.Exists(templatePath))
        {
            return File.ReadAllText(templatePath);
        }

        // Dosya bulunamadıysa, gömülü fallback kullan
        return LoadEmbeddedTemplate(category, fileName);
    }

    /// <summary>
    /// Gömülü template yükle (fallback)
    /// </summary>
    private string LoadEmbeddedTemplate(string category, string fileName)
    {
        var assembly = typeof(EmailTemplateService).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(fileName));

        if (resourceName != null)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        throw new FileNotFoundException($"Email template bulunamadı: {Path.Combine(_templatesPath, category, fileName)}");
    }

    public string GetPasswordChangeNotificationTemplate(string email)
    {
        var template = LoadTemplate("Email", "PasswordChangeNotification.html");
        return ReplaceVariables(template, new Dictionary<string, object>
        {
            { "Email", email },
            { "DateTime", DateTime.Now.ToString("dd.MM.yyyy HH:mm") }
        });
    }

    /// <summary>
    /// Template içindeki değişkenleri değiştir
    /// </summary>
    private string ReplaceVariables(string template, Dictionary<string, object> variables)
    {
        var result = template;
        
        foreach (var variable in variables)
        {
            var placeholder = $"{{{{{variable.Key}}}}}";
            result = result.Replace(placeholder, variable.Value?.ToString() ?? string.Empty);
        }

        return result;
    }
}
