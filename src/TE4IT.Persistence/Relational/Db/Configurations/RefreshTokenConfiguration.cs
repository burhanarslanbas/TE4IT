using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Persistence.Relational.Identity;

namespace TE4IT.Persistence.Relational.Db.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Token).IsRequired().HasMaxLength(512);
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.ExpiresAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.RevokedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreatedByIp).HasMaxLength(64);
        builder.Property(x => x.RevokedByIp).HasMaxLength(64);
        builder.Property(x => x.ReplacedByToken).HasMaxLength(512);
        builder.Property(x => x.TokenHash).HasMaxLength(128);
        builder.Property(x => x.RevokeReason).HasMaxLength(128);
        builder.HasIndex(x => x.Token).IsUnique();
        builder.HasIndex(x => x.TokenHash);
        builder.HasIndex(x => new { x.UserId, x.RevokedAt });
    }
}