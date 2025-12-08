using FluentValidation;

namespace TE4IT.Application.Features.UseCases.Queries.GetUseCaseById;

public sealed class GetUseCaseByIdQueryValidator : AbstractValidator<GetUseCaseByIdQuery>
{
    public GetUseCaseByIdQueryValidator()
    {
        RuleFor(x => x.UseCaseId).NotEmpty();
    }
}

