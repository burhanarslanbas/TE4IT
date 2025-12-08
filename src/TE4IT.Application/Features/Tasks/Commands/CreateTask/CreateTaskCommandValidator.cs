using FluentValidation;
using TE4IT.Domain.Constants;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.UseCaseId)
            .NotEmpty()
            .WithMessage("Kullanım senaryosu ID'si zorunludur.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Görev başlığı zorunludur.")
            .MinimumLength(DomainConstants.MinTaskTitleLength)
            .WithMessage($"Görev başlığı en az {DomainConstants.MinTaskTitleLength} karakter olmalıdır.")
            .MaximumLength(DomainConstants.MaxTaskTitleLength)
            .WithMessage($"Görev başlığı en fazla {DomainConstants.MaxTaskTitleLength} karakter olabilir.");

        RuleFor(x => x.TaskType)
            .IsInEnum()
            .WithMessage("Geçerli bir görev tipi seçilmelidir.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.MaxTaskDescriptionLength)
            .When(x => x.Description != null)
            .WithMessage($"Görev açıklaması en fazla {DomainConstants.MaxTaskDescriptionLength} karakter olabilir.");

        RuleFor(x => x.ImportantNotes)
            .MaximumLength(DomainConstants.MaxTaskImportantNotesLength)
            .When(x => x.ImportantNotes != null)
            .WithMessage($"Önemli notlar en fazla {DomainConstants.MaxTaskImportantNotesLength} karakter olabilir.");

        RuleFor(x => x.AssigneeId)
            .NotEmpty()
            .When(x => x.AssigneeId.HasValue)
            .WithMessage("Atanan kişi ID'si geçerli bir GUID olmalıdır.");
    }
}

