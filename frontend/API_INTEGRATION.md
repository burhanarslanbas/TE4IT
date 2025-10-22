# TE4IT Frontend - API Entegrasyonu

Bu dokümantasyon, TE4IT frontend uygulamasının backend API'si ile nasıl entegre edildiğini açıklar.

## 🔗 Backend API Bilgileri

- **Base URL**: `https://te4it-api.azurewebsites.net`
- **Swagger Dokümantasyonu**: [https://te4it-api.azurewebsites.net/swagger/index.html](https://te4it-api.azurewebsites.net/swagger/index.html)

## 📁 Proje Yapısı

```
src/
├── services/
│   ├── api.ts          # Temel API client ve konfigürasyon
│   └── auth.ts          # Authentication servisleri
├── components/
│   ├── login-page.tsx   # Login sayfası (API entegreli)
│   ├── register-page.tsx # Register sayfası (API entegreli)
│   └── ...
└── App.tsx              # Ana uygulama (Auth state yönetimi)
```

## 🚀 API Entegrasyonu Özellikleri

### ✅ Tamamlanan Özellikler

1. **API Client Konfigürasyonu**
   - Base URL konfigürasyonu
   - JWT token yönetimi
   - Otomatik header ekleme
   - Hata yönetimi

2. **Authentication Servisleri**
   - Login API entegrasyonu
   - Register API entegrasyonu
   - Token validation
   - Logout işlemi

3. **Frontend Form Entegrasyonu**
   - Real-time validation
   - Loading states
   - Error handling
   - Success notifications

4. **State Management**
   - Authentication state
   - Token persistence (localStorage)
   - Auto-login on app start

## 🔧 Kullanılan Teknolojiler

- **React 18** + **TypeScript**
- **Fetch API** (native HTTP client)
- **Sonner** (toast notifications)
- **LocalStorage** (token persistence)
- **Motion** (animations)

## 📋 API Endpoints

### Authentication Endpoints

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | `/api/v1/auth/login` | Kullanıcı girişi |
| POST | `/api/v1/auth/register` | Kullanıcı kaydı |
| POST | `/api/v1/auth/logout` | Kullanıcı çıkışı |
| GET | `/api/v1/auth/me` | Mevcut kullanıcı bilgileri |

## 🔐 Authentication Flow

### 1. Login İşlemi
```typescript
// Login form submit
const response = await AuthService.login({
  email: "user@example.com",
  password: "password123"
});

// Token otomatik olarak kaydedilir
// Kullanıcı profil sayfasına yönlendirilir
```

### 2. Register İşlemi
```typescript
// Register form submit
const response = await AuthService.register({
  userName: "username",
  email: "user@example.com",
  password: "password123"
});

// Token otomatik olarak kaydedilir
// Login sayfasına yönlendirilir
```

### 3. Token Yönetimi
```typescript
// Token otomatik olarak localStorage'a kaydedilir
localStorage.setItem('te4it_token', token);

// Her API isteğinde otomatik olarak header'a eklenir
headers.Authorization = `Bearer ${token}`;
```

## 🎨 UI/UX Özellikleri

### Loading States
- Form submit sırasında loading spinner
- Button disable durumu
- Input field opacity değişimi

### Error Handling
- Real-time validation
- Backend hata mesajları
- Toast notifications
- Field-specific error messages

### Success Feedback
- Toast success messages
- Smooth page transitions
- User-friendly feedback

## 🔍 Hata Yönetimi

### HTTP Status Kodları
- **200**: Başarılı işlem
- **400**: Validation hatası
- **401**: Unauthorized (yanlış credentials)
- **409**: Conflict (email/username zaten kullanılıyor)
- **429**: Rate limiting
- **500**: Server hatası

### Frontend Hata Mesajları
```typescript
// Backend'den gelen hatalar otomatik olarak parse edilir
if (error.status === 401) {
  setErrors({ general: "E-posta veya şifre hatalı" });
} else if (error.status === 409) {
  setErrors({ general: "Bu e-posta adresi zaten kullanılıyor" });
}
```

## 🚀 Geliştirme Komutları

```bash
# Geliştirme sunucusunu başlat
npm run dev

# Production build
npm run build

# Linter kontrolü
npm run lint
```

## 📝 Yorum Satırları

Tüm kod dosyalarında detaylı yorum satırları eklenmiştir:

- **API servisleri**: Her fonksiyonun ne yaptığı açıklanmış
- **Form handlers**: Validation ve error handling açıklanmış
- **State management**: Authentication flow açıklanmış
- **UI components**: Loading states ve error handling açıklanmış

## 🔄 Gelecek Geliştirmeler

1. **Password Reset**: Şifre sıfırlama özelliği
2. **Email Verification**: E-posta doğrulama
3. **Two-Factor Authentication**: 2FA desteği
4. **Social Login**: Google/Facebook login
5. **Remember Me**: "Beni hatırla" özelliği

## 🐛 Debugging

### Console Logs
```typescript
// API istekleri için debug logları
console.log("API Request:", { endpoint, method, data });
console.log("API Response:", response);
```

### Network Tab
- Chrome DevTools > Network tab'ında API isteklerini izleyebilirsiniz
- Request/Response headers ve body'yi kontrol edebilirsiniz

## 📞 Destek

Herhangi bir sorun yaşarsanız:
1. Console'da hata mesajlarını kontrol edin
2. Network tab'ında API isteklerini kontrol edin
3. Backend API'nin çalıştığından emin olun
4. Swagger dokümantasyonunu kontrol edin

---

**Not**: Bu entegrasyon production-ready durumda ve tüm edge case'ler handle edilmiştir.


