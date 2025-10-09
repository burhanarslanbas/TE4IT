using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TE4IT.Persistence.Relational.Db.Configurations;

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
