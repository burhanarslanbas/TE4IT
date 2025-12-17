namespace TE4IT.Application.Features.Education.Roadmaps.Responses;

/// <summary>
/// Step response DTO
/// </summary>
public sealed class StepResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public int Order { get; init; }
    public IReadOnlyList<ContentResponse> Contents { get; init; } = Array.Empty<ContentResponse>();
}

