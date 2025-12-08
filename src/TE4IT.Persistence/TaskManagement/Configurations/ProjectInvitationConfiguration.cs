using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Enums;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Persistence.TaskManagement.Configurations;

public sealed class ProjectInvitationConfiguration : IEntityTypeConfiguration<ProjectInvitation>
{
    public void Configure(EntityTypeBuilder<ProjectInvitation> b)
    {
        b.ToTable("ProjectInvitations");

        b.HasKey(pi => pi.Id);

        b.Property(pi => pi.ProjectId)
            .IsRequired();

        b.Property(pi => pi.Email)
            .HasMaxLength(256)
            .IsRequired();

        b.Property(pi => pi.Role)
            .HasConversion<int>()
            .IsRequired();

        b.Property(pi => pi.Token)
            .HasMaxLength(512)
            .IsRequired();

        b.Property(pi => pi.TokenHash)
            .HasMaxLength(256)
            .IsRequired();

        b.Property(pi => pi.InvitedByUserId)
            .HasConversion(
                v => v.Value,
                v => (UserId)v)
            .IsRequired();

        b.Property(pi => pi.ExpiresAt)
            .IsRequired();

        b.Property(pi => pi.AcceptedAt)
            .IsRequired(false);

        b.Property(pi => pi.AcceptedByUserId)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v.HasValue ? (UserId)v.Value : null)
            .IsRequired(false);

        b.Property(pi => pi.CancelledAt)
            .IsRequired(false);

        b.Property(pi => pi.CreatedDate)
            .IsRequired();

        b.Property(pi => pi.UpdatedDate)
            .IsRequired(false);

        b.Property(pi => pi.DeletedDate)
            .IsRequired(false);

        // Indexes
        b.HasIndex(pi => pi.ProjectId)
            .HasDatabaseName("IX_ProjectInvitations_ProjectId");

        b.HasIndex(pi => pi.Email)
            .HasDatabaseName("IX_ProjectInvitations_Email");

        b.HasIndex(pi => pi.TokenHash)
            .IsUnique()
            .HasDatabaseName("IX_ProjectInvitations_TokenHash");

        b.HasIndex(pi => new { pi.ProjectId, pi.Email })
            .HasDatabaseName("IX_ProjectInvitations_ProjectId_Email");

        // Foreign Keys
        b.HasOne<Project>()
            .WithMany()
            .HasForeignKey(pi => pi.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ProjectInvitations_ProjectId_Projects");

        // Note: Foreign keys to AspNetUsers cannot be configured here
        // because AppUser is in the Identity framework context
        // They will be added manually in the migration
    }
}

