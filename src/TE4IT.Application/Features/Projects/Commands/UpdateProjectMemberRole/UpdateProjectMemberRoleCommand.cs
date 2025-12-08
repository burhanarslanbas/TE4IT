using MediatR;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Projects.Commands.UpdateProjectMemberRole;

public sealed record UpdateProjectMemberRoleCommand(
    Guid ProjectId,
    Guid UserId,
    ProjectRole NewRole) : IRequest<bool>;

