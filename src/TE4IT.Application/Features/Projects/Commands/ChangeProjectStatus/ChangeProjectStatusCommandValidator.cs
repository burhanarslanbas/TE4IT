using FluentValidation;

namespace TE4IT.Application.Features.Projects.Commands.ChangeProjectStatus;

public sealed class ChangeProjectStatusCommandValidator : AbstractValidator<ChangeProjectStatusCommand>
{
    public ChangeProjectStatusCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}


