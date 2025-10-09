using MediatR;
using TE4IT.Application.Features.Projects.Responses;

namespace TE4IT.Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid ProjectId) : IRequest<ProjectResponse>;