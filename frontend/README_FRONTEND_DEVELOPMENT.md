# Frontend Geliştirme Özeti

## 📦 Oluşturulan Dosyalar

### Types & Services
- `src/types/index.ts` - TypeScript type definitions (Project, Module, UseCase, Task)
- `src/services/projectService.ts` - Project API servisleri
- `src/services/moduleService.ts` - Module API servisleri
- `src/services/useCaseService.ts` - UseCase API servisleri
- `src/services/taskService.ts` - Task API servisleri
- `src/utils/permissions.ts` - Permission helper fonksiyonları

### Pages
- `src/pages/ProjectsListPage.tsx` - Projects listesi (/projects)
- `src/pages/ProjectDetailPage.tsx` - Project detayı (/projects/:projectId)
- `src/pages/ModuleDetailPage.tsx` - Module detayı (/projects/:projectId/modules/:moduleId)

## 🚀 Kurulum ve Çalıştırma

```bash
cd temp-te4it-repo/frontend
npm install
npm run dev
```

## 📋 Route Yapısı

```
/projects                                    → Projects List
/projects/:projectId                         → Project Detail
/projects/:projectId/modules/:moduleId       → Module Detail
/projects/:projectId/modules/:moduleId/usecases/:useCaseId  → UseCase Detail (yapılacak)
```

## ⚙️ API Entegrasyonu

Backend API'leri hazır olduğunda çalışacak şekilde tasarlandı:
- Tüm API çağrıları `services/` klasöründe merkezi olarak yönetiliyor
- Error handling ve loading states mevcut
- Permission kontrolü JWT token'dan yapılıyor

## 🔐 Yetkilendirme

Permission kontrolü `utils/permissions.ts` dosyasında:
```typescript
import { hasPermission, PERMISSIONS } from '../utils/permissions';

// Kullanım örneği
{hasPermission(PERMISSIONS.PROJECT_CREATE) && (
  <Button>Create Project</Button>
)}
```

## 🎨 UI Components

Tüm sayfalarda shadcn/ui component'leri kullanılıyor:
- Button, Input, Select, Table
- Dialog, AlertDialog
- Badge, Breadcrumb, Pagination
- Form validation (react-hook-form)

## 📝 Kalan Görevler

- [ ] UseCase Detail Page (List View)
- [ ] Task Detail Page
- [ ] Kanban View (UseCase Detail)
- [ ] Dashboard/Overview Page

