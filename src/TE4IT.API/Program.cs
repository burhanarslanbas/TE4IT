using TE4IT.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Azure App Service için environment variables'ları ekle
builder.Configuration.AddEnvironmentVariables();

// Service Registration - Clean ve organized
builder.Services
    .AddApiServices()
    .AddApplicationServices(builder.Configuration, builder.Environment)
    .AddSecurityServices(builder.Configuration);

var app = builder.Build();

// Middleware Pipeline - Clean ve organized
app.ConfigureMiddlewarePipeline();

// Startup Operations - Clean ve organized
await app.InitializeStartupAsync();

app.Run();
