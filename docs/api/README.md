# API Dokümantasyonu

Bu klasör TE4IT API endpoint'leri ve kullanım kılavuzlarını içerir.

## 📋 İçindekiler

### 🔐 Authentication API
- **[AUTH_API_DOCUMENTATION.md](./AUTH_API_DOCUMENTATION.md)** - Kimlik doğrulama endpoint'leri
  - Kullanıcı kaydı ve girişi
  - Token yönetimi (JWT + Refresh Token)
  - Şifre sıfırlama (Email ile)
  - Uygulama içi şifre değiştirme
  - JavaScript, React Native, Kotlin örnekleri

### 🎨 Frontend Entegrasyon Rehberi
- **[FRONTEND_INTEGRATION_GUIDE.md](./FRONTEND_INTEGRATION_GUIDE.md)** - Frontend geliştiriciler için detaylı rehber
  - Tüm auth endpoint'leri için adım adım rehberler
  - React Context ve Hook'lar
  - Email link handling (sadece web)
  - Şifre sıfırlama süreci (forgotPassword + resetPassword)
  - Güvenlik en iyi uygulamaları
  - Test senaryoları

### 📱 Mobil Entegrasyon Rehberi
- **[MOBILE_INTEGRATION_GUIDE.md](./MOBILE_INTEGRATION_GUIDE.md)** - Mobil geliştiriciler için detaylı rehber
  - Tüm auth endpoint'leri için adım adım rehberler
  - Android (Kotlin) tam entegrasyonu
  - iOS (Swift) entegrasyonu
  - React Native entegrasyonu
  - Şifre değiştirme (sadece changePassword)
  - Web'e yönlendirme (şifremi unuttum)

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

// Şifre sıfırlama isteği gönder
await auth.forgotPassword('john@example.com');

// Email'den gelen token ile şifre sıfırla
await auth.resetPassword('john@example.com', 'token_from_email', 'NewPassword123!');

// Uygulama içi şifre değiştir
await auth.changePassword('CurrentPassword123!', 'NewPassword456!');

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
- **Forgot Password**: Saatte 5 istek
- **Change Password**: Saatte 10 istek

## 🔗 Hızlı Linkler

### 📚 Dokümantasyon
- **[Authentication API](./AUTH_API_DOCUMENTATION.md)** - Detaylı auth dokümantasyonu
- **[Frontend Entegrasyon](./FRONTEND_INTEGRATION_GUIDE.md)** - React, Vue, Angular rehberi
- **[Mobil Entegrasyon](./MOBILE_INTEGRATION_GUIDE.md)** - Android, iOS, React Native rehberi

### 🧪 Test ve Geliştirme
- **[Live Swagger UI](https://te4it-api.azurewebsites.net/swagger)** - Production API testi
- **[Local Swagger UI](https://localhost:5001/swagger)** - Development API testi
- **[Postman Collection](./postman/)** - Hazır API collection'ları (yakında)

### 🎯 Platform Spesifik
- **React**: Context, Hook'lar, Email link handling, Şifre sıfırlama
- **Android**: Kotlin, Retrofit, Web'e yönlendirme, Şifre değiştirme
- **iOS**: Swift, URLSession, Keychain, Web'e yönlendirme
- **React Native**: AsyncStorage, Linking, Web'e yönlendirme

## 📞 Destek

API kullanımında sorun yaşarsanız:
1. Swagger UI'da test edin
2. İlgili dokümantasyonu kontrol edin
3. GitHub Issues'da sorun bildirin
