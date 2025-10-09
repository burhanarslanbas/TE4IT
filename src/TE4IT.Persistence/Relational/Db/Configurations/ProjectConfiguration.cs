using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Domain.Entities;
using TE4IT.Domain.ValueObjects;

namespace TE4IT.Persistence.Relational.Db.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> b)
    {
        b.Property(p => p.Title).IsRequired().HasMaxLength(100);
        b.Property(p => p.Description).HasMaxLength(1000);
        b.Property(p => p.CreatorId).HasConversion(v => v.Value, v => (UserId)v);
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.StartedDate);
    }
}


