using FluentValidation;

namespace TE4IT.Application.Features.UseCases.Queries.ListUseCases;

public sealed class ListUseCasesQueryValidator : AbstractValidator<ListUseCasesQuery>
{
    public ListUseCasesQueryValidator()
    {
        RuleFor(x => x.ModuleId).NotEmpty();
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

