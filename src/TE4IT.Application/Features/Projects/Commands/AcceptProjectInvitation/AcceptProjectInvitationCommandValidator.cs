using FluentValidation;

namespace TE4IT.Application.Features.Projects.Commands.AcceptProjectInvitation;

public sealed class AcceptProjectInvitationCommandValidator : AbstractValidator<AcceptProjectInvitationCommand>
{
    public AcceptProjectInvitationCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token zorunludur.");
    }
}

