namespace TE4IT.API.Configuration.Constants;

public static class CorsOrigins
{
    public static readonly string[] FrontendOrigins = 
    {
        "https://localhost:5173",
        "http://localhost:5173",
        "https://localhost:4200",
        "http://localhost:4200",
        "https://te4it-api.azurewebsites.net", // Azure API domain
        "https://te4it-frontend.up.railway.app", // Railway frontend URL'i
        "https://*.up.railway.app" // Railway subdomain'leri için
    };
}
