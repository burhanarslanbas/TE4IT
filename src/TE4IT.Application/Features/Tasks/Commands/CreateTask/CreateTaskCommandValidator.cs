using FluentValidation;
using TE4IT.Domain.Constants;

namespace TE4IT.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.UseCaseId)
            .NotEmpty()
            .WithMessage("Use Case ID'si zorunludur.");

        RuleFor(x => x.CreatorId)
            .NotEmpty()
            .WithMessage("Oluşturan kişi ID'si zorunludur.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Görev başlığı zorunludur.")
            .MinimumLength(DomainConstants.MinTaskTitleLength)
            .WithMessage($"Görev başlığı en az {DomainConstants.MinTaskTitleLength} karakter olmalıdır.")
            .MaximumLength(DomainConstants.MaxTaskTitleLength)
            .WithMessage($"Görev başlığı en fazla {DomainConstants.MaxTaskTitleLength} karakter olabilir.");

        RuleFor(x => x.TaskType)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Görev tipi geçerli olmalıdır.")
            .LessThanOrEqualTo(3)
            .WithMessage("Görev tipi geçerli olmalıdır.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.MaxTaskDescriptionLength)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage($"Görev açıklaması en fazla {DomainConstants.MaxTaskDescriptionLength} karakter olabilir.");

        RuleFor(x => x.ImportantNotes)
            .MaximumLength(DomainConstants.MaxTaskImportantNotesLength)
            .When(x => !string.IsNullOrEmpty(x.ImportantNotes))
            .WithMessage($"Önemli notlar en fazla {DomainConstants.MaxTaskImportantNotesLength} karakter olabilir.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Teslim tarihi gelecekte bir tarih olmalıdır.");
    }
}

