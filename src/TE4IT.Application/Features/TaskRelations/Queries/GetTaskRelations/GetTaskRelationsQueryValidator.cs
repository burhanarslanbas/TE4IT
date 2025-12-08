using FluentValidation;

namespace TE4IT.Application.Features.TaskRelations.Queries.GetTaskRelations;

public sealed class GetTaskRelationsQueryValidator : AbstractValidator<GetTaskRelationsQuery>
{
    public GetTaskRelationsQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

