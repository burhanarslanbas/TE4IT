# 🏗️ Frontend Klasör Yapısı Refactoring Planı

## 📊 Mevcut Durum vs Hedef Yapı

### Mevcut Yapı (İyi ✅)
```
src/
├── components/     (50+ dosya - Karışık)
├── services/      (✅ İyi)
├── ui/            (✅ İyi)
└── styles/        (✅ İyi)
```

### Hedef Yapı (Profesyonel 🎯)
```
src/
├── app/              → App.tsx, main.tsx
├── components/       → Reusable components
│   ├── ui/          → UI component library
│   ├── layout/       → Layout components (Navigation, Footer)
│   ├── auth/         → Auth-related components
│   └── profile/      → Profile-related components
├── pages/            → Page components (Login, Register, Profile)
├── features/         → Feature-specific components
│   ├── landing/      → Landing page sections
│   └── dashboard/    → Dashboard features
├── hooks/            → Custom React hooks
├── services/         → API services
├── stores/           → Zustand state management
├── types/            → TypeScript type definitions
├── utils/            → Helper functions
├── constants/         → App constants
├── assets/           → Images, fonts
└── styles/           → Global styles
```

## 🎯 Refactoring Adımları

### 1. Yeni Klasörler Oluştur
```bash
mkdir src/pages
mkdir src/features
mkdir src/hooks
mkdir src/types
mkdir src/utils
mkdir src/constants
mkdir src/assets
mkdir src/stores
```

### 2. Component'leri Yeniden Organize Et

#### `components/` → Sadece Reusable Components
- Taşı → `components/ui/`: Tüm UI components (zaten orada)
- Taşı → `components/layout/`: Navigation, Footer
- Taşı → `components/auth/`: Auth-specific components
- Taşı → `components/profile/`: Profile-specific components

#### `pages/` → Tüm Sayfalar
- `login-page.tsx` → `pages/LoginPage.tsx`
- `register-page.tsx` → `pages/RegisterPage.tsx`
- `forgot-password-page.tsx` → `pages/ForgotPasswordPage.tsx`
- `profile-page.tsx` → `pages/ProfilePage.tsx`

#### `features/` → Feature-Specific Components
- `hero-section.tsx` → `features/landing/HeroSection.tsx`
- `features-section.tsx` → `features/landing/FeaturesSection.tsx`
- `detailed-features-section.tsx` → `features/landing/DetailedFeaturesSection.tsx`
- `trust-building-section.tsx` → `features/landing/TrustBuildingSection.tsx`
- `how-it-works-section.tsx` → `features/landing/HowItWorksSection.tsx`
- `pricing-section.tsx` → `features/landing/PricingSection.tsx`
- `final-cta-section.tsx` → `features/landing/FinalCtaSection.tsx`

#### `profile-sections/` → `components/profile/`
- `profile-info.tsx` → `components/profile/ProfileInfo.tsx`
- `security-settings.tsx` → `components/profile/SecuritySettings.tsx`
- `app-settings.tsx` → `components/profile/AppSettings.tsx`

### 3. Services'i Organize Et
```
services/
├── api/
│   ├── client.ts      → API client
│   ├── endpoints.ts    → API endpoints
│   └── types.ts        → API types
├── auth/
│   ├── authService.ts  → Auth service
│   └── tokenHelper.ts  → Token utilities
```

### 4. Yeni Dosyalar Oluştur

#### `src/types/`
```
types/
├── auth.types.ts       → Auth types
├── user.types.ts       → User types
├── api.types.ts        → API types
└── index.ts            → Type exports
```

#### `src/hooks/`
```
hooks/
├── useAuth.ts          → Auth hook
├── useUser.ts          → User data hook
├── useApi.ts           → API hook
└── index.ts            → Hook exports
```

#### `src/utils/`
```
utils/
├── validation.ts       → Validation helpers
├── formatting.ts       → Format helpers
└── index.ts            → Utility exports
```

#### `src/constants/`
```
constants/
├── api.ts              → API constants
├── routes.ts           → Route constants
└── config.ts           → App config
```

## 📝 Rename Scripts

### Windows PowerShell
```powershell
# Pages rename
Rename-Item "components/login-page.tsx" "pages/LoginPage.tsx"
Rename-Item "components/register-page.tsx" "pages/RegisterPage.tsx"
Rename-Item "components/forgot-password-page.tsx" "pages/ForgotPasswordPage.tsx"
Rename-Item "components/profile-page.tsx" "pages/ProfilePage.tsx"
```

### Linux/Mac
```bash
# Pages rename
mv components/login-page.tsx pages/LoginPage.tsx
mv components/register-page.tsx pages/RegisterPage.tsx
mv components/forgot-password-page.tsx pages/ForgotPasswordPage.tsx
mv components/profile-page.tsx pages/ProfilePage.tsx
```

## 🔄 Import Update Pattern

### Before
```typescript
import { LoginPage } from "./components/login-page";
```

### After
```typescript
import { LoginPage } from "@/pages/LoginPage";
```

## ✅ Refactoring Sonrası Avantajlar

1. **Daha iyi organize** → Her dosyanın yeri belli
2. **Scalability** → Yeni özellikler kolayca eklenebilir
3. **Maintainability** → Kod bulmak daha kolay
4. **Team collaboration** → Takım çalışması için uygun
5. **Best practices** → Industry standard yapı

## 🎯 Uygulama Önceliği

### Yüksek Öncelik ✅
- [ ] Yeni klasörleri oluştur
- [ ] `pages/` klasörüne sayfaları taşı
- [ ] `components/profile/` klasörünü oluştur
- [ ] `features/` klasörüne landing sections'ı taşı
- [ ] Import'ları güncelle

### Orta Öncelik 📊
- [ ] `hooks/` klasörü oluştur
- [ ] `types/` klasörü oluştur
- [ ] `utils/` klasörü oluştur
- [ ] Custom hooks ekle
- [ ] Type definitions ekle

### Düşük Öncelik 📝
- [ ] `stores/` klasörü oluştur (eğer gerekirse)
- [ ] `constants/` klasörü oluştur
- [ ] `assets/` klasörü oluştur
- [ ] Documentation güncelle

## 🚀 Hızlı Başlangıç

1. **Test et** → Önce testleri çalıştır
2. **Kopyala** → Dosyaları yeni klasörlere kopyala
3. **Update** → Import'ları güncelle
4. **Test et** → Tekrar test et
5. **Sil** → Eski dosyaları sil

## 📚 Kaynaklar

- [React Folder Structure Best Practices](https://kentcdodds.com/blog/folder-structure)
- [TypeScript Project References](https://www.typescriptlang.org/docs/handbook/project-references.html)
- [Feature-Sliced Design](https://feature-sliced.design/)


