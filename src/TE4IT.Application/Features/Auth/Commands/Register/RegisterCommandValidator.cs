using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
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
            .MinimumLength(8)
            .WithMessage("Şifre en az 8 karakter olmalıdır.")
            .MaximumLength(100)
            .WithMessage("Şifre en fazla 100 karakter olabilir.")
            .Matches(@"[A-Z]")
            .WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches(@"[a-z]")
            .WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches(@"[0-9]")
            .WithMessage("Şifre en az bir rakam içermelidir.");
    }
}

