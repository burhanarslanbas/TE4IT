using FluentValidation;

namespace TE4IT.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryValidator : AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
    }
}

