using Microsoft.OpenApi.Models;

namespace TE4IT.API.Extensions;

public static class SwaggerRegistration
{
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TE4IT API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer {token}"
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


