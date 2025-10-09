using FluentValidation;

namespace TE4IT.Application.Features.Auth.Queries.Roles.GetRoleById;

public sealed class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Rol ID'si zorunludur.");
    }
}

