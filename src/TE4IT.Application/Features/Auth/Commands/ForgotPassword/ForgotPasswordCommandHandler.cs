using MediatR;
using System.Net;
using TE4IT.Application.Abstractions.Auth;
using TE4IT.Application.Abstractions.Email;
using TE4IT.Application.Abstractions.Common;

namespace TE4IT.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserAccountService accounts,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplate,
    IUrlService urlService) : IRequestHandler<ForgotPasswordCommand, ForgotPasswordCommandResponse>
{
    public async Task<ForgotPasswordCommandResponse> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return new ForgotPasswordCommandResponse(true, "İşlem tamamlandı.");

        var token = await accounts.GeneratePasswordResetTokenAsync(request.Email, ct);
        if (string.IsNullOrEmpty(token))
            return new ForgotPasswordCommandResponse(true, "İşlem tamamlandı.");

        var tokenEncoded = WebUtility.UrlEncode(token);
        
        // Frontend URL'ini environment-aware olarak al
        var frontendUrl = urlService.GetFrontendUrl();
        var resetLink = $"{frontendUrl}/reset-password?email={WebUtility.UrlEncode(request.Email)}&token={tokenEncoded}";
        
        // Güzel email şablonu kullan
        var htmlBody = emailTemplate.GetPasswordResetTemplate(resetLink, request.Email);
        
        await emailSender.SendAsync(request.Email, "TE4IT - Şifre Sıfırlama", htmlBody, ct);
        
        return new ForgotPasswordCommandResponse(true, "Şifre sıfırlama linki gönderildi.");
    }
}
