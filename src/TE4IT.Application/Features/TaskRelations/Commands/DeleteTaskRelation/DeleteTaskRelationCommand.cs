using MediatR;

namespace TE4IT.Application.Features.TaskRelations.Commands.DeleteTaskRelation;

public sealed record DeleteTaskRelationCommand(Guid TaskId, Guid RelationId) : IRequest<bool>;

