using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Domain.Entities;

public class UseCase : AggregateRoot
{
    public Guid ModuleId { get; private set; } // Modül ID'si
    public UserId CreatorId { get; private set; } = null!; // Oluşturan Kişi ID'si
    public string Title { get; private set; } = default!; // Kullanım Senaryosu Başlığı
    public string? Description { get; private set; } // Kullanım Senaryosu Açıklaması
    public string? ImportantNotes { get; private set; } // Önemli Notlar
    // public List<string>? Tags { get; set; } // Etiket Listesi
    public DateTime StartedDate { get; private set; } = default; // Başlangıç Tarihi
    public bool IsActive { get; private set; } = false; // Aktiflik durumu

    private UseCase() { }

    public static UseCase Create(Guid moduleId, UserId creatorId, string title, string? description = null, string? importantNotes = null)
    {
        if (moduleId == Guid.Empty) throw new ArgumentException("ModuleId boş olamaz.", nameof(moduleId));
        if ((Guid)creatorId == Guid.Empty) throw new ArgumentException("CreatorId boş olamaz.", nameof(creatorId));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title zorunludur.", nameof(title));

        return new UseCase
        {
            ModuleId = moduleId,
            CreatorId = creatorId,
            Title = title,
            Description = description,
            ImportantNotes = importantNotes,
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

    public void UpdateImportantNotes(string? importantNotes)
    {
        ImportantNotes = importantNotes;
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
