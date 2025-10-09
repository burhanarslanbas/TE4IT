using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Roles.UpdateRole;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Rol ID'si zorunludur.");

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Rol adı zorunludur.")
            .MinimumLength(2)
            .WithMessage("Rol adı en az 2 karakter olmalıdır.")
            .MaximumLength(50)
            .WithMessage("Rol adı en fazla 50 karakter olabilir.")
            .Matches(@"^[a-zA-Z]+$")
            .WithMessage("Rol adı sadece harf içerebilir (boşluk veya özel karakter kullanılamaz).");
    }
}

