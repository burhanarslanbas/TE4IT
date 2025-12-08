using MediatR;

namespace TE4IT.Application.Features.Projects.Commands.RemoveProjectMember;

public sealed record RemoveProjectMemberCommand(
    Guid ProjectId,
    Guid UserId) : IRequest<bool>;

