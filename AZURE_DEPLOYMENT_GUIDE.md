# Azure Deployment Konfigürasyon Rehberi

Bu dokümantasyon, TE4IT API'nin Azure App Service'e deployment edilirken yapılması gereken environment variable ayarlarını içerir.

## 🔧 Azure App Service Configuration

Azure Portal → App Service → Configuration → Application settings bölümüne aşağıdaki değişkenleri ekleyin:

### **1. Environment Settings**

```plaintext
ASPNETCORE_ENVIRONMENT = Production
```

### **2. Database Connection**

```plaintext
CONNECTION_STRING = {Your_Production_PostgreSQL_Connection_String}
```

veya

```plaintext
ConnectionStrings__Pgsql = {Your_Production_PostgreSQL_Connection_String}
```

### **3. JWT Settings**

```plaintext
JWT_ISSUER = https://te4it-api.azurewebsites.net
JWT_AUDIENCE = te4it-api
JWT_SIGNING_KEY = {Your_Production_Signing_Key}
```

veya `appsettings.Production.json` kullanıyorsanız sadece `JWT_SIGNING_KEY` yeterli (diğerleri dosyadan okunur).

### **4. Email Settings**

```plaintext
EMAIL_USERNAME = infoarslanbas@gmail.com
EMAIL_PASSWORD = {Your_Gmail_App_Password}
```

### **5. Frontend URL (ÖNEMLİ!)**

```plaintext
FRONTEND_URL = https://te4it-frontend.up.railway.app
```

Bu değişken olmadan şifre sıfırlama ve welcome email'leri yanlış URL kullanır!

---

## 🎯 Otomatik Environment Yönetimi

Uygulama artık `ASPNETCORE_ENVIRONMENT` değişkenine göre otomatik olarak şu ayarlamaları yapar:

### **Development (localhost:5001)**
- ✅ Swagger Server: `https://localhost:5001`
- ✅ CORS Origins: localhost ports (3000, 5173, 4200)
- ✅ Frontend URL: `http://localhost:3000` (appsettings'ten)
- ✅ Email Reset Link: `http://localhost:3000/reset-password?...`

### **Production (Azure)**
- ✅ Swagger Server: `https://te4it-api.azurewebsites.net`
- ✅ CORS Origins: production domains (up.railway.app, azurewebsites.net)
- ✅ Frontend URL: `https://te4it-frontend.up.railway.app` (env variable'dan)
- ✅ Email Reset Link: `https://te4it-frontend.up.railway.app/reset-password?...`
- ✅ JWT Issuer: `https://te4it-api.azurewebsites.net`

---

## 📦 Deployment Süreci

### **1. GitHub'a Push**
```bash
git add .
git commit -m "Production configuration updates"
git push origin main
```

### **2. Azure Otomatik Deployment**
Azure GitHub webhook'u algılar ve:
- Kodu çeker
- `dotnet publish -c Release` çalıştırır
- Uygulamayı restart eder

### **3. Environment Variables Override**
Configuration öncelik sırası:
```
appsettings.json (Base)
    ↓
appsettings.Production.json (Override)
    ↓
Azure Environment Variables (Final Override)
```

---

## ✅ Deployment Checklist

Deployment öncesi kontrol listesi:

- [x] `appsettings.Production.json` oluşturuldu
- [ ] Azure'da `ASPNETCORE_ENVIRONMENT=Production` ayarlandı
- [ ] Azure'da `FRONTEND_URL` environment variable eklendi
- [ ] Azure'da `JWT_SIGNING_KEY` production key ile güncellendi
- [ ] Azure'da `CONNECTION_STRING` production database ile güncellendi
- [ ] Azure'da email credentials (`EMAIL_USERNAME`, `EMAIL_PASSWORD`) eklendi
- [ ] `.gitignore` içinde hassas bilgiler kontrol edildi
- [ ] GitHub'a push yapıldı
- [ ] Azure deployment logları kontrol edildi
- [ ] Swagger UI açıldı ve test edildi
- [ ] Email gönderimi test edildi (register, forgotPassword)

---

## 🔍 Troubleshooting

### **Email Gönderilmiyor**
- Azure'da `EMAIL_USERNAME` ve `EMAIL_PASSWORD` kontrol edin
- Gmail App Password kullandığınızdan emin olun (normal şifre değil!)

### **Reset Link Yanlış URL Gösteriyor**
- Azure'da `FRONTEND_URL` environment variable kontrolü yapın
- `appsettings.Production.json` içinde `FrontendUrl` ayarını kontrol edin

### **JWT Token Validate Etmiyor**
- Azure'da `JWT_ISSUER` production URL olmalı
- `JWT_SIGNING_KEY` production key ile güncellenmiş olmalı

### **CORS Hatası**
- `ASPNETCORE_ENVIRONMENT=Production` olmalı
- Production CORS origins içinde frontend domain'i var mı kontrol edin

---

## 📚 İlgili Dosyalar

- `src/TE4IT.API/appsettings.json` - Base configuration
- `src/TE4IT.API/appsettings.Development.json` - Development settings
- `src/TE4IT.API/appsettings.Production.json` - Production settings
- `src/TE4IT.API/Configuration/Constants/CorsOrigins.cs` - CORS configuration
- `src/TE4IT.API/Extensions/SwaggerRegistration.cs` - Swagger configuration
- `src/TE4IT.Infrastructure/Common/UrlService.cs` - URL service implementation
- `src/TE4IT.API/Controllers/AuthController.cs` - ForgotPassword endpoint
- `src/TE4IT.Application/Features/Auth/Commands/Register/RegisterCommandHandler.cs` - Welcome email

---

## 🚀 Yeni Özellikler

Bu deployment ile eklenen özellikler:

1. **Environment-Aware URL Service** - Development ve Production için otomatik URL yönetimi
2. **Production appsettings** - Production-specific ayarlar
3. **Fallback Template System** - Email template dosyaları bulunamazsa embedded templates
4. **Professional Email Templates** - HTML email templates
5. **Logging Improvements** - Email gönderim süreci loglanıyor

---

**Son Güncelleme:** 2024
**Versiyon:** 1.0.0

