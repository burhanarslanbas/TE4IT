using FluentValidation;
using TE4IT.Domain.Constants;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Proje başlığı zorunludur.")
            .MinimumLength(DomainConstants.MinProjectTitleLength)
            .WithMessage($"Proje başlığı en az {DomainConstants.MinProjectTitleLength} karakter olmalıdır.")
            .MaximumLength(DomainConstants.MaxProjectTitleLength)
            .WithMessage($"Proje başlığı en fazla {DomainConstants.MaxProjectTitleLength} karakter olabilir.");
        
        RuleFor(x => x.Description)
            .MaximumLength(DomainConstants.MaxProjectDescriptionLength)
            .When(x => x.Description != null)
            .WithMessage($"Proje açıklaması en fazla {DomainConstants.MaxProjectDescriptionLength} karakter olabilir.");
    }
}


