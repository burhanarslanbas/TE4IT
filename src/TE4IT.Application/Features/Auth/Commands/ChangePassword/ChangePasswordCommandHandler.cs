using MediatR;
using Microsoft.AspNetCore.Http;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;
using System.Security.Claims;

namespace TE4IT.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IUserAccountService accounts,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplate,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ChangePasswordCommand, ChangePasswordCommandResponse>
{
    public async Task<ChangePasswordCommandResponse> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        // Kullanıcı ID'sini JWT token'dan al
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return new ChangePasswordCommandResponse(false, "Kullanıcı kimliği bulunamadı.");
        }

        // Kullanıcı email'ini al
        var emailClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email);
        if (emailClaim == null)
        {
            return new ChangePasswordCommandResponse(false, "Kullanıcı email'i bulunamadı.");
        }

        var email = emailClaim.Value;

        // Şifreyi değiştir
        var success = await accounts.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, ct);
        if (!success)
        {
            return new ChangePasswordCommandResponse(false, "Şifre değiştirme başarısız. Mevcut şifre yanlış olabilir.");
        }

        // Başarılı şifre değişikliği email'i gönder
        try
        {
            var htmlBody = emailTemplate.GetPasswordChangeNotificationTemplate(email);
            await emailSender.SendAsync(email, "TE4IT - Şifre Değişikliği Bildirimi", htmlBody, ct);
        }
        catch
        {
            // Email gönderimi başarısız olsa bile şifre değişikliği başarılı
        }

        return new ChangePasswordCommandResponse(true, "Şifreniz başarıyla güncellendi.");
    }
}
