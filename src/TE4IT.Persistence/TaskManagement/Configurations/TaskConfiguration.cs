using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Domain.ValueObjects;
using DomainTask = TE4IT.Domain.Entities.Task;

namespace TE4IT.Persistence.TaskManagement.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<DomainTask>
{
    public void Configure(EntityTypeBuilder<DomainTask> b)
    {
        b.Property(t => t.Title).IsRequired().HasMaxLength(200);
        b.Property(t => t.Description).HasMaxLength(2000);
        b.Property(t => t.ImportantNotes).HasMaxLength(1000);
        b.Property(t => t.CreatorId).HasConversion(v => v.Value, v => (UserId)v);
        b.Property(t => t.AssigneeId).HasConversion(v => v.Value, v => (UserId)v);
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.UseCaseId);
        b.HasIndex(x => x.AssigneeId);

        b.HasOne<TE4IT.Domain.Entities.UseCase>()
            .WithMany()
            .HasForeignKey(t => t.UseCaseId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}


