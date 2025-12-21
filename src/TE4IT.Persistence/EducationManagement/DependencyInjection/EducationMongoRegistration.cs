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
    /// <summary>
    /// Registers MongoDB persistence for the Education bounded context.
    /// <list type="bullet">
    /// <item>Binds <see cref="MongoOptions"/> from <c>MongoDB</c> configuration section.</item>
    /// <item>Registers Mongo mappings (Guids as string, embedded documents, etc.).</item>
    /// <item>Registers <see cref="IMongoClient"/> and <see cref="IMongoDatabase"/> as singletons.</item>
    /// <item>Registers Education read/write repositories and index initializer.</item>
    /// </list>
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEducationMongo(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MongoDB mapping'lerini bir kez kaydet
        EducationMongoMappings.RegisterMappings();

        // "MongoDB" section'ını MongoOptions ile eşle
        services.Configure<MongoOptions>(configuration.GetSection("MongoDB"));

        // IMongoClient singleton
        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException(
                    "MongoDB connection string is not configured. Please set 'MongoDB:ConnectionString' in appsettings or environment variables.");
            }

            return new MongoClient(options.ConnectionString);
        });

        // IMongoDatabase singleton
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();

            var databaseName = string.IsNullOrWhiteSpace(options.DatabaseName)
                ? throw new InvalidOperationException(
                    "MongoDB database name is not configured. Please set 'MongoDB:DatabaseName' in configuration or provide it in the connection string.")
                : options.DatabaseName;

            return client.GetDatabase(databaseName);
        });

        // Repository kayıtları (scoped)
        services.AddRepositories();

        // Index initializer - application startup'ta index'leri kurar
        services.AddHostedService<EducationIndexInitializer>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICourseReadRepository, CourseReadRepository>();
        services.AddScoped<ICourseWriteRepository, CourseWriteRepository>();
        services.AddScoped<IEnrollmentReadRepository, EnrollmentReadRepository>();
        services.AddScoped<IEnrollmentWriteRepository, EnrollmentWriteRepository>();
        services.AddScoped<IProgressReadRepository, ProgressReadRepository>();
        services.AddScoped<IProgressWriteRepository, ProgressWriteRepository>();
        return services;
    }
}