namespace TE4IT.Domain.Exceptions;

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

    public ProjectAccessDeniedException(string message) : base(message)
    {
    }

    public ProjectAccessDeniedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
