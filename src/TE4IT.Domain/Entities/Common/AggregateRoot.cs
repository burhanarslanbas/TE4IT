using TE4IT.Domain.Events;

namespace TE4IT.Domain.Entities.Common;

/// <summary>
/// Aggregate Root base sınıfı - domain event'leri yönetir
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Domain event'leri
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Domain event ekler
    /// </summary>
    /// <param name="domainEvent">Eklenecek domain event</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Domain event'i kaldırır
    /// </summary>
    /// <param name="domainEvent">Kaldırılacak domain event</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Tüm domain event'leri temizler
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
