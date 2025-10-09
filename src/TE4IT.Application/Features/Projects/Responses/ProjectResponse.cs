namespace TE4IT.Application.Features.Projects.Responses;

public sealed class ProjectResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
}
