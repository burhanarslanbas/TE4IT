# Frontend Development

Bu klasör React frontend uygulaması için ayrılmıştır.

## 📋 Görevler

- [ ] React projesi oluştur
- [ ] TypeScript konfigürasyonu
- [ ] UI/UX tasarımı
- [ ] Backend API entegrasyonu
- [ ] Authentication sistemi
- [ ] State management
- [ ] Testing setup
- [ ] Build & deployment

## 🚀 Başlangıç

### 1. Proje Oluşturma
```bash
# React + TypeScript projesi oluştur
npx create-react-app . --template typescript

# veya Vite ile (önerilen)
npm create vite@latest . -- --template react-ts
```

### 2. Bağımlılıklar
```bash
npm install
```

### 3. Geliştirme Sunucusu
```bash
npm start
# veya Vite kullanıyorsan
npm run dev
```

## 🔗 Backend API Entegrasyonu

Backend API şu adreste çalışıyor:
- **URL**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

### Authentication
```typescript
// API base URL
const API_BASE_URL = 'https://localhost:5001';

// Login örneği
const login = async (email: string, password: string) => {
  const response = await fetch(`${API_BASE_URL}/api/v1/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });
  
  const data = await response.json();
  return data;
};
```

### API Endpoints
- `POST /api/v1/auth/register` - Kullanıcı kaydı
- `POST /api/v1/auth/login` - Giriş
- `GET /api/v1/users` - Kullanıcı listesi
- `GET /api/v1/projects` - Proje listesi
- `GET /api/v1/roles` - Rol listesi

## 📚 Dokümantasyon

Detaylı API dokümantasyonu için:
- [Ana README](../README.md)
- [Geliştirme Rehberi](../docs/DEVELOPMENT.md)
- [AI Entegrasyonu](../docs/AI_INTEGRATION.md)

## 🛠️ Önerilen Teknolojiler

- **Framework**: React 18+
- **Language**: TypeScript
- **State Management**: Zustand, Redux Toolkit, veya Context API
- **Data Fetching**: TanStack Query, SWR, veya Axios
- **UI Library**: Material-UI, Ant Design, Chakra UI, veya Tailwind CSS
- **Forms**: React Hook Form + Zod
- **Routing**: React Router v6
- **Testing**: Jest + React Testing Library

## 🎨 UI/UX Önerileri

- Modern, temiz tasarım
- Responsive (mobil uyumlu)
- Dark/Light mode desteği
- Accessibility (WCAG) standartları
- Loading states ve error handling

## 📱 Responsive Design

- Mobile-first yaklaşım
- Tablet ve desktop uyumluluğu
- Touch-friendly interface

İyi kodlamalar! 🚀
