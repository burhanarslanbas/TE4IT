# 🚀 Railway Deployment - Adım Adım Rehber

## ✅ Hazırlık Tamamlandı!

Kodunuz GitHub'a push edildi ve Railway deployment için hazır. Şimdi Railway'de deployment yapalım.

## 📋 Railway Deployment Adımları

### 1. Railway Hesabı Oluşturma
1. [railway.app](https://railway.app) adresine gidin
2. "Sign Up" butonuna tıklayın
3. GitHub hesabınızla giriş yapın

### 2. Yeni Proje Oluşturma
1. Railway dashboard'da "New Project" butonuna tıklayın
2. "Deploy from GitHub repo" seçin
3. TE4IT repository'nizi seçin
4. "Deploy Now" butonuna tıklayın

### 3. Environment Variables Ayarlama
Railway otomatik olarak projenizi deploy etmeye başlayacak, ancak environment variables'ları ayarlamamız gerekiyor:

1. Deploy edilen servisinize tıklayın
2. "Variables" sekmesine gidin
3. Aşağıdaki environment variables'ları ekleyin:

```
CONNECTION_STRING=User Id=postgres.chrisnrtblexlktxwetq;Password=h+Hgg6sq@@c7#+W;Server=aws-1-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Command Timeout=120;Timeout=120

JWT_ISSUER=https://te4it-api-production.up.railway.app
JWT_AUDIENCE=te4it-api
JWT_SIGNING_KEY=ToAyJiE3OPBfbv7GN0elgLNxxEwmXtd+7EyC76yxiss=

EMAIL_USERNAME=infoarslanbas@gmail.com
EMAIL_PASSWORD=rzowjgsyptscottc
```

### 4. Build Ayarları Kontrolü
Railway otomatik olarak .NET projesini algılayacak, ancak kontrol edelim:

1. "Settings" sekmesine gidin
2. "Build" bölümünde:
   - **Root Directory**: `src/TE4IT.API`
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet TE4IT.API.dll`

### 5. Domain Ayarları
1. "Settings" sekmesinde "Domains" bölümüne gidin
2. Railway otomatik olarak bir domain verecek: `https://te4it-api-production.up.railway.app`
3. İsterseniz custom domain ekleyebilirsiniz

## 🔍 Deployment Sonrası Test

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

## 📱 Arkadaşlarınızla Paylaşım

### API Base URL
```
https://te4it-api-production.up.railway.app
```

### Swagger Documentation
```
https://te4it-api-production.up.railway.app/swagger
```

### Örnek Kullanım
```javascript
// Frontend'de API çağrısı
const API_BASE_URL = 'https://te4it-api-production.up.railway.app';

// Register endpoint
fetch(`${API_BASE_URL}/api/auth/register`, {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    email: 'user@example.com',
    password: 'SecurePassword123!'
  })
});
```

## 🔒 Güvenlik Notları

✅ **Yapılanlar:**
- Hassas bilgiler environment variables'da saklanıyor
- CORS ayarları production için güncellendi
- User Secrets development için kullanılıyor

⚠️ **Dikkat Edilecekler:**
- Railway dashboard'da environment variables'ları kontrol edin
- API endpoint'lerini test edin
- Log'ları kontrol edin

## 📊 Monitoring

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
Railway dashboard'da "Logs" sekmesinden real-time log'ları görüntüleyebilirsiniz.

## 🎉 Tebrikler!

API'niz başarıyla Railway'de deploy edildi ve arkadaşlarınızla paylaşmaya hazır!
