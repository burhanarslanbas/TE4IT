namespace TE4IT.Domain.Events;

/// <summary>
/// Domain event marker interface - domain'deki önemli olayları temsil eder
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Olayın oluştuğu tarih
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Olayın benzersiz kimliği
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Olayın türü
    /// </summary>
    string EventType { get; }
}
