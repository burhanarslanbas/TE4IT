using MediatR;
using TE4IT.Application.Features.Tasks.Responses;

namespace TE4IT.Application.Features.TaskRelations.Queries.GetTaskRelations;

public sealed record GetTaskRelationsQuery(Guid TaskId) : IRequest<List<TaskRelationResponse>>;

