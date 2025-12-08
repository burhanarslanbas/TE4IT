namespace TE4IT.Application.Features.UseCases.Responses;

public sealed class UseCaseResponse
{
    public Guid Id { get; init; }
    public Guid ModuleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImportantNotes { get; init; }
    public bool IsActive { get; init; }
    public DateTime StartedDate { get; init; }
}

