using FluentValidation;

namespace TE4IT.Application.Features.Projects.Commands.CancelProjectInvitation;

public sealed class CancelProjectInvitationCommandValidator : AbstractValidator<CancelProjectInvitationCommand>
{
    public CancelProjectInvitationCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Proje ID'si zorunludur.");

        RuleFor(x => x.InvitationId)
            .NotEmpty()
            .WithMessage("Davet ID'si zorunludur.");
    }
}

