using TE4IT.Domain.Constants;
using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Events;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Domain.Entities;

public class Project : AggregateRoot
{
    public UserId CreatorId { get; private set; } = null!; // Oluşturan Kişi ID'si
    public string Title { get; private set; } = string.Empty; // Proje Başlığı
    public string? Description { get; private set; } // Proje Açıklaması
    public DateTime StartedDate { get; private set; } = DateTime.MinValue; // Başlangıç Tarihi
    public bool IsActive { get; private set; } = false; // Aktiflik durumu

    /// <summary>
    /// Proje oluşturur
    /// </summary>
    private Project() { }

    public static Project Create(UserId creatorId, string title, string? description = null)
    {
        ValidateTitle(title);
        ValidateDescription(description);

        var project = new Project
        {
            CreatorId = creatorId,
            Title = title,
            Description = description,
            StartedDate = DateTime.UtcNow,
            IsActive = true
        };

        project.AddDomainEvent(new ProjectCreatedEvent(project.Id, creatorId.Value, title, description));
        return project;
    }

    /// <summary>
    /// Proje başlığını günceller
    /// </summary>
    public void UpdateTitle(string title)
    {
        ValidateTitle(title);
        Title = title;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Proje açıklamasını günceller
    /// </summary>
    public void UpdateDescription(string? description)
    {
        ValidateDescription(description);
        Description = description;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Proje durumunu değiştirir
    /// </summary>
    public void ChangeStatus(bool isActive)
    {
        if (IsActive == isActive)
            return;

        var previousStatus = IsActive;
        IsActive = isActive;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new ProjectStatusChangedEvent(Id, CreatorId.Value, Title, previousStatus, isActive));
    }

    /// <summary>
    /// Projeyi arşivler
    /// </summary>
    public void Archive()
    {
        ChangeStatus(false);
    }

    /// <summary>
    /// Projeyi aktifleştirir
    /// </summary>
    public void Activate()
    {
        ChangeStatus(true);
    }

    // Project aggregate, Module koleksiyonunu yönetmez; ilişki ID ile yürütülür.

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(DomainConstants.RequiredFieldMessage, nameof(title));

        if (title.Length < DomainConstants.MinProjectTitleLength)
            throw new ArgumentException($"Proje başlığı en az {DomainConstants.MinProjectTitleLength} karakter olmalıdır.", nameof(title));

        if (title.Length > DomainConstants.MaxProjectTitleLength)
            throw new ArgumentException($"Proje başlığı en fazla {DomainConstants.MaxProjectTitleLength} karakter olabilir.", nameof(title));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length > DomainConstants.MaxProjectDescriptionLength)
            throw new ArgumentException($"Proje açıklaması en fazla {DomainConstants.MaxProjectDescriptionLength} karakter olabilir.", nameof(description));
    }
}
