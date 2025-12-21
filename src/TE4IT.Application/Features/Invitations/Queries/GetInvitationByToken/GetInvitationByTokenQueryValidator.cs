using FluentValidation;

namespace TE4IT.Application.Features.Invitations.Queries.GetInvitationByToken;

public sealed class GetInvitationByTokenQueryValidator : AbstractValidator<GetInvitationByTokenQuery>
{
    public GetInvitationByTokenQueryValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required.");
    }
}
