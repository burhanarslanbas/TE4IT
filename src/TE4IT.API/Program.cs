using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TE4IT.API.Extensions;
using TE4IT.API.Middlewares;
using TE4IT.Application;
using TE4IT.Infrastructure;
using TE4IT.Persistence.Relational.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", p => p
        .WithOrigins(
            "https://localhost:5173",
            "http://localhost:5173",
            "https://localhost:4200",
            "http://localhost:4200"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});
builder.Services.AddSwaggerDocs();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed-refresh", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
        opt.QueueLimit = 0;
    });
});

// Add services to the container. (Application, Infrastructure, Persistence)
builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddRelational(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("Pgsql")));

// Add Identity and Jwt Authentication to the container.
builder.Services.AddIdentityAndJwtAuthentication(builder.Configuration);

// Add Role Seeder
builder.Services.AddScoped<TE4IT.Infrastructure.Auth.Services.RoleSeeder>();

var app = builder.Build();

// ────────────────────────────────────────────────────
// MIDDLEWARE PIPELINE (Sıralama önemli!)
// ────────────────────────────────────────────────────

// 1️⃣ EN ÖNCE: Global Exception Middleware (tüm hataları yakalar)
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUI(redirectRoot: true);
}

// Always redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable CORS for frontend origins
app.UseCors("Frontend");

// Use Authentication and Authorization. 
app.UseAuthentication();
// Optional simple fixed-window rate limiter for refresh endpoint
app.UseRateLimiter(new RateLimiterOptions());
app.UseAuthorization();
app.MapControllers();

// Seed default roles on startup
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<TE4IT.Infrastructure.Auth.Services.RoleSeeder>();
    await roleSeeder.SeedDefaultRolesAsync();
}

app.Run();
