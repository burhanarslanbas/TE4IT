using FluentValidation;

namespace TE4IT.Application.Features.Tasks.Commands.UnassignTask;

public sealed class UnassignTaskCommandValidator : AbstractValidator<UnassignTaskCommand>
{
    public UnassignTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

