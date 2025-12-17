using System.IO;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using TE4IT.Domain.Entities.Education;

namespace TE4IT.Persistence.EducationManagement.Indexes;

/// <summary>
/// Education koleksiyonları için index kurulumunu yapan hosted service.
/// </summary>
public sealed class EducationIndexInitializer : IHostedService
{
    private readonly IMongoDatabase database;

    public EducationIndexInitializer(IMongoDatabase database)
    {
        this.database = database;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // #region agent log
        var workspaceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var logPath = Path.Combine(workspaceRoot, ".cursor", "debug.log");
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"C\",\"location\":\"EducationIndexInitializer.cs:21\",\"message\":\"Index initialization started\",\"data\":{{\"databaseName\":\"{database.DatabaseNamespace.DatabaseName}\"}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
        // #endregion
        
        try
        {
            // #region agent log
            File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"C\",\"location\":\"EducationIndexInitializer.cs:28\",\"message\":\"Before CreateCoursesIndexesAsync\",\"data\":{{}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
            // #endregion
            
            await CreateCoursesIndexesAsync(cancellationToken);
            
            // #region agent log
            File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"C\",\"location\":\"EducationIndexInitializer.cs:33\",\"message\":\"After CreateCoursesIndexesAsync - before CreateEnrollmentsIndexesAsync\",\"data\":{{}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
            // #endregion
            
            await CreateEnrollmentsIndexesAsync(cancellationToken);
            
            // #region agent log
            File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"C\",\"location\":\"EducationIndexInitializer.cs:38\",\"message\":\"After CreateEnrollmentsIndexesAsync - before CreateProgressIndexesAsync\",\"data\":{{}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
            // #endregion
            
            await CreateProgressIndexesAsync(cancellationToken);
            
            // #region agent log
            File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"C\",\"location\":\"EducationIndexInitializer.cs:43\",\"message\":\"All indexes created successfully\",\"data\":{{}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
            // #endregion
        }
        catch (Exception ex)
        {
            // #region agent log
            File.AppendAllText(logPath, $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"C\",\"location\":\"EducationIndexInitializer.cs:47\",\"message\":\"Index creation failed\",\"data\":{{\"exceptionType\":\"{ex.GetType().Name}\",\"message\":\"{ex.Message.Replace("\"", "\\\"")}\",\"stackTrace\":\"{ex.StackTrace?.Replace("\"", "\\\"").Replace("\n", "\\n").Substring(0, Math.Min(500, ex.StackTrace?.Length ?? 0)) ?? "null"}\"}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n");
            // #endregion
            throw;
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

