using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Identity;

namespace TE4IT.Persistence.TaskManagement.Configurations;

public sealed class UseCaseConfiguration : IEntityTypeConfiguration<UseCase>
{
    public void Configure(EntityTypeBuilder<UseCase> b)
    {
        b.Property(u => u.Title).IsRequired().HasMaxLength(100);
        b.Property(u => u.Description).HasMaxLength(1000);
        b.Property(u => u.ImportantNotes).HasMaxLength(500);
        b.Property(u => u.CreatorId).HasConversion(v => v.Value, v => (UserId)v);
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.ModuleId);

        b.HasOne<Module>()
            .WithMany()
            .HasForeignKey(u => u.ModuleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // FK: UseCase.CreatorId -> AspNetUsers.Id
        // Not: ValueObject conversion ile FK tanımı EF Core'da sorun çıkarıyor
        // Migration'da manuel olarak SQL ile ekleniyor
    }
}


