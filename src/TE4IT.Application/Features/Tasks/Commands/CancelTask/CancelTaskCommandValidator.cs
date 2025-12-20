using FluentValidation;

namespace TE4IT.Application.Features.Tasks.Commands.CancelTask;

public sealed class CancelTaskCommandValidator : AbstractValidator<CancelTaskCommand>
{
    public CancelTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

