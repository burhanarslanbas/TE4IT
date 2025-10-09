using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-posta adresi zorunludur.")
            .EmailAddress()
            .WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Şifre zorunludur.")
            .MinimumLength(6)
            .WithMessage("Şifre en az 6 karakter olmalıdır.");
    }
}

