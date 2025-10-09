namespace TE4IT.Application.Features.Projects.Responses;

public sealed class ProjectListItemResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
}
