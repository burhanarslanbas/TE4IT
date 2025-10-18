using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TE4IT.Persistence.Common.Entities.Relational;

namespace TE4IT.Persistence.Common.Configurations.Relational;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.EventType).IsRequired().HasMaxLength(256);
        b.Property(x => x.Payload).IsRequired();
        b.HasIndex(x => x.OccurredAt);
        b.HasIndex(x => x.ProcessedAt);
    }
}
