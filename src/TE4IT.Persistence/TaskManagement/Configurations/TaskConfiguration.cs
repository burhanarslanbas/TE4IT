using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Domain.ValueObjects;
using TE4IT.Persistence.Common.Identity;
using DomainTask = TE4IT.Domain.Entities.Task;

namespace TE4IT.Persistence.TaskManagement.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<DomainTask>
{
    public void Configure(EntityTypeBuilder<DomainTask> b)
    {
        b.Property(t => t.Title).IsRequired().HasMaxLength(200);
        b.Property(t => t.Description).HasMaxLength(2000);
        b.Property(t => t.ImportantNotes).HasMaxLength(1000);
        b.Property(t => t.CompletionNote).HasMaxLength(2000).IsRequired(false);
        b.Property(t => t.CompletedDate).IsRequired(false);
        b.Property(t => t.CreatorId).HasConversion(v => v.Value, v => (UserId)v);
        b.Property(t => t.AssigneeId)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v != null ? (UserId)v.Value : null)
            .IsRequired(false);
        b.Property(t => t.StartedDate).IsRequired(false);
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.UseCaseId);
        b.HasIndex(x => x.AssigneeId);

        b.HasOne<TE4IT.Domain.Entities.UseCase>()
            .WithMany()
            .HasForeignKey(t => t.UseCaseId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // FK: Task.CreatorId -> AspNetUsers.Id
        // FK: Task.AssigneeId -> AspNetUsers.Id
        // Not: ValueObject conversion ile FK tanımı EF Core'da sorun çıkarıyor
        // Migration'da manuel olarak SQL ile ekleniyor
    }
}


