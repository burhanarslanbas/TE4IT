using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Domain.Entities;

public class Module : AggregateRoot
{
    public Guid ProjectId { get; private set; } // Proje ID'si
    public UserId CreatorId { get; private set; } = null!; // Oluşturan Kişi ID'si
    public string Title { get; private set; } = default!; // Modül Başlığı
    public string? Description { get; private set; } // Modül Açıklaması
    public DateTime StartedDate { get; private set; } = default; // Başlangıç Tarihi
    public bool IsActive { get; private set; } = false; // Aktiflik durumu

    private Module() { }

    public static Module Create(Guid projectId, UserId creatorId, string title, string? description = null)
    {
        if (projectId == Guid.Empty) throw new ArgumentException("ProjectId boş olamaz.", nameof(projectId));
        if ((Guid)creatorId == Guid.Empty) throw new ArgumentException("CreatorId boş olamaz.", nameof(creatorId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title zorunludur.", nameof(title));

        return new Module
        {
            ProjectId = projectId,
            CreatorId = creatorId,
            Title = title,
            Description = description,
            StartedDate = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title zorunludur.", nameof(title));
        Title = title;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            UpdatedDate = DateTime.UtcNow;
        }
    }

    public void Archive()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}