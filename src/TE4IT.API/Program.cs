using TE4IT.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Service Registration - Clean ve organized
builder.Services
    .AddApiServices()
    .AddApplicationServices(builder.Configuration)
    .AddSecurityServices(builder.Configuration);

var app = builder.Build();

// Middleware Pipeline - Clean ve organized
app.ConfigureMiddlewarePipeline();

// Startup Operations - Clean ve organized
await app.InitializeStartupAsync();

app.Run();
