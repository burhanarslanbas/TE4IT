using TE4IT.Domain.Enums;

namespace TE4IT.Application.Abstractions.Email;

/// <summary>
/// Email şablonları için interface
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Şifre sıfırlama email şablonu
    /// </summary>
    string GetPasswordResetTemplate(string resetLink, string email);
    
    /// <summary>
    /// Hoş geldin email şablonu
    /// </summary>
    string GetWelcomeTemplate(string userName, string email, string appUrl);
    
    /// <summary>
    /// Şifre değişikliği bildirimi email şablonu
    /// </summary>
    string GetPasswordChangeNotificationTemplate(string email);
    
    /// <summary>
    /// Proje daveti email şablonu
    /// </summary>
    string GetProjectInvitationTemplate(string projectTitle, string inviterName, ProjectRole role, string acceptLink, DateTime expiresAt);
}
