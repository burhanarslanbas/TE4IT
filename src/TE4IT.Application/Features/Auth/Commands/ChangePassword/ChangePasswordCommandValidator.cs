using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Mevcut şifre zorunludur.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Yeni şifre zorunludur.")
            .MinimumLength(8)
            .WithMessage("Yeni şifre en az 8 karakter olmalıdır.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Yeni şifre en az bir küçük harf, bir büyük harf, bir rakam ve bir özel karakter içermelidir.");

        RuleFor(x => x)
            .Must(x => x.CurrentPassword != x.NewPassword)
            .WithMessage("Yeni şifre mevcut şifre ile aynı olamaz.");
    }
}
