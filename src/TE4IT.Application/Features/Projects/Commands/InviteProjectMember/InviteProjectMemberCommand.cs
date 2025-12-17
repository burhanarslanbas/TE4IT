using MediatR;
using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Projects.Commands.InviteProjectMember;

public sealed record InviteProjectMemberCommand(
    Guid ProjectId,
    string Email,
    ProjectRole Role) : IRequest<InviteProjectMemberCommandResponse>;

