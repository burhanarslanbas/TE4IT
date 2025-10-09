using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Users.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Kullanıcı ID'si zorunludur.");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Rol adı zorunludur.")
            .MinimumLength(2)
            .WithMessage("Rol adı en az 2 karakter olmalıdır.")
            .MaximumLength(50)
            .WithMessage("Rol adı en fazla 50 karakter olabilir.");
    }
}

