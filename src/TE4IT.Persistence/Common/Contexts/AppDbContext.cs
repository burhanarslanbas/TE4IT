using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TE4IT.Domain.Entities;
using TE4IT.Domain.Entities.Common;
using TE4IT.Persistence.Common.Entities.Relational;
using TE4IT.Persistence.Common.Identity;
using DomainTask = TE4IT.Domain.Entities.Task;

namespace TE4IT.Persistence.Common.Contexts;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<UseCase> UseCases => Set<UseCase>();
    public DbSet<DomainTask> Tasks => Set<DomainTask>();
    public DbSet<TaskRelation> TaskRelations => Set<TaskRelation>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // CommandTimeout ayarı - Migration'lar için 2 dakika
            optionsBuilder.UseNpgsql(options => options.CommandTimeout(120));
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var utcNow = DateTime.UtcNow;
        foreach (var e in entries)
        {
            if (e.State == EntityState.Added)
            {
                e.Entity.CreatedDate = utcNow;
            }
            if (e.State == EntityState.Modified)
            {
                e.Entity.UpdatedDate = utcNow;
            }
        }
        // Capture domain events into Outbox
        var aggregates = ChangeTracker.Entries<AggregateRoot>().Where(x => x.Entity.DomainEvents.Any()).ToList();
        var outbox = Set<OutboxMessage>();
        foreach (var agg in aggregates)
        {
            foreach (var evt in agg.Entity.DomainEvents)
            {
                var msg = new OutboxMessage
                {
                    EventType = evt.EventType,
                    Payload = JsonSerializer.Serialize(evt),
                    OccurredAt = evt.OccurredAt
                };
                await outbox.AddAsync(msg, cancellationToken);
            }
            agg.Entity.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}


