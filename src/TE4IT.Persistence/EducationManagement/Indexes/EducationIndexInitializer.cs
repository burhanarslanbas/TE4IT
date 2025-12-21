using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Indexes;

/// <summary>
/// Education koleksiyonları için index kurulumunu yapan hosted service.
/// </summary>
public sealed class EducationIndexInitializer : IHostedService
{
    private readonly IMongoDatabase database;
    private readonly ILogger<EducationIndexInitializer>? logger;

    public EducationIndexInitializer(IMongoDatabase database, ILogger<EducationIndexInitializer>? logger = null)
    {
        this.database = database;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // MongoDB bağlantısını test et - bağlantı yoksa index oluşturma
        try
        {
            // MongoDB bağlantısını test et (timeout ile)
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await database.Client.ListDatabaseNamesAsync(cts.Token);
            
            logger?.LogInformation("MongoDB connection successful. Creating indexes for Education module...");
            
            await CreateCoursesIndexesAsync(cancellationToken);
            await CreateEnrollmentsIndexesAsync(cancellationToken);
            await CreateProgressIndexesAsync(cancellationToken);
            
            logger?.LogInformation("MongoDB indexes for Education module created successfully");
        }
        catch (OperationCanceledException)
        {
            logger?.LogWarning(
                "MongoDB connection timeout. Education module indexes will be created on first use. " +
                "Please check your MongoDB connection string and network connectivity.");
        }
        catch (Exception ex)
        {
            // Index oluşturma hatası uygulamanın başlamasını engellemez
            // Sadece log'la ve devam et
            logger?.LogError(ex, 
                "Failed to create MongoDB indexes for Education module. " +
                "Indexes will be created on first use. Error: {Message}", 
                ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private Task CreateCoursesIndexesAsync(CancellationToken ct)
    {
        // #region agent log
        var workspaceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var logPath = Path.Combine(workspaceRoot, ".cursor", "debug.log");
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"D\",\"location\":\"EducationIndexInitializer.cs:62\",\"message\":\"CreateCoursesIndexesAsync - before GetCollection\",\"data\":{{}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
        // #endregion
        
        var collection = database.GetCollection<Course>("courses");

        // #region agent log
        File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"D\",\"location\":\"EducationIndexInitializer.cs:66\",\"message\":\"CreateCoursesIndexesAsync - before CreateManyAsync\",\"data\":{{\"indexCount\":2}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
        // #endregion

        var indexes = new[]
        {
            new CreateIndexModel<Course>(
                Builders<Course>.IndexKeys
                    .Ascending(c => c.IsActive)
                    .Descending(c => c.CreatedDate)),
            new CreateIndexModel<Course>(
                Builders<Course>.IndexKeys
                    .Ascending("roadmap.steps.order"))
        };

        return collection.Indexes.CreateManyAsync(indexes, ct);
    }

    private Task CreateEnrollmentsIndexesAsync(CancellationToken ct)
    {
        var collection = database.GetCollection<Enrollment>("enrollments");

        var indexes = new[]
        {
            new CreateIndexModel<Enrollment>(
                Builders<Enrollment>.IndexKeys
                    .Ascending(e => e.UserId)
                    .Ascending(e => e.CourseId),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<Enrollment>(
                Builders<Enrollment>.IndexKeys
                    .Ascending(e => e.CourseId)),
            new CreateIndexModel<Enrollment>(
                Builders<Enrollment>.IndexKeys
                    .Ascending(e => e.UserId)
                    .Ascending(e => e.IsActive))
        };

        return collection.Indexes.CreateManyAsync(indexes, ct);
    }

    private Task CreateProgressIndexesAsync(CancellationToken ct)
    {
        var collection = database.GetCollection<Progress>("progresses");

        var indexes = new[]
        {
            new CreateIndexModel<Progress>(
                Builders<Progress>.IndexKeys
                    .Ascending(p => p.UserId)
                    .Ascending(p => p.ContentId)
                    .Ascending(p => p.CourseId),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<Progress>(
                Builders<Progress>.IndexKeys
                    .Ascending(p => p.EnrollmentId)),
            new CreateIndexModel<Progress>(
                Builders<Progress>.IndexKeys
                    .Ascending(p => p.CourseId)
                    .Ascending(p => p.UserId)),
            new CreateIndexModel<Progress>(
                Builders<Progress>.IndexKeys
                    .Ascending(p => p.CourseId)
                    .Ascending(p => p.StepId))
        };

        return collection.Indexes.CreateManyAsync(indexes, ct);
    }
}

