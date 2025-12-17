# API Quick Reference

> **Güncelleme:** 2024-12-08  
> **Base URL:** `/api/v1`

Tüm endpoint'lerin hızlı referansı. Detaylı bilgi için ilgili user story dokümantasyonlarına bakın.

## 🔐 Authentication

Tüm endpoint'ler (Public olanlar hariç) JWT Bearer Token gerektirir:
```
Authorization: Bearer YOUR_TOKEN
```

---

## 📁 Projects

| Method | Endpoint | Açıklama | Auth Policy |
|--------|----------|----------|-------------|
| GET | `/projects` | Projeleri listele | Authenticated |
| GET | `/projects/{id}` | Proje detayı | ProjectRead |
| POST | `/projects` | Yeni proje oluştur | ProjectCreate |
| PUT | `/projects/{id}` | Proje güncelle | ProjectUpdate |
| PATCH | `/projects/{id}/status` | Durumu değiştir | ProjectUpdate |
| DELETE | `/projects/{id}` | Proje sil | ProjectDelete |

### Proje Üyeleri

| Method | Endpoint | Açıklama | Auth Policy |
|--------|----------|----------|-------------|
| GET | `/projects/{projectId}/members` | Üyeleri listele | ProjectRead |
| POST | `/projects/{projectId}/members` | Üye ekle (direkt) | ProjectUpdate |
| PUT | `/projects/{projectId}/members/{userId}/role` | Rol güncelle | ProjectUpdate |
| DELETE | `/projects/{projectId}/members/{userId}` | Üye çıkar | ProjectUpdate |

### Email Davetleri

| Method | Endpoint | Açıklama | Auth Policy |
|--------|----------|----------|-------------|
| POST | `/projects/{projectId}/invitations` | Davet gönder | ProjectUpdate |
| GET | `/projects/invitations/{token}` | Davet detayı | **Public** |
| POST | `/projects/invitations/{token}/accept` | Daveti kabul et | Authenticated |
| DELETE | `/projects/{projectId}/invitations/{invitationId}` | Daveti iptal et | ProjectUpdate |
| GET | `/projects/{projectId}/invitations` | Davetleri listele | ProjectUpdate |

---

## 📦 Modules

| Method | Endpoint | Açıklama | Auth Policy |
|--------|----------|----------|-------------|
| GET | `/modules/projects/{projectId}` | Modülleri listele | ModuleRead |
| GET | `/modules/{id}` | Modül detayı | ModuleRead |
| POST | `/modules/projects/{projectId}` | Yeni modül oluştur | ModuleCreate |
| PUT | `/modules/{id}` | Modül güncelle | ModuleUpdate |
| PATCH | `/modules/{id}/status` | Durumu değiştir | ModuleUpdate |
| DELETE | `/modules/{id}` | Modül sil | ModuleDelete |

---

## 📋 Use Cases

| Method | Endpoint | Açıklama | Auth Policy |
|--------|----------|----------|-------------|
| GET | `/usecases/modules/{moduleId}` | Use case'leri listele | UseCaseRead |
| GET | `/usecases/{id}` | Use case detayı | UseCaseRead |
| POST | `/usecases/modules/{moduleId}` | Yeni use case oluştur | UseCaseCreate |
| PUT | `/usecases/{id}` | Use case güncelle | UseCaseUpdate |
| PATCH | `/usecases/{id}/status` | Durumu değiştir | UseCaseUpdate |
| DELETE | `/usecases/{id}` | Use case sil | UseCaseDelete |

---

## ✅ Tasks

| Method | Endpoint | Açıklama | Auth Policy |
|--------|----------|----------|-------------|
| GET | `/tasks/usecases/{useCaseId}` | Task'ları listele | TaskRead |
| GET | `/tasks/{id}` | Task detayı | TaskRead |
| POST | `/tasks/usecases/{useCaseId}` | Yeni task oluştur | TaskCreate |
| PUT | `/tasks/{id}` | Task güncelle | TaskUpdate |
| PATCH | `/tasks/{id}/state` | Durumu değiştir | TaskStateChange |
| POST | `/tasks/{id}/assign` | Task ata | TaskAssign |
| DELETE | `/tasks/{id}` | Task sil | TaskDelete |

### Task İlişkileri

| Method | Endpoint | Açıklama | Auth Policy |
|--------|----------|----------|-------------|
| GET | `/tasks/{taskId}/relations` | İlişkileri listele | TaskRelationRead |
| POST | `/tasks/{taskId}/relations` | İlişki oluştur | TaskRelationCreate |
| DELETE | `/tasks/{taskId}/relations/{relationId}` | İlişki sil | TaskRelationDelete |

---

## 📝 Query Parameters

### Pagination (Tüm liste endpoint'lerinde)

```
?page=1&pageSize=20
```

### Projects Filtreler

```
?search=eticaret
&isActive=true
```

### Modules Filtreler

```
?search=kullanici
&isActive=true
```

### Use Cases Filtreler

```
?search=kayit
&isActive=true
```

### Tasks Filtreler

```
?search=form
&state=2           # TaskState (1: NotStarted, 2: InProgress, 3: Completed, 4: Cancelled)
&type=2            # TaskType (1: Documentation, 2: Feature, 3: Test, 4: Bug)
&assigneeId=guid
&dueDateFrom=2024-01-01
&dueDateTo=2024-01-31
```

---

## 🔢 Enum Değerleri

### ProjectRole

```typescript
enum ProjectRole {
  Viewer = 1,   // Sadece görüntüleme
  Member = 2,   // Düzenleme yetkisi
  Owner = 5     // Tam yetki
}
```

### TaskState

```typescript
enum TaskState {
  NotStarted = 1,  // Başlanmadı
  InProgress = 2,  // Devam ediyor
  Completed = 3,   // Tamamlandı
  Cancelled = 4    // İptal edildi
}
```

### TaskType

```typescript
enum TaskType {
  Documentation = 1,  // Dokümantasyon
  Feature = 2,        // Özellik geliştirme
  Test = 3,           // Test
  Bug = 4             // Hata düzeltme
}
```

### TaskRelationType

```typescript
enum TaskRelationType {
  Blocks = 1,      // Bu task diğerini bloklar
  RelatesTo = 2,   // İlişkili
  Fixes = 3,       // Bu task diğer bug'ı düzeltir
  Duplicates = 4   // Tekrar
}
```

---

## 🔴 HTTP Status Codes

| Code | Anlamı | Ne Zaman? |
|------|--------|-----------|
| 200 | OK | Başarılı GET/PATCH işlemi |
| 201 | Created | Başarılı POST işlemi |
| 204 | No Content | Başarılı PUT/DELETE işlemi |
| 400 | Bad Request | Validation hatası |
| 401 | Unauthorized | Token yok veya geçersiz |
| 403 | Forbidden | Yetki yok |
| 404 | Not Found | Kaynak bulunamadı |
| 409 | Conflict | İşlem çakışması (örn: duplicate) |
| 500 | Server Error | Sunucu hatası |

---

## 📤 Request Body Örnekleri

### Create Project

```json
POST /projects
{
  "title": "E-Ticaret Platformu",
  "description": "Online alışveriş sistemi"
}
```

### Invite Member

```json
POST /projects/{projectId}/invitations
{
  "email": "user@example.com",
  "role": 2
}
```

### Create Task

```json
POST /tasks/usecases/{useCaseId}
{
  "title": "Kayıt formu tasarımı",
  "taskType": 1,
  "description": "UI/UX tasarımı",
  "importantNotes": "Responsive olmalı",
  "dueDate": "2024-01-25T23:59:59Z",
  "assigneeId": "guid" // NULLABLE - opsiyonel
}
```

### Change Task State

```json
PATCH /tasks/{id}/state
{
  "newState": 2
}
```

### Create Task Relation

```json
POST /tasks/{taskId}/relations
{
  "targetTaskId": "guid",
  "relationType": 1
}
```

---

## 📥 Response Örnekleri

### Paginated Response

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 42,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Validation Error (400)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["Title is required."],
    "Email": ["Email format is invalid."]
  }
}
```

---

## 🚀 Quick Start Kod Örnekleri

### Axios Setup

```typescript
import axios from 'axios';

const api = axios.create({
  baseURL: '/api/v1',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Token interceptor
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

### Fetch Projects

```typescript
const getProjects = async (page = 1, search = '') => {
  const response = await api.get('/projects', {
    params: { page, pageSize: 20, search }
  });
  return response.data;
};
```

### Create Project

```typescript
const createProject = async (data) => {
  const response = await api.post('/projects', data);
  return response.data;
};
```

### Change Task State

```typescript
const changeTaskState = async (taskId, newState) => {
  await api.patch(`/tasks/${taskId}/state`, { newState });
};
```

---

## 📚 Detaylı Dokümantasyon

- **User Stories & Akışlar:** `FRONTEND_USER_STORIES.md`
- **Task Management:** `TASK_MANAGEMENT_FLOWS.md`
- **Common Patterns:** `COMMON_PATTERNS.md`

---

**Not:** Bu dokümantasyon tüm endpoint'lerin özet listesidir. Detaylı açıklamalar, business rules ve kod örnekleri için yukarıdaki dokümantasyonlara bakın.
