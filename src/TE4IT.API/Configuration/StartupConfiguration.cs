using Microsoft.Extensions.Logging;
using TE4IT.Infrastructure.Auth.Services;

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
        else
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Role seeding is disabled. Set RoleSeeding:Enabled=true in appsettings to enable.");
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
