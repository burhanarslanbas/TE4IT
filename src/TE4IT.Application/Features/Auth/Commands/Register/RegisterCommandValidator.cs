using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Kullanıcı adı zorunludur.")
            .MaximumLength(100)
            .WithMessage("Kullanıcı adı en fazla 100 karakter olabilir.")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-posta adresi zorunludur.")
            .EmailAddress()
            .WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(100)
            .WithMessage("E-posta adresi en fazla 100 karakter olabilir.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Şifre zorunludur.")
            .MinimumLength(6)
            .WithMessage("Şifre en az 6 karakter olmalıdır.")
            .MaximumLength(100)
            .WithMessage("Şifre en fazla 100 karakter olabilir.");
    }
}

