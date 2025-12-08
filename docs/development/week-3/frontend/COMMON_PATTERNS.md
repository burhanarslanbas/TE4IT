# Common Patterns - Frontend Development Guide

> **Hedef Kitle:** Frontend Developers  
> **Güncelleme:** 2024-12-08  
> **Odak:** Yaygın kullanılan pattern'ler ve best practices

Bu dokümantasyon, TE4IT platformunda frontend geliştirme yaparken tekrar eden pattern'leri ve best practice'leri içerir.

## İçindekiler

1. [Authentication & Authorization](#1-authentication--authorization)
2. [Error Handling](#2-error-handling)
3. [API Communication](#3-api-communication)
4. [Pagination](#4-pagination)
5. [Loading & Skeleton States](#5-loading--skeleton-states)
6. [Optimistic Updates](#6-optimistic-updates)

---

## 1. Authentication & Authorization

### JWT Token Management

```typescript
// lib/auth.ts
interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

class AuthService {
  private readonly ACCESS_TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  
  setTokens(tokens: AuthTokens): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, tokens.refreshToken);
  }
  
  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }
  
  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }
}

export const authService = new AuthService();
```

### Axios Interceptor

```typescript
// lib/axios.ts
import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: '/api/v1',
  timeout: 30000
});

// Request interceptor - Token ekle
axiosInstance.interceptors.request.use((config) => {
  const token = authService.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor - 401 handle
axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      authService.clearTokens();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default axiosInstance;
```

---

## 2. Error Handling

### Centralized Error Handler

```typescript
// lib/errorHandler.ts
import { toast } from 'react-hot-toast';

export class ErrorHandler {
  static handle(error: unknown): void {
    if (axios.isAxiosError(error)) {
      const status = error.response?.status;
      const data = error.response?.data;
      
      switch (status) {
        case 400:
          // Validation errors
          if (data.errors) {
            const firstError = Object.values(data.errors)[0][0];
            toast.error(firstError);
          }
          break;
        case 401:
          toast.error('Oturumunuz sonlanmış');
          break;
        case 403:
          toast.error('Yetkiniz bulunmuyor');
          break;
        case 404:
          toast.error('Kaynak bulunamadı');
          break;
        default:
          toast.error('Bir hata oluştu');
      }
    }
  }
  
  // Field errors
  static extractFieldErrors(error: any): Record<string, string> {
    const fieldErrors: Record<string, string> = {};
    if (error.response?.data.errors) {
      Object.entries(error.response.data.errors).forEach(([key, messages]: [string, any]) => {
        fieldErrors[key.toLowerCase()] = messages[0];
      });
    }
    return fieldErrors;
  }
}
```

---

## 3. API Communication

### Type-safe Service

```typescript
// services/projectService.ts
export class ProjectService {
  async getAll(params?: {
    page?: number;
    pageSize?: number;
    search?: string;
  }): Promise<PagedResult<Project>> {
    const response = await axios.get('/projects', { params });
    return response.data;
  }
  
  async getById(id: string): Promise<Project> {
    const response = await axios.get(`/projects/${id}`);
    return response.data;
  }
  
  async create(data: CreateProjectRequest): Promise<Project> {
    const response = await axios.post('/projects', data);
    return response.data;
  }
}

export const projectService = new ProjectService();
```

### React Query Integration

```typescript
// hooks/useProjects.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

export const useProjects = (filters?: ProjectFilters) => {
  return useQuery({
    queryKey: ['projects', filters],
    queryFn: () => projectService.getAll(filters)
  });
};

export const useCreateProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: projectService.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      toast.success('Proje oluşturuldu!');
    },
    onError: ErrorHandler.handle
  });
};
```

---

## 4. Pagination

### Reusable Component

```tsx
// components/Pagination.tsx
interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  onPageChange
}) => {
  return (
    <div className="pagination">
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
      >
        ◄ Önceki
      </button>
      
      <span>Sayfa {currentPage} / {totalPages}</span>
      
      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages}
      >
        Sonraki ►
      </button>
    </div>
  );
};
```

---

## 5. Loading & Skeleton States

### Skeleton Loader

```tsx
// components/SkeletonLoader.tsx
export const ProjectCardSkeleton = () => {
  return (
    <div className="skeleton-card">
      <div className="skeleton skeleton-title" />
      <div className="skeleton skeleton-text" />
      <div className="skeleton skeleton-text" />
    </div>
  );
};

// CSS
.skeleton {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: loading 1.5s ease-in-out infinite;
}

@keyframes loading {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
```

---

## 6. Optimistic Updates

```typescript
const useUpdateTask = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }) => taskService.update(id, data),
    
    onMutate: async ({ id, data }) => {
      // Cancel queries
      await queryClient.cancelQueries({ queryKey: ['task', id] });
      
      // Get previous
      const previous = queryClient.getQueryData(['task', id]);
      
      // Optimistic update
      queryClient.setQueryData(['task', id], (old) => ({ ...old, ...data }));
      
      return { previous };
    },
    
    onError: (err, variables, context) => {
      // Rollback
      if (context?.previous) {
        queryClient.setQueryData(['task', variables.id], context.previous);
      }
      ErrorHandler.handle(err);
    }
  });
};
```

---

## Best Practices Özet

### ✅ DO (Yapın)
- Token'ları güvenli saklayın (httpOnly cookies ideal)
- Hataları merkezi olarak handle edin
- Loading state'leri gösterin
- Optimistic updates kullanın
- React Query ile cache yönetin
- Type-safe API servisleri yazın

### ❌ DON'T (Yapmayın)
- Token'ı localStorage'da saklamayın (XSS riski)
- Her component'te ayrı error handling
- Loading state olmadan API çağrısı
- Cache invalidation'ı unutmayın
- Any type kullanmayın

---

Bu pattern'ler TE4IT platformunda tutarlı ve kaliteli frontend geliştirme sağlar.
