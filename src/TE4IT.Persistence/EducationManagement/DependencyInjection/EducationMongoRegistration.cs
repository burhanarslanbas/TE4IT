using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Courses;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Enrollments;
using TE4IT.Application.Abstractions.Persistence.Repositories.Education.Progresses;
using TE4IT.Persistence.EducationManagement.Indexes;
using TE4IT.Persistence.EducationManagement.Mappings;
using TE4IT.Persistence.EducationManagement.Options;
using TE4IT.Persistence.EducationManagement.Repositories.Courses;
using TE4IT.Persistence.EducationManagement.Repositories.Enrollments;
using TE4IT.Persistence.EducationManagement.Repositories.Progresses;

namespace TE4IT.Persistence.EducationManagement.DependencyInjection;

public static class EducationMongoRegistration
{
    public static IServiceCollection AddEducationMongo(this IServiceCollection services, IConfiguration configuration)
    {
            // MongoDB mapping'lerini kaydet (Guid'ler string olarak saklanacak)
        EducationMongoMappings.RegisterMappings();

        services.Configure<MongoOptions>(configuration.GetSection("MongoDB"));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;

            // #region agent log
            try
            {
                var workspaceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
                var logPath = Path.Combine(workspaceRoot, ".cursor", "debug.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

                var hasConnectionString = !string.IsNullOrWhiteSpace(opts.ConnectionString);
                var isSrvFormat = hasConnectionString && opts.ConnectionString.StartsWith("mongodb+srv://", StringComparison.OrdinalIgnoreCase);

                var logLine = $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"A\",\"location\":\"EducationMongoRegistration.cs:28\",\"message\":\"Creating MongoClient\",\"data\":{{\"hasConnectionString\":{hasConnectionString.ToString().ToLower()},\"isSrvFormat\":{isSrvFormat.ToString().ToLower()},\"connectionStringLength\":{opts.ConnectionString?.Length ?? 0}}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}{Environment.NewLine}";
                File.AppendAllText(logPath, logLine);
            }
            catch
            {
                // Debug log yazılamazsa uygulamayı bozma
            }
            // #endregion

            try
            {
                return new MongoClient(opts.ConnectionString);
            }
            catch (Exception ex)
            {
                // #region agent log
                try
                {
                    var workspaceRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
                    var logPath = Path.Combine(workspaceRoot, ".cursor", "debug.log");
                    Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

                    var logLine = $"{{\"sessionId\":\"debug-session\",\"runId\":\"run1\",\"hypothesisId\":\"B\",\"location\":\"EducationMongoRegistration.cs:48\",\"message\":\"MongoClient creation failed\",\"data\":{{\"exceptionType\":\"{ex.GetType().Name}\",\"message\":\"{ex.Message.Replace("\"", "\\\"")}\"}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}{Environment.NewLine}";
                    File.AppendAllText(logPath, logLine);
                }
                catch
                {
                    // Debug log yazılamazsa uygulamayı bozma
                }
                // #endregion

                throw;
            }
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(opts.DatabaseName);
        });
        services.AddScoped<ICourseReadRepository, CourseReadRepository>();
        services.AddScoped<ICourseWriteRepository, CourseWriteRepository>();
        services.AddScoped<IEnrollmentReadRepository, EnrollmentReadRepository>();
        services.AddScoped<IEnrollmentWriteRepository, EnrollmentWriteRepository>();
        services.AddScoped<IProgressReadRepository, ProgressReadRepository>();
        services.AddScoped<IProgressWriteRepository, ProgressWriteRepository>();

        services.AddHostedService<EducationIndexInitializer>();

        return services;
    }
}

