using FluentValidation;

namespace TE4IT.Application.Features.Tasks.Commands.StartTask;

public sealed class StartTaskCommandValidator : AbstractValidator<StartTaskCommand>
{
    public StartTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

