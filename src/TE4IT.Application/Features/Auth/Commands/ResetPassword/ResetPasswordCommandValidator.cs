using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-posta adresi zorunludur.")
            .EmailAddress()
            .WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token zorunludur.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Yeni şifre en az 8 karakter olmalıdır.");
    }
}