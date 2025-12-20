using FluentValidation;

namespace TE4IT.Application.Features.Tasks.Commands.AssignAndStartTask;

public sealed class AssignAndStartTaskCommandValidator : AbstractValidator<AssignAndStartTaskCommand>
{
    public AssignAndStartTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.AssigneeId).NotEmpty();
    }
}

