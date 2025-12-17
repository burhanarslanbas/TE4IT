using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Enums.Education;

namespace TE4IT.Domain.Entities.Education;

/// <summary>
/// Bir roadmap adımındaki tekil içerik. Zorunluluk ve sıralama kuralları içerik seviyesinde tutulur.
/// </summary>
public sealed class CourseContent : BaseEntity
{
    private CourseContent()
    {
    }

    public CourseContent(
        ContentType type,
        string title,
        int order,
        bool isRequired,
        string? content = null,
        string? linkUrl = null,
        string? embedUrl = null,
        string? platform = null,
        string? description = null)
    {
        Type = type;
        Title = title;
        Order = order;
        IsRequired = isRequired;
        Content = content;
        LinkUrl = linkUrl;
        EmbedUrl = embedUrl;
        Platform = platform;
        Description = description;
    }

    public ContentType Type { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Content { get; private set; }
    public string? LinkUrl { get; private set; }
    public string? EmbedUrl { get; private set; }
    public string? Platform { get; private set; }
    public int Order { get; private set; }
    public bool IsRequired { get; private set; }
}