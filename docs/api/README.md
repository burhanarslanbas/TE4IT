# API Dokümantasyonu

Bu klasör TE4IT API endpoint'leri ve kullanım kılavuzlarını içerir.

## 📋 İçindekiler

### 🔐 Authentication API
- **[AUTH_API_DOCUMENTATION.md](./AUTH_API_DOCUMENTATION.md)** - Kimlik doğrulama endpoint'leri
  - Kullanıcı kaydı ve girişi
  - Token yönetimi (JWT + Refresh Token)
  - Şifre sıfırlama
  - JavaScript, React Native, Kotlin örnekleri

### 🚧 Yakında Gelecek
- **Projects API** - Proje yönetimi endpoint'leri
- **Tasks API** - Görev yönetimi endpoint'leri  
- **Education API** - Eğitim modülü endpoint'leri
- **AI API** - AI servisi endpoint'leri

## 🚀 Hızlı Başlangıç

### 1. Production API'ye Erişim
- **Live API**: `https://te4it-api.azurewebsites.net`
- **Swagger UI**: `https://te4it-api.azurewebsites.net/swagger`

### 2. Local Development
```bash
cd src/TE4IT.API
dotnet run
```
- **Local API**: `https://localhost:5001`
- **Local Swagger**: `https://localhost:5001/swagger`

### 3. Authentication Flow
```javascript
// JavaScript örneği - Production
const auth = new TE4ITAuth('https://te4it-api.azurewebsites.net');

// Kayıt ol
await auth.register('johndoe', 'john@example.com', 'Password123!');

// Giriş yap
await auth.login('john@example.com', 'Password123!');

// Authenticated request
const projects = await auth.makeAuthenticatedRequest('/api/v1/projects');
```

## 📚 API Genel Bilgileri

### Base URLs
```
Production: https://te4it-api.azurewebsites.net/api/v1
Development: https://localhost:5001/api/v1
```

### Authentication
- **Type**: JWT Bearer Token
- **Header**: `Authorization: Bearer {accessToken}`
- **Refresh**: Otomatik token yenileme

### Response Format
- **Success**: JSON response
- **Error**: RFC 7807 Problem Details format
- **Status Codes**: HTTP standart kodları

### Rate Limiting
- **Login**: Saatte 10 istek
- **Register**: Saatte 5 istek
- **Refresh Token**: Saatte 5 istek

## 🔗 Hızlı Linkler

- **[Authentication API](./AUTH_API_DOCUMENTATION.md)** - Detaylı auth dokümantasyonu
- **[Live Swagger UI](https://te4it-api.azurewebsites.net/swagger)** - Production API testi
- **[Local Swagger UI](https://localhost:5001/swagger)** - Development API testi
- **[Postman Collection](./postman/)** - Hazır API collection'ları (yakında)

## 📞 Destek

API kullanımında sorun yaşarsanız:
1. Swagger UI'da test edin
2. İlgili dokümantasyonu kontrol edin
3. GitHub Issues'da sorun bildirin
