using FluentValidation;

namespace TE4IT.Application.Features.Tasks.Queries.ListTasks;

public sealed class ListTasksQueryValidator : AbstractValidator<ListTasksQuery>
{
    public ListTasksQueryValidator()
    {
        RuleFor(x => x.UseCaseId).NotEmpty();
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

