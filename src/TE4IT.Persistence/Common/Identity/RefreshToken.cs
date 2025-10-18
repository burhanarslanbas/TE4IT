using System.ComponentModel.DataAnnotations;

namespace TE4IT.Persistence.Common.Identity;

public sealed class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(512)]
    public string Token { get; set; } = string.Empty;

    [MaxLength(128)]
    public string? TokenHash { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(64)]
    public string? CreatedByIp { get; set; }

    public DateTime? RevokedAt { get; set; }

    [MaxLength(64)]
    public string? RevokedByIp { get; set; }

    [MaxLength(512)]
    public string? ReplacedByToken { get; set; }

    [MaxLength(128)]
    public string? RevokeReason { get; set; }
}


