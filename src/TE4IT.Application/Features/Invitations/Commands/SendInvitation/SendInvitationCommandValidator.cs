using FluentValidation;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Invitations.Commands.SendInvitation;

public sealed class SendInvitationCommandValidator : AbstractValidator<SendInvitationCommand>
{
    public SendInvitationCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email address is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.")
            .MaximumLength(256)
            .WithMessage("Email address cannot exceed 256 characters.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("A valid role must be selected.")
            .Must(role => role == ProjectRole.Member || role == ProjectRole.Viewer)
            .WithMessage("Only Member or Viewer role can be assigned. Owner role is only assigned when the project is created.");
    }
}
