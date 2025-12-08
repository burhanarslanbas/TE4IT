using MediatR;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.TaskRelations.Commands.CreateTaskRelation;

public sealed record CreateTaskRelationCommand(
    Guid SourceTaskId,
    Guid TargetTaskId,
    TaskRelationType RelationType) : IRequest<bool>;

