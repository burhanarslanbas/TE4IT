using FluentValidation;

namespace TE4IT.Application.Features.UseCases.Commands.ChangeUseCaseStatus;

public sealed class ChangeUseCaseStatusCommandValidator : AbstractValidator<ChangeUseCaseStatusCommand>
{
    public ChangeUseCaseStatusCommandValidator()
    {
        RuleFor(x => x.UseCaseId).NotEmpty();
    }
}

