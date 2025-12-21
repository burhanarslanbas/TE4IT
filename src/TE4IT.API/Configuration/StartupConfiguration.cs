using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using TE4IT.Infrastructure.Auth.Services;
using TE4IT.Persistence.Common.Contexts;

namespace TE4IT.API.Configuration;

public static class StartupConfiguration
{
    public static async Task InitializeStartupAsync(this WebApplication app)
    {
        // Configuration'dan seeding ayarını kontrol et
        var configuration = app.Services.GetRequiredService<IConfiguration>();
        var enableSeeding = configuration.GetValue<bool>("RoleSeeding:Enabled");
        
        if (enableSeeding)
        {
            await app.SeedDatabaseAsync();
        }
        
        // EF Core Model Warm-up - İlk sorguyu burada yaparak model'i önceden oluştur
        await app.WarmupDbContextAsync();
        
        // MongoDB Warm-up - İlk bağlantıyı burada kurarak cold start'ı azalt
        await app.WarmupMongoDbAsync();
    }
    
    private static async Task WarmupDbContextAsync(this WebApplication app)
    {
        try
        {
            // Test ortamında warm-up'ı atla (In-Memory DB zaten hazır)
            var environment = app.Environment.EnvironmentName;
            if (environment == "Testing")
            {
                return;
            }

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Basit bir sorgu ile model'i warm-up yap
            // Bu, EF Core'un model'i oluşturmasını ve cache'lemesini sağlar
            _ = await dbContext.Database.CanConnectAsync();
            
            // Model'in tamamen yüklendiğinden emin olmak için basit bir query yap
            // Identity tablolarından birini kullanarak tüm model'in yüklendiğini garanti ederiz
            _ = await dbContext.Set<IdentityRole<Guid>>().CountAsync();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(ex, "EF Core warm-up failed, but continuing startup. First request may be slower.");
            // Warm-up başarısız olsa bile startup'ı durdurmuyoruz
            // İlk istek sırasında model oluşturulacak
        }
    }
    
    private static async Task SeedDatabaseAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            logger.LogInformation("Starting database seeding...");
            
            var roleSeeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
            
            // Timeout ile seeding işlemi - 10 dakika timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
            await roleSeeder.SeedDefaultRolesAsync(cts.Token);
            
            logger.LogInformation("Database seeding completed successfully");
        }
        catch (OperationCanceledException)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError("Database seeding timed out after 10 minutes");
            throw;
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Database seeding failed");
            throw;
        }
    }
    
    private static async Task WarmupMongoDbAsync(this WebApplication app)
    {
        try
        {
            // Test ortamında warm-up'ı atla
            var environment = app.Environment.EnvironmentName;
            if (environment == "Testing")
            {
                return;
            }

            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            
            // MongoDB client'ı al (singleton olduğu için direkt alabiliriz)
            var mongoClient = scope.ServiceProvider.GetService<IMongoClient>();
            var mongoDatabase = scope.ServiceProvider.GetService<IMongoDatabase>();
            
            if (mongoClient == null || mongoDatabase == null)
            {
                logger?.LogDebug("MongoDB services not registered, skipping warm-up.");
                return;
            }

            logger?.LogInformation("Starting MongoDB warm-up...");
            
            // MongoDB bağlantısını test et ve ilk sorguyu yap
            // Bu, connection pool'un oluşturulmasını ve DNS çözümlemesini tetikler
            // DNS çözümleme sorunları olabileceği için timeout'u artırıyoruz
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            
            try
            {
                await mongoClient.ListDatabaseNamesAsync(cts.Token);
                
                // Database'in erişilebilir olduğunu doğrula
                await mongoDatabase.RunCommandAsync<BsonDocument>(
                    new BsonDocument("ping", 1), 
                    cancellationToken: cts.Token);
                
                logger?.LogInformation("MongoDB warm-up completed successfully.");
            }
            catch (OperationCanceledException)
            {
                logger?.LogWarning("MongoDB warm-up timed out after 60 seconds. This may indicate DNS or network issues.");
                throw; // Timeout durumunda exception'ı yeniden fırlat
            }
        }
        catch (OperationCanceledException)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("MongoDB warm-up timed out after 60 seconds. This may indicate DNS or network issues. " +
                            "Application will continue, but MongoDB operations may fail. " +
                            "Please check your MongoDB connection string and network connectivity.");
            // Warm-up başarısız olsa bile startup'ı durdurmuyoruz
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(ex, "MongoDB warm-up failed, but continuing startup. " +
                            "This may indicate DNS resolution issues or network connectivity problems. " +
                            "First request may be slower or may fail. " +
                            "Please check your MongoDB connection string and network connectivity.");
            // Warm-up başarısız olsa bile startup'ı durdurmuyoruz
        }
    }
}
