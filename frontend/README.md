# TE4IT Frontend Application

Modern, responsive ve kullanıcı dostu bir React frontend uygulaması. TE4IT projesinin web arayüzünü oluşturan bu uygulama, backend API'si ile tam entegre çalışır.

## 🚀 Özellikler

### ✅ Tamamlanan Özellikler

- **🎨 Modern UI/UX**: Dark theme ile modern tasarım
- **🔐 Authentication**: Login/Register sistemi
- **👤 User Management**: Profil yönetimi
- **📱 Responsive Design**: Mobil uyumlu tasarım
- **🎭 Animations**: Smooth geçişler ve animasyonlar
- **🔔 Notifications**: Toast bildirimleri
- **🌐 API Integration**: Backend ile tam entegrasyon

### 🔄 Geliştirilmekte Olan Özellikler

- **📊 Dashboard**: Kullanıcı dashboard'u
- **📋 Project Management**: Proje yönetimi arayüzü
- **📚 Education Module**: Eğitim modülü
- **🤖 AI Features**: AI destekli özellikler

## 🛠️ Teknoloji Stack

### Frontend Framework
- **React 18.3.1** - Modern React hooks ve functional components
- **TypeScript** - Type-safe development
- **Vite** - Hızlı build tool ve dev server

### UI/UX Libraries
- **Tailwind CSS** - Utility-first CSS framework
- **Radix UI** - Accessible component primitives
- **Lucide React** - Modern icon library
- **Motion** - Animation library
- **Sonner** - Toast notifications

### State Management & Forms
- **Zustand** - Lightweight state management
- **React Hook Form** - Form handling
- **Next Themes** - Theme management

### Development Tools
- **ESLint** - Code linting
- **Prettier** - Code formatting
- **TypeScript** - Static type checking

## 📁 Proje Yapısı

```
frontend/
├── 📁 src/
│   ├── 📁 components/           # React bileşenleri
│   │   ├── 📁 ui/              # Reusable UI bileşenleri
│   │   ├── 📁 profile-sections/ # Profil sayfası bileşenleri
│   │   ├── 📁 figma/           # Figma tasarımlarından gelen bileşenler
│   │   ├── auth-background.tsx  # Auth sayfaları arka planı
│   │   ├── hero-section.tsx     # Ana sayfa hero bölümü
│   │   ├── features-section.tsx # Özellikler bölümü
│   │   ├── login-page.tsx       # Giriş sayfası
│   │   ├── register-page.tsx    # Kayıt sayfası
│   │   ├── profile-page.tsx     # Profil sayfası
│   │   └── ...                  # Diğer bileşenler
│   ├── 📁 services/             # API servisleri
│   │   ├── api.ts              # Temel API client
│   │   └── auth.ts             # Authentication servisleri
│   ├── 📁 styles/               # CSS dosyaları
│   │   └── globals.css          # Global stiller
│   ├── 📁 guidelines/           # Geliştirme rehberleri
│   │   └── Guidelines.md        # Kod standartları
│   ├── App.tsx                  # Ana uygulama bileşeni
│   ├── main.tsx                 # Uygulama giriş noktası
│   └── index.css                # Ana CSS dosyası
├── 📄 package.json              # Proje bağımlılıkları
├── 📄 vite.config.ts            # Vite konfigürasyonu
├── 📄 index.html                # HTML template
├── 📄 API_INTEGRATION.md        # API entegrasyon dokümantasyonu
└── 📄 README.md                 # Bu dosya
```

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler
- **Node.js** >= 18.0.0
- **npm** >= 8.0.0

### Kurulum Adımları

1. **Bağımlılıkları yükleyin:**
   ```bash
   npm install
   ```

2. **Geliştirme sunucusunu başlatın:**
   ```bash
   npm run dev
   ```

3. **Tarayıcıda açın:**
   ```
   http://localhost:3000
   ```

### Build İşlemi

```bash
# Production build
npm run build

# Build dosyalarını preview etme
npm run preview
```

## 🔗 API Entegrasyonu

### Backend API Bilgileri
- **Base URL**: `https://te4it-api.azurewebsites.net`
- **Swagger**: [API Dokümantasyonu](https://te4it-api.azurewebsites.net/swagger/index.html)

### Authentication Endpoints
| Method | Endpoint | Açıklama |
|--------|----------|----------|
| POST | `/api/v1/auth/login` | Kullanıcı girişi |
| POST | `/api/v1/auth/register` | Kullanıcı kaydı |
| POST | `/api/v1/auth/logout` | Kullanıcı çıkışı |
| GET | `/api/v1/auth/me` | Mevcut kullanıcı bilgileri |

### API Client Kullanımı
```typescript
import { AuthService } from './services/auth';

// Login işlemi
const response = await AuthService.login({
  email: "user@example.com",
  password: "password123"
});

// Register işlemi
const response = await AuthService.register({
  userName: "username",
  email: "user@example.com",
  password: "password123"
});
```

## 🎨 UI/UX Özellikleri

### Tema Sistemi
- **Dark Theme**: Varsayılan tema
- **Responsive Design**: Mobil-first yaklaşım
- **Accessibility**: WCAG 2.1 uyumlu

### Bileşen Kütüphanesi
- **Radix UI**: Accessible primitives
- **Custom Components**: Projeye özel bileşenler
- **Icon System**: Lucide React icons
- **Animation**: Motion library ile smooth geçişler

### Form Handling
- **Real-time Validation**: Anlık doğrulama
- **Error Handling**: Kullanıcı dostu hata mesajları
- **Loading States**: İşlem durumu göstergeleri
- **Success Feedback**: Başarı bildirimleri

## 🔐 Authentication Flow

### Login İşlemi
1. Kullanıcı email/password girer
2. Form validation çalışır
3. API'ye login isteği gönderilir
4. Token localStorage'a kaydedilir
5. Kullanıcı profil sayfasına yönlendirilir

### Register İşlemi
1. Kullanıcı bilgilerini girer
2. Real-time validation
3. API'ye register isteği gönderilir
4. Başarılı kayıt sonrası login sayfasına yönlendirme

### Token Yönetimi
- **JWT Tokens**: Secure authentication
- **Auto-refresh**: Token yenileme
- **Logout**: Token temizleme
- **Persistent Storage**: localStorage kullanımı

## 🧪 Testing

### Test Komutları
```bash
# Unit tests (gelecekte eklenecek)
npm test

# E2E tests (gelecekte eklenecek)
npm run test:e2e

# Coverage report (gelecekte eklenecek)
npm run test:coverage
```

### Test Stratejisi
- **Unit Tests**: Bileşen testleri
- **Integration Tests**: API entegrasyon testleri
- **E2E Tests**: Kullanıcı senaryoları
- **Visual Tests**: UI regression testleri

## 🚀 Deployment

### Production Build
```bash
npm run build
```

### Deployment Seçenekleri
- **Vercel**: Kolay deployment
- **Netlify**: Static site hosting
- **Azure Static Web Apps**: Microsoft cloud
- **GitHub Pages**: Ücretsiz hosting

### Environment Variables
```bash
# .env.local
VITE_API_BASE_URL=https://te4it-api.azurewebsites.net
VITE_APP_NAME=TE4IT Frontend
```

## 🔧 Development

### Kod Standartları
- **ESLint**: Code linting
- **Prettier**: Code formatting
- **TypeScript**: Type safety
- **Conventional Commits**: Commit mesajları

### Git Workflow
1. Feature branch oluştur
2. Değişiklikleri yap
3. Testleri çalıştır
4. Pull request oluştur
5. Code review
6. Merge to develop

### Debugging
- **React DevTools**: Component debugging
- **Redux DevTools**: State debugging
- **Network Tab**: API istekleri
- **Console Logs**: Debug mesajları

## 📚 Dokümantasyon

### API Dokümantasyonu
- **Swagger UI**: [API Docs](https://te4it-api.azurewebsites.net/swagger/index.html)
- **API Integration**: `API_INTEGRATION.md`
- **Component Docs**: Her bileşen için JSDoc

### Geliştirme Rehberleri
- **Guidelines**: `src/guidelines/Guidelines.md`
- **Architecture**: Clean Architecture principles
- **Best Practices**: React best practices

## 🤝 Contributing

### Katkıda Bulunma
1. Repository'yi fork edin
2. Feature branch oluşturun
3. Değişikliklerinizi yapın
4. Testleri çalıştırın
5. Pull request oluşturun

### Code Review Süreci
- **Automated Checks**: CI/CD pipeline
- **Manual Review**: Peer review
- **Testing**: Comprehensive test coverage
- **Documentation**: Updated docs

## 🐛 Known Issues

### Mevcut Sorunlar
- [ ] Mobile responsive iyileştirmeleri
- [ ] Performance optimizasyonları
- [ ] Accessibility iyileştirmeleri

### Gelecek Geliştirmeler
- [ ] PWA desteği
- [ ] Offline functionality
- [ ] Advanced animations
- [ ] Internationalization (i18n)

## 📞 Support

### Yardım Alma
- **GitHub Issues**: Bug reports ve feature requests
- **Documentation**: Comprehensive docs
- **Community**: Developer discussions

### İletişim
- **Email**: team@te4it.com
- **GitHub**: [burhanarslanbas/TE4IT](https://github.com/burhanarslanbas/TE4IT)
- **Issues**: [GitHub Issues](https://github.com/burhanarslanbas/TE4IT/issues)

## 📄 License

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

---

**Built with ❤️ by the TE4IT Development Team**

_Empowering software development teams with intelligent project management and integrated learning solutions._