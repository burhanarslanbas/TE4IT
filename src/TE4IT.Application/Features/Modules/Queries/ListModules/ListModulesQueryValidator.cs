using FluentValidation;

namespace TE4IT.Application.Features.Modules.Queries.ListModules;

public sealed class ListModulesQueryValidator : AbstractValidator<ListModulesQuery>
{
    public ListModulesQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

