using TE4IT.Domain.Enums.Education;

namespace TE4IT.Application.Features.Education.Roadmaps.Responses;

/// <summary>
/// Content response DTO
/// </summary>
public sealed class ContentResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public ContentType Type { get; init; }
    public string? Content { get; init; }
    public string? LinkUrl { get; init; }
    public string? EmbedUrl { get; init; }
    public string? Platform { get; init; }
}

