using Microsoft.EntityFrameworkCore;
using TE4IT.Persistence.Common.Contexts;
using Xunit;

namespace TE4IT.Tests.Integration.Common;

/// <summary>
/// Integration testler için base class
/// Her test için izole bir In-Memory DbContext sağlar
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected AppDbContext DbContext { get; }

    protected IntegrationTestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        DbContext = new AppDbContext(options);
    }

    protected async Task SeedDataAsync(params object[] entities)
    {
        foreach (var entity in entities)
        {
            await DbContext.AddAsync(entity);
        }
        await DbContext.SaveChangesAsync();
    }

    protected async Task ClearDatabaseAsync()
    {
        DbContext.Projects.RemoveRange(DbContext.Projects);
        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}

