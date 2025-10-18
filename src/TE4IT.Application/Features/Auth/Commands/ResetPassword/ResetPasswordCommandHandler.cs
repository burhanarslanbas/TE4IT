using MediatR;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;

namespace TE4IT.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserAccountService accounts,
    IEmailSender emailSender) : IRequestHandler<ResetPasswordCommand, ResetPasswordCommandResponse>
{
    public async Task<ResetPasswordCommandResponse> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        // Token ile yeni şifreyi uygula
        var success = await accounts.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, ct);
        if (!success)
        {
            return new ResetPasswordCommandResponse(false, "Şifre sıfırlama başarısız. Token geçersiz veya kullanıcı bulunamadı.");
        }

        // İsteğe bağlı: bilgilendirme e-postası
        var html = "<p>Şifreniz başarılı şekilde güncellendi.</p>";
        await emailSender.SendAsync(request.Email, "Şifre Güncelleme", html, ct);

        return new ResetPasswordCommandResponse(true, "Şifre güncellendi.");
    }
}