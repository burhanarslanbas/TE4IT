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
    private readonly string? _templatesPath;

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
            _templatesPath = Path.Combine(_environment.ContentRootPath, "..", "TE4IT.Infrastructure", "Email", "Templates");
        }
        
        // Eğer hala yoksa, BaseDirectory'de ara
        if (!Directory.Exists(_templatesPath))
        {
            _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TE4IT.Infrastructure", "Email", "Templates");
        }
        
        // Son çare: Embedded resource olarak kullan
        if (!Directory.Exists(_templatesPath))
        {
            _templatesPath = null; // Embedded resource kullanacağız
        }
        
        // Debug için yol bilgisini logla
        Console.WriteLine($"Email Templates Path: {_templatesPath}");
        Console.WriteLine($"Directory Exists: {_templatesPath != null && Directory.Exists(_templatesPath)}");
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
    /// Template dosyasını yükle
    /// </summary>
    private string LoadTemplate(string category, string fileName)
    {
        // Eğer template path null ise, embedded resource kullan
        if (_templatesPath == null)
        {
            return LoadEmbeddedTemplate(category, fileName);
        }
        
        var templatePath = Path.Combine(_templatesPath, category, fileName);
        
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Email template bulunamadı: {templatePath}");
        }

        return File.ReadAllText(templatePath);
    }

    /// <summary>
    /// Embedded resource'dan template yükle
    /// </summary>
    private string LoadEmbeddedTemplate(string category, string fileName)
    {
        // Fallback olarak basit HTML template'ler döndür
        if (fileName == "PasswordReset.html")
        {
            return GetPasswordResetFallbackTemplate();
        }
        
        if (fileName == "Welcome.html")
        {
            return GetWelcomeFallbackTemplate();
        }
        
        throw new FileNotFoundException($"Embedded template bulunamadı: {category}/{fileName}");
    }

    /// <summary>
    /// Password Reset için fallback template
    /// </summary>
    private string GetPasswordResetFallbackTemplate()
    {
        return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <title>Şifre Sıfırlama - TE4IT</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; margin: -30px -30px 20px -30px; }
        .button { display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; }
        .footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 14px; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🚀 TE4IT</h1>
            <p>Görev Yönetim Platformu</p>
        </div>
        
        <h2>Şifrenizi Sıfırlayın</h2>
        <p>Merhaba!</p>
        <p>TE4IT hesabınız için şifre sıfırlama talebinde bulundunuz. Aşağıdaki butona tıklayarak yeni şifrenizi belirleyebilirsiniz.</p>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""{{ResetLink}}"" class=""button"">Şifremi Sıfırla</a>
        </div>
        
        <div style=""background-color: #f8f9fa; border-left: 4px solid #28a745; padding: 15px; margin: 20px 0;"">
            <strong>🔒 Güvenlik Notu:</strong><br>
            Bu link 1 saat boyunca geçerlidir. Eğer bu talebi siz yapmadıysanız, bu emaili görmezden gelebilirsiniz.
        </div>
        
        <div class=""footer"">
            <p>Bu email otomatik olarak gönderilmiştir. Lütfen yanıtlamayın.</p>
            <p>© 2024 TE4IT. Tüm hakları saklıdır.</p>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Welcome için fallback template
    /// </summary>
    private string GetWelcomeFallbackTemplate()
    {
        return @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <title>Hoş Geldiniz - TE4IT</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }
        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; margin: -30px -30px 20px -30px; }
        .button { display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px 30px; text-decoration: none; border-radius: 25px; font-weight: bold; }
        .trial-info { background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%); padding: 20px; border-radius: 10px; margin: 20px 0; text-align: center; }
        .footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 14px; text-align: center; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🚀 TE4IT</h1>
            <p>Görev Yönetim Platformu</p>
        </div>
        
        <h2>Hoş Geldiniz!</h2>
        <p>Merhaba {{UserName}}!</p>
        <p>TE4IT ailesine katıldığınız için teşekkür ederiz. Görev yönetimi deneyiminiz başlıyor!</p>
        
        <div class=""trial-info"">
            <h3>🎉 Ücretsiz Deneme Sürümü</h3>
            <p>14 gün boyunca tüm premium özellikleri ücretsiz deneyebilirsiniz!</p>
        </div>
        
        <div style=""text-align: center; margin: 30px 0;"">
            <a href=""{{AppUrl}}"" class=""button"">Hemen Başlayın</a>
        </div>
        
        <div class=""footer"">
            <p>Bu email otomatik olarak gönderilmiştir.</p>
            <p>© 2024 TE4IT. Tüm hakları saklıdır.</p>
        </div>
    </div>
</body>
</html>";
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
