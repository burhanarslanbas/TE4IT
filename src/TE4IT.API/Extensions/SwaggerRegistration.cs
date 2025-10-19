using Microsoft.OpenApi.Models;

namespace TE4IT.API.Extensions;

public static class SwaggerRegistration
{
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "TE4IT API", 
                Version = "v1",
                Description = "Task Management API for TE4IT Project",
                Contact = new OpenApiContact
                {
                    Name = "TE4IT Team",
                    Email = "infoarslanbas@gmail.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License"
                }
            });
            
            // Azure App Service için server bilgisi ekle
            c.AddServer(new OpenApiServer
            {
                Url = "https://te4it-api.azurewebsites.net",
                Description = "Azure App Service Production Server"
            });
            
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app, bool redirectRoot = true)
    {
        app.UseSwagger();
        app.UseStaticFiles();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TE4IT API V1");
        });
        if (redirectRoot && app is WebApplication wapp)
        {
            wapp.MapGet("/", () => Results.Redirect("/swagger"));
        }
        return app;
    }
}


