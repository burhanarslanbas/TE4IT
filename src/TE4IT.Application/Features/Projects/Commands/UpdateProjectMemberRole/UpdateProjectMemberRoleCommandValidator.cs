using FluentValidation;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Services;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProjectMemberRole;

public sealed class UpdateProjectMemberRoleCommandValidator : AbstractValidator<UpdateProjectMemberRoleCommand>
{
    public UpdateProjectMemberRoleCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("A valid role must be selected.")
            .Must(role => role == ProjectRole.Member || role == ProjectRole.Viewer)
            .WithMessage("Only Member or Viewer role can be assigned. Owner role cannot be changed.");
    }
}

