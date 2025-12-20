using FluentValidation;
using TE4IT.Domain.Constants;

namespace TE4IT.Application.Features.Tasks.Commands.CompleteTask;

public sealed class CompleteTaskCommandValidator : AbstractValidator<CompleteTaskCommand>
{
    public CompleteTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();

        RuleFor(x => x.CompletionNote)
            .MaximumLength(DomainConstants.MaxTaskCompletionNoteLength)
            .When(x => x.CompletionNote != null)
            .WithMessage($"Tamamlanma notu en fazla {DomainConstants.MaxTaskCompletionNoteLength} karakter olabilir.");
    }
}

