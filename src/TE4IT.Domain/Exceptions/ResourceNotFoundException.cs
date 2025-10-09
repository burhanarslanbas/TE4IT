namespace TE4IT.Domain.Exceptions;

/// <summary>
/// Kaynak bulunamadığında fırlatılan exception
/// </summary>
public class ResourceNotFoundException : DomainException
{
    public string ResourceType { get; init; } = string.Empty;
    public Guid ResourceId { get; init; }

    public ResourceNotFoundException(string resourceType, Guid resourceId)
        : base($"{resourceType} with ID {resourceId} not found")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public ResourceNotFoundException(string message) : base(message)
    {
        ResourceType = string.Empty;
        ResourceId = Guid.Empty;
    }

    public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
