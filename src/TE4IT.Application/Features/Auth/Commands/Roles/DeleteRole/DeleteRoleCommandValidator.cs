using FluentValidation;

namespace TE4IT.Application.Features.Auth.Commands.Roles.DeleteRole;

public sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Rol ID'si zorunludur.");
    }
}

