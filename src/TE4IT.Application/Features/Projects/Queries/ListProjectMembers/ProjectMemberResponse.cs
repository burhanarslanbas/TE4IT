using TE4IT.Domain.Enums;

namespace TE4IT.Application.Features.Projects.Responses;

public sealed class ProjectMemberResponse
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public ProjectRole Role { get; init; }
    public DateTime JoinedDate { get; init; }
}

