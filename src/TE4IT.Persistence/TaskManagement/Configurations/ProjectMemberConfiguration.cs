using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Enums;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Identity;

namespace TE4IT.Persistence.TaskManagement.Configurations;

public sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> b)
    {
        b.HasKey(x => x.Id);

        b.Property(pm => pm.ProjectId).IsRequired();
        b.Property(pm => pm.UserId)
            .HasConversion(v => v.Value, v => (UserId)v)
            .IsRequired();
        b.Property(pm => pm.Role)
            .HasConversion<int>()
            .IsRequired();
        b.Property(pm => pm.JoinedDate).IsRequired();

        // Composite unique index: Bir kullanıcı bir projede sadece bir kez bulunabilir
        b.HasIndex(x => new { x.ProjectId, x.UserId }).IsUnique();

        // FK: ProjectMember.ProjectId -> Projects.Id (ON DELETE CASCADE)
        // FK: ProjectMember.UserId -> AspNetUsers.Id (ON DELETE RESTRICT)
        // Not: ValueObject conversion ile FK tanımı EF Core'da sorun çıkarıyor
        // Migration'da manuel olarak SQL ile ekleniyor
    }
}

