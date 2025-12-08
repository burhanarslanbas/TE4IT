using FluentValidation;
using TE4IT.Domain.Constants;

namespace TE4IT.Application.Features.Modules.Commands.CreateModule;

/// <summary>
/// CreateModuleCommand için validation kuralları
/// </summary>
public sealed class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Proje ID'si zorunludur.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Modül başlığı zorunludur.")
            .MinimumLength(DomainConstants.MinModuleTitleLength)
            .WithMessage($"Modül başlığı en az {DomainConstants.MinModuleTitleLength} karakter olmalıdır.")
            .MaximumLength(DomainConstants.MaxModuleTitleLength)
            .WithMessage($"Modül başlığı en fazla {DomainConstants.MaxModuleTitleLength} karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.MaxModuleDescriptionLength)
            .When(x => x.Description != null)
            .WithMessage($"Modül açıklaması en fazla {DomainConstants.MaxModuleDescriptionLength} karakter olabilir.");
    }
}

