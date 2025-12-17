using FluentValidation;

namespace TE4IT.Application.Features.TaskRelations.Commands.DeleteTaskRelation;

public sealed class DeleteTaskRelationCommandValidator : AbstractValidator<DeleteTaskRelationCommand>
{
    public DeleteTaskRelationCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.RelationId).NotEmpty();
    }
}

