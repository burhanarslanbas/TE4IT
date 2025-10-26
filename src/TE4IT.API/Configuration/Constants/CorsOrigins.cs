namespace TE4IT.API.Configuration.Constants;

public static class CorsOrigins
{
    public static readonly string[] DevelopmentOrigins = 
    {
        "https://localhost:5173",
        "http://localhost:5173",
        "https://localhost:3000",
        "http://localhost:3000",
        "https://localhost:4200",
        "http://localhost:4200"
    };
    
    public static readonly string[] ProductionOrigins = 
    {
        "https://te4it-api.azurewebsites.net",
        "https://te4it-frontend.up.railway.app",
        "https://*.up.railway.app"
    };
    
    public static string[] GetFrontendOrigins()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return environment == "Development" ? DevelopmentOrigins : ProductionOrigins;
    }
}
