using FluentValidation;

namespace TE4IT.Application.Features.UseCases.Commands.DeleteUseCase;

public sealed class DeleteUseCaseCommandValidator : AbstractValidator<DeleteUseCaseCommand>
{
    public DeleteUseCaseCommandValidator()
    {
        RuleFor(x => x.UseCaseId).NotEmpty();
    }
}

