using MediatR;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Projects.Commands.AddProjectMember;

public sealed record AddProjectMemberCommand(
    Guid ProjectId,
    Guid UserId,
    ProjectRole Role) : IRequest<AddProjectMemberCommandResponse>;

