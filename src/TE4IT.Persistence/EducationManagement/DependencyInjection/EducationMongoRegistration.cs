using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            var loggerFactory = sp.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("EducationMongoRegistration");

            // Connection string kontrolü
            if (string.IsNullOrWhiteSpace(opts.ConnectionString))
            {
                logger?.LogWarning("MongoDB connection string is empty. Education module will not be available.");
                // Boş connection string ile MongoClient oluştur (lazy connection)
                return new MongoClient("mongodb://localhost:27017");
            }

            try
            {
                // MongoClientSettings kullanarak DNS çözümlemesini daha güvenli hale getir
                var settings = MongoClientSettings.FromConnectionString(opts.ConnectionString);
                
                // Connection timeout ayarla (varsayılan 30 saniye)
                settings.ConnectTimeout = TimeSpan.FromSeconds(10);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
                
                // DNS çözümleme hatası durumunda uygulamanın başlamasına izin ver
                settings.SocketTimeout = TimeSpan.FromSeconds(10);
                
                var client = new MongoClient(settings);
                logger?.LogInformation("MongoDB client created successfully");
                return client;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, 
                    "Failed to create MongoDB client. Education module will not be available. " +
                    "Error: {Message}. The application will continue to run but Education endpoints may not work.", 
                    ex.Message);
                
                // Hata durumunda fallback olarak localhost kullan (uygulama çalışmaya devam eder)
                // Not: Bu durumda Education endpoint'leri çalışmayacak ama uygulama başlayacak
                var fallbackSettings = new MongoClientSettings
                {
                    Server = new MongoServerAddress("localhost", 27017),
                    ConnectTimeout = TimeSpan.FromSeconds(5),
                    ServerSelectionTimeout = TimeSpan.FromSeconds(5)
                };
                return new MongoClient(fallbackSettings);
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

