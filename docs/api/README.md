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

---

## 🆕 Yeni Dokümantasyon Yapısı

### Frontend Developer İçin
📁 **`docs/development/`** klasörüne taşındı:

- **FRONTEND_USER_STORIES.md** ⭐
  - User story bazlı akışlar
  - Proje, modül, use case, task yönetimi
  - Email davet sistemi (tam akış)
  - Hazır kod örnekleri (React/TypeScript)
  - UI mockup'ları

- **TASK_MANAGEMENT_FLOWS.md** ⭐
  - Modül, use case, task hiyerarşisi
  - Kanban board implementasyonu
  - Task ilişkileri ve bağımlılıklar
  - Performance optimization

- **COMMON_PATTERNS.md** ⭐
  - Authentication & JWT
  - Error handling
  - Pagination
  - State management
  - Best practices

- **API_QUICK_REFERENCE.md** ⭐
  - Tüm endpoint'lerin özet listesi
  - Request/Response örnekleri
  - Enum değerleri
  - HTTP status codes

### Mobil Developer İçin
📁 **`docs/development/`** klasörüne eklendi:

- **MOBILE_APP_SPECIFICATION.md** 🆕
  - Uygulama konsepti (Dashboard & Monitoring)
  - Read-only yaklaşım
  - Tüm ekranlar ve UI mockup'ları
  - Push notifications
  - Offline support

- **MOBILE_API_INTEGRATION.md** 🆕
  - Retrofit setup
  - Repository pattern
  - Room database (caching)
  - Background sync
  - Error handling
  - Kod örnekleri (Kotlin)

---

## 🚀 Hızlı Başlangıç

### 1. Production API'ye Erişim
- **Live API**: `https://te4it-api.azurewebsites.net`
- **Swagger UI**: `https://te4it-api.azurewebsites.net/swagger`

### 2. Local Development
```bash
cd src/TE4IT.API
dotnet run
```
- **Local API**: `https://localhost:7001`
- **Local Swagger**: `https://localhost:7001/swagger`

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

---

## 📚 Platform Bazlı Yönlendirme

### Frontend Developer misiniz?
👉 **`../development/FRONTEND_USER_STORIES.md`** ile başlayın!
- User story bazlı akışlar
- React/TypeScript örnekleri
- UI mockup'ları
- Tam entegrasyon rehberi

### Mobil Developer misiniz?
👉 **`../development/MOBILE_APP_SPECIFICATION.md`** ile başlayın!
- Dashboard & monitoring odaklı uygulama
- Read-only yaklaşım
- Kotlin örnekleri
- Push notification entegrasyonu

### API Referansı mı arıyorsunuz?
👉 **`../development/API_QUICK_REFERENCE.md`**
- Tüm endpoint'lerin özet listesi
- Hızlı kod örnekleri

---

## 📚 API Genel Bilgileri

### Base URLs
```
Production: https://te4it-api.azurewebsites.net/api/v1
Development: https://localhost:7001/api/v1
```

### Authentication
- **Type**: JWT Bearer Token
- **Header**: `Authorization: Bearer {accessToken}`
- **Refresh**: Otomatik token yenileme

### Response Format
- **Success**: JSON response
- **Error**: RFC 7807 Problem Details format
- **Status Codes**: HTTP standart kodları

---

## 🔗 Hızlı Linkler

### 📚 Dokümantasyon
- **[Authentication API](./AUTH_API_DOCUMENTATION.md)** - Detaylı auth dokümantasyonu
- **[Frontend Entegrasyon](./FRONTEND_INTEGRATION_GUIDE.md)** - React, Vue, Angular rehberi
- **[Mobil Entegrasyon](./MOBILE_INTEGRATION_GUIDE.md)** - Android, iOS, React Native rehberi

### 🧪 Test ve Geliştirme
- **[Live Swagger UI](https://te4it-api.azurewebsites.net/swagger)** - Production API testi
- **[Local Swagger UI](https://localhost:7001/swagger)** - Development API testi

### 🎯 Platform Spesifik
- **React**: Context, Hook'lar, Email link handling, Şifre sıfırlama
- **Android**: Kotlin, Retrofit, Dashboard app, Push notifications
- **iOS**: Swift, URLSession, Keychain, Web'e yönlendirme
- **React Native**: AsyncStorage, Linking, Web'e yönlendirme

---

## 🚀 Deployment

- **[Frontend Azure Deployment](../deployment/FRONTEND_AZURE_DEPLOYMENT.md)** - Frontend geliştiriciler için Azure Static Web Apps deployment rehberi

---

## 📞 Destek

API kullanımında sorun yaşarsanız:
1. Swagger UI'da test edin
2. İlgili dokümantasyonu kontrol edin
3. GitHub Issues'da sorun bildirin

---

**💡 İpucu:** 
- **Frontend developer** → `../development/FRONTEND_USER_STORIES.md`
- **Mobil developer** → `../development/MOBILE_APP_SPECIFICATION.md`