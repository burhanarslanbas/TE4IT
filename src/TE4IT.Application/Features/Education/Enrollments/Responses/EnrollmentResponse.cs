namespace TE4IT.Application.Features.Education.Enrollments.Responses;

/// <summary>
/// Enrollment response DTO
/// </summary>
public sealed class EnrollmentResponse
{
    public Guid Id { get; init; }
    public DateTime EnrolledAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public bool IsActive { get; init; }
}

