using FluentValidation;
using TE4IT.Domain.Constants;

namespace TE4IT.Application.Features.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Görev başlığı zorunludur.")
            .MinimumLength(DomainConstants.MinTaskTitleLength)
            .WithMessage($"Görev başlığı en az {DomainConstants.MinTaskTitleLength} karakter olmalıdır.")
            .MaximumLength(DomainConstants.MaxTaskTitleLength)
            .WithMessage($"Görev başlığı en fazla {DomainConstants.MaxTaskTitleLength} karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.MaxTaskDescriptionLength)
            .When(x => x.Description != null)
            .WithMessage($"Görev açıklaması en fazla {DomainConstants.MaxTaskDescriptionLength} karakter olabilir.");

        RuleFor(x => x.ImportantNotes)
            .MaximumLength(DomainConstants.MaxTaskImportantNotesLength)
            .When(x => x.ImportantNotes != null)
            .WithMessage($"Önemli notlar en fazla {DomainConstants.MaxTaskImportantNotesLength} karakter olabilir.");
    }
}

