using FluentValidation;

namespace TE4IT.Application.Features.Invitations.Commands.CancelInvitation;

public sealed class CancelInvitationCommandValidator : AbstractValidator<CancelInvitationCommand>
{
    public CancelInvitationCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Invitation ID is required.");
    }
}
