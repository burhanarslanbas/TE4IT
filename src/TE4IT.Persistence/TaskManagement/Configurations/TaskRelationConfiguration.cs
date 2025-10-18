using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainTask = TE4IT.Domain.Entities.Task;
using TE4IT.Domain.Entities;

namespace TE4IT.Persistence.TaskManagement.Configurations;

public sealed class TaskRelationConfiguration : IEntityTypeConfiguration<TaskRelation>
{
    public void Configure(EntityTypeBuilder<TaskRelation> b)
    {
        b.HasKey(x => x.Id);

        b.HasIndex(x => x.SourceTaskId);
        b.HasIndex(x => x.TargetTaskId);
        b.HasIndex(x => new { x.SourceTaskId, x.TargetTaskId, x.RelationType }).IsUnique();

        b.HasOne<DomainTask>()
            .WithMany(t => t.Relations)
            .HasForeignKey(tr => tr.SourceTaskId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<DomainTask>()
            .WithMany()
            .HasForeignKey(tr => tr.TargetTaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


