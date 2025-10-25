using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;

namespace TE4IT.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserAccountService accounts,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplate) : IRequestHandler<ResetPasswordCommand, ResetPasswordCommandResponse>
{
    public async Task<ResetPasswordCommandResponse> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        // Token ile yeni şifreyi uygula
        var success = await accounts.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, ct);
        if (!success)
        {
            return new ResetPasswordCommandResponse(false, "Şifre sıfırlama başarısız. Token geçersiz veya kullanıcı bulunamadı.");
        }

        // Başarılı şifre sıfırlama email'i gönder
        try
        {
            var htmlBody = emailTemplate.GetPasswordChangeNotificationTemplate(request.Email);
            await emailSender.SendAsync(request.Email, "TE4IT - Şifre Sıfırlama Tamamlandı", htmlBody, ct);
        }
        catch
        {
            // Email gönderimi başarısız olsa bile şifre sıfırlama başarılı
        }

        return new ResetPasswordCommandResponse(true, "Şifre başarıyla güncellendi.");
    }
}