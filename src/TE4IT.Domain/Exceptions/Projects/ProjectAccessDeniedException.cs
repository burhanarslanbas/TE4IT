using TE4IT.Domain.Exceptions.Base;

namespace TE4IT.Domain.Exceptions.Projects;

/// <summary>
/// Proje erişim yetkisi yok için exception
/// </summary>
public class ProjectAccessDeniedException : DomainException
{
    public Guid ProjectId { get; }
    public Guid UserId { get; }

    public ProjectAccessDeniedException(Guid projectId, Guid userId)
        : base($"User {userId} cannot access project {projectId}")
    {
        ProjectId = projectId;
        UserId = userId;
    }

    public ProjectAccessDeniedException(Guid projectId, Guid userId, string message)
        : base(message)
    {
        ProjectId = projectId;
        UserId = userId;
    }

    public ProjectAccessDeniedException(string message) : base(message)
    {
    }

    public ProjectAccessDeniedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
