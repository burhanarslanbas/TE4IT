using FluentValidation;

namespace TE4IT.Application.Features.Auth.Queries.Users.GetUserRoles;

public sealed class GetUserRolesQueryValidator : AbstractValidator<GetUserRolesQuery>
{
    public GetUserRolesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Kullanıcı ID'si zorunludur.");
    }
}

