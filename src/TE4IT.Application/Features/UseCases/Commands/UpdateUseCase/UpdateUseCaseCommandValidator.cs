using FluentValidation;
using TE4IT.Domain.Constants;

namespace TE4IT.Application.Features.UseCases.Commands.UpdateUseCase;

public sealed class UpdateUseCaseCommandValidator : AbstractValidator<UpdateUseCaseCommand>
{
    public UpdateUseCaseCommandValidator()
    {
        RuleFor(x => x.UseCaseId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Kullanım senaryosu başlığı zorunludur.")
            .MinimumLength(DomainConstants.MinUseCaseTitleLength)
            .WithMessage($"Kullanım senaryosu başlığı en az {DomainConstants.MinUseCaseTitleLength} karakter olmalıdır.")
            .MaximumLength(DomainConstants.MaxUseCaseTitleLength)
            .WithMessage($"Kullanım senaryosu başlığı en fazla {DomainConstants.MaxUseCaseTitleLength} karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.MaxUseCaseDescriptionLength)
            .When(x => x.Description != null)
            .WithMessage($"Kullanım senaryosu açıklaması en fazla {DomainConstants.MaxUseCaseDescriptionLength} karakter olabilir.");

        RuleFor(x => x.ImportantNotes)
            .MaximumLength(DomainConstants.MaxUseCaseImportantNotesLength)
            .When(x => x.ImportantNotes != null)
            .WithMessage($"Önemli notlar en fazla {DomainConstants.MaxUseCaseImportantNotesLength} karakter olabilir.");
    }
}

