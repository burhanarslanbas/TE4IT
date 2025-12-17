using TE4IT.Domain.Entities.Common;
using TE4IT.Domain.Enums;
using TE4IT.Domain.Events;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Domain.Entities;

/// <summary>
/// Proje daveti entity'si - email tabanlı davet sistemi
/// </summary>
public class ProjectInvitation : AggregateRoot
{
    public Guid ProjectId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public ProjectRole Role { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public UserId InvitedByUserId { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public UserId? AcceptedByUserId { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private ProjectInvitation() { }

    /// <summary>
    /// Proje daveti oluşturur
    /// </summary>
    public static ProjectInvitation Create(
        Guid projectId,
        string email,
        ProjectRole role,
        UserId invitedByUserId,
        string token,
        string tokenHash,
        int expirationDays = 7)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException("Invalid email format.", nameof(email));

        if (invitedByUserId == null)
            throw new ArgumentNullException(nameof(invitedByUserId));

        if (role != ProjectRole.Member && role != ProjectRole.Viewer)
            throw new ArgumentException("Role must be Member or Viewer.", nameof(role));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty.", nameof(token));

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash cannot be empty.", nameof(tokenHash));

        if (expirationDays <= 0)
            throw new ArgumentException("Expiration days must be greater than zero.", nameof(expirationDays));

        var invitation = new ProjectInvitation
        {
            ProjectId = projectId,
            Email = email.ToLowerInvariant().Trim(),
            Role = role,
            Token = token,
            TokenHash = tokenHash,
            InvitedByUserId = invitedByUserId,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
            AcceptedAt = null,
            AcceptedByUserId = null,
            CancelledAt = null
        };

        invitation.AddDomainEvent(new ProjectInvitationSentEvent(
            invitation.Id,
            projectId,
            email,
            (int)role,
            invitedByUserId.Value,
            invitation.ExpiresAt));

        return invitation;
    }

    /// <summary>
    /// Daveti kabul eder
    /// </summary>
    public void Accept(UserId acceptedByUserId)
    {
        if (acceptedByUserId == null)
            throw new ArgumentNullException(nameof(acceptedByUserId));

        if (IsExpired)
            throw new InvalidOperationException("Cannot accept expired invitation.");

        if (CancelledAt != null)
            throw new InvalidOperationException("Cannot accept cancelled invitation.");

        if (AcceptedAt != null)
            throw new InvalidOperationException("Invitation already accepted.");

        AcceptedAt = DateTime.UtcNow;
        AcceptedByUserId = acceptedByUserId;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new ProjectInvitationAcceptedEvent(
            Id,
            ProjectId,
            Email,
            acceptedByUserId.Value,
            AcceptedAt.Value));
    }

    /// <summary>
    /// Daveti iptal eder
    /// </summary>
    public void Cancel()
    {
        if (AcceptedAt != null)
            throw new InvalidOperationException("Cannot cancel accepted invitation.");

        if (CancelledAt != null)
            throw new InvalidOperationException("Invitation already cancelled.");

        CancelledAt = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new ProjectInvitationCancelledEvent(
            Id,
            ProjectId,
            Email,
            CancelledAt.Value));
    }

    /// <summary>
    /// Davetin süresi dolmuş mu?
    /// </summary>
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

    /// <summary>
    /// Davet durumu
    /// </summary>
    public string Status
    {
        get
        {
            if (AcceptedAt != null)
                return "Accepted";

            if (CancelledAt != null)
                return "Cancelled";

            if (IsExpired)
                return "Expired";

            return "Pending";
        }
    }

    /// <summary>
    /// Davet beklemede mi?
    /// </summary>
    public bool IsPending => AcceptedAt == null && CancelledAt == null && !IsExpired;
}

