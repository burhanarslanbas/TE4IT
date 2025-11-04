using FluentValidation;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(4000);
    }
}


