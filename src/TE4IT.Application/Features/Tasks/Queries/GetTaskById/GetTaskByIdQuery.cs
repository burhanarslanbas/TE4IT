using MediatR;
using TE4IT.Application.Features.Tasks.Responses;

namespace TE4IT.Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskResponse>;

