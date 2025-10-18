# 🚀 Railway Deployment Rehberi - TE4IT API

## 📋 Ön Gereksinimler

1. **Railway Hesabı**: [railway.app](https://railway.app) üzerinde hesap oluşturun
2. **GitHub Repository**: Kodunuz GitHub'da olmalı
3. **Supabase Projesi**: Production veritabanı için Supabase projesi

## 🔐 Environment Variables Ayarları

Railway dashboard'da aşağıdaki environment variables'ları ayarlayın:

### Veritabanı Konfigürasyonu
```
CONNECTION_STRING=User Id=postgres.[YOUR_SUPABASE_USER];Password=[YOUR_SUPABASE_PASSWORD];Server=aws-1-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Command Timeout=120;Timeout=120
```

### JWT Konfigürasyonu
```
JWT_ISSUER=https://te4it-api.up.railway.app
JWT_AUDIENCE=te4it-api
JWT_SIGNING_KEY=[YOUR_SECURE_JWT_KEY_32_CHARS_MIN]
```

### Email Konfigürasyonu
```
EMAIL_USERNAME=[YOUR_EMAIL_USERNAME]
EMAIL_PASSWORD=[YOUR_EMAIL_APP_PASSWORD]
```

## 🛠️ Railway Deployment Adımları

### 1. Railway'e Proje Bağlama
1. [Railway Dashboard](https://railway.app/dashboard)'a gidin
2. "New Project" butonuna tıklayın
3. "Deploy from GitHub repo" seçin
4. TE4IT repository'nizi seçin

### 2. Build Ayarları
Railway otomatik olarak .NET projesini algılayacak, ancak manuel ayarlar:

- **Root Directory**: `src/TE4IT.API`
- **Build Command**: `dotnet publish -c Release -o out`
- **Start Command**: `dotnet TE4IT.API.dll`

### 3. Environment Variables Ekleme
1. Railway dashboard'da projenizi seçin
2. "Variables" sekmesine gidin
3. Yukarıdaki environment variables'ları ekleyin

### 4. Domain Ayarları
1. "Settings" sekmesine gidin
2. "Domains" bölümünde custom domain ekleyebilirsiniz
3. Varsayılan Railway domain'i: `https://te4it-api-production.up.railway.app`

## 🔍 Deployment Sonrası Kontroller

### API Endpoints Test
```bash
# Health Check
curl https://te4it-api-production.up.railway.app/swagger

# Auth Test
curl -X POST https://te4it-api-production.up.railway.app/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'
```

### Swagger UI
- **URL**: `https://te4it-api-production.up.railway.app/swagger`
- API dokümantasyonunu buradan görüntüleyebilirsiniz

## 🔒 Güvenlik Notları

1. **Environment Variables**: Asla kod içinde hardcode etmeyin
2. **JWT Key**: En az 32 karakter uzunluğunda güçlü bir key kullanın
3. **CORS**: Sadece gerekli origin'lere izin verin
4. **HTTPS**: Railway otomatik olarak HTTPS sağlar

## 📊 Monitoring ve Logs

Railway dashboard'da:
- **Metrics**: CPU, Memory, Network kullanımı
- **Logs**: Real-time log görüntüleme
- **Deployments**: Deployment geçmişi

## 🚨 Sorun Giderme

### Yaygın Sorunlar:
1. **Build Hatası**: Environment variables eksik olabilir
2. **Database Bağlantı Hatası**: Connection string kontrol edin
3. **CORS Hatası**: Frontend URL'i CORS listesinde olmalı

### Log Kontrolü:
```bash
# Railway CLI ile log görüntüleme
railway logs
```

## 📱 Frontend Entegrasyonu

Frontend uygulamanızda API base URL'i güncelleyin:
```typescript
const API_BASE_URL = 'https://te4it-api-production.up.railway.app';
```

## 🔄 Otomatik Deployment

GitHub'a push yaptığınızda Railway otomatik olarak:
1. Kodu çeker
2. Build eder
3. Deploy eder
4. Health check yapar

## 📞 Destek

- Railway Docs: [docs.railway.app](https://docs.railway.app)
- Railway Discord: [discord.gg/railway](https://discord.gg/railway)
