using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Identity;

namespace TE4IT.Persistence.TaskManagement.Configurations;

public sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> b)
    {
        b.Property(m => m.Title).IsRequired().HasMaxLength(100);
        b.Property(m => m.Description).HasMaxLength(1000);
        b.Property(m => m.CreatorId).HasConversion(v => v.Value, v => (UserId)v);
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.ProjectId);

        b.HasOne<Project>()
            .WithMany()
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // FK: Module.CreatorId -> AspNetUsers.Id
        // Not: ValueObject conversion ile FK tanımı EF Core'da sorun çıkarıyor
        // Migration'da manuel olarak SQL ile ekleniyor
    }
}


