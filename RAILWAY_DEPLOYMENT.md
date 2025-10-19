# Railway Deployment Configuration

## .NET 9.0 Configuration

Bu proje .NET 9.0 kullanır ve Railway'de otomatik olarak algılanır.

## Environment Variables

Aşağıdaki environment variables'ları Railway dashboard'da ayarlayın:

```
CONNECTION_STRING=[SUPABASE_CONNECTION_STRING]

JWT_ISSUER=https://your-app-name.up.railway.app
JWT_AUDIENCE=te4it-api
JWT_SIGNING_KEY=[YOUR_JWT_SIGNING_KEY]

EMAIL_USERNAME=[YOUR_EMAIL_USERNAME]
EMAIL_PASSWORD=[YOUR_EMAIL_PASSWORD]
```

## Build Settings

- **Root Directory:** `src/TE4IT.API`
- **Build Command:** `dotnet publish -c Release -o out`
- **Start Command:** `dotnet TE4IT.API.dll`

## API Endpoints

Deployment sonrası:
- **Base URL:** `https://your-app-name.up.railway.app`
- **Swagger UI:** `https://your-app-name.up.railway.app/swagger`
- **Health Check:** `https://your-app-name.up.railway.app/swagger`
