using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
}
