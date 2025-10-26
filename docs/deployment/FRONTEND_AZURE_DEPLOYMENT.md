# Frontend Azure Static Web Apps Deployment Guide

**Sürüm:** 1.0  
**Tarih:** Ocak 2025  
**Platform:** React + Vite  
**Deployment:** Azure Static Web Apps  

---

## 🎯 Amaç

Bu dokümantasyon, frontend uygulamasının Azure Static Web Apps'e deploy edilmesi için yapılması gereken tüm değişiklikleri içerir.

**Not:** Bu dokümantasyon Cursor AI tarafından doğrudan kullanılabilir.

---

## 📋 İçindekiler

1. [Environment Variables](#1-environment-variables)
2. [Vite Configuration](#2-vite-configuration)
3. [API Configuration](#3-api-configuration)
4. [Build Configuration](#4-build-configuration)
5. [GitHub Actions](#5-github-actions)
6. [Deployment Checklist](#6-deployment-checklist)
7. [Testing](#7-testing)

---

## 1. Environment Variables

### 1.1 Production Environment

**Dosya:** `frontend/.env.production`

```env
# Production API Base URL
VITE_API_BASE_URL=https://te4it-api.azurewebsites.net

# Environment Type
VITE_APP_ENV=production

# Frontend URL
VITE_FRONTEND_URL=https://te4it-frontend.azurestaticapps.net

# App Name
VITE_APP_NAME=TE4IT
```

### 1.2 Development Environment

**Dosya:** `frontend/.env.development`

```env
# Development API Base URL
VITE_API_BASE_URL=http://localhost:5001

# Environment Type
VITE_APP_ENV=development

# Frontend URL
VITE_FRONTEND_URL=http://localhost:3000

# App Name
VITE_APP_NAME=TE4IT (Dev)
```

### 1.3 Git Ignore

**Dosya:** `frontend/.gitignore` (zaten var olmalı)

```gitignore
# Environment files (güvenlik için production hariç)
.env
.env.local
.env.development.local
.env.test.local
.env.production.local

# Build output
dist
build
```

**Not:** `.env.production` dosyası commit edilmelidir çünkü içinde sadece public URL'ler var.

---

## 2. Vite Configuration

### 2.1 Vite Config

**Dosya:** `frontend/vite.config.ts`

```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  
  // Build configuration
  build: {
    outDir: 'dist',
    emptyOutDir: true,
    sourcemap: false, // Production'da sourcemap kapalı
    rollupOptions: {
      output: {
        // Chunk için dosya isimlendirme
        entryFileNames: 'assets/[name].[hash].js',
        chunkFileNames: 'assets/[name].[hash].js',
        assetFileNames: 'assets/[name].[hash].[ext]'
      }
    }
  },
  
  // Development server
  server: {
    port: 3000,
    host: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5001',
        changeOrigin: true,
        secure: false
      }
    }
  },
  
  // Path resolution
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  
  // Environment variables
  define: {
    __APP_VERSION__: JSON.stringify(process.env.npm_package_version)
  }
});
```

---

## 3. API Configuration

### 3.1 API Config File

**Dosya:** `frontend/src/config/api.ts` (YENİ DOSYA)

```typescript
/**
 * API Configuration
 * Environment'a göre API base URL'ini belirler
 */

// API Base URL'i environment'a göre belirle
const getApiBaseUrl = (): string => {
  const env = import.meta.env.VITE_APP_ENV;
  const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
  
  // Production
  if (env === 'production') {
    return apiBaseUrl || 'https://te4it-api.azurewebsites.net';
  }
  
  // Development
  return apiBaseUrl || 'http://localhost:5001';
};

// Frontend URL'i environment'a göre belirle
const getFrontendUrl = (): string => {
  const env = import.meta.env.VITE_APP_ENV;
  const frontendUrl = import.meta.env.VITE_FRONTEND_URL;
  
  // Production
  if (env === 'production') {
    return frontendUrl || 'https://te4it-frontend.azurestaticapps.net';
  }
  
  // Development
  return frontendUrl || 'http://localhost:3000';
};

// Export edilen konfigürasyonlar
export const API_BASE_URL = getApiBaseUrl();
export const FRONTEND_URL = getFrontendUrl();
export const APP_ENV = import.meta.env.VITE_APP_ENV || 'development';

// API endpoints
export const API_ENDPOINTS = {
  AUTH: `${API_BASE_URL}/api/v1/auth`,
  PROJECTS: `${API_BASE_URL}/api/v1/projects`,
  TASKS: `${API_BASE_URL}/api/v1/tasks`,
  USERS: `${API_BASE_URL}/api/v1/users`
};

// Log configuration (development only)
if (APP_ENV === 'development') {
  console.log('🔧 API Configuration:', {
    API_BASE_URL,
    FRONTEND_URL,
    APP_ENV
  });
}
```

### 3.2 Update Existing API Services

**Mevcut dosyalarda kullanım:**

**Örnek:** `frontend/src/services/api.ts`

```typescript
import { API_BASE_URL, API_ENDPOINTS } from '@/config/api';

// Önceden hardcoded URL kullanıyorsanız, şimdi şöyle kullanın:
const response = await fetch(`${API_ENDPOINTS.AUTH}/login`, {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email, password })
});
```

---

## 4. Build Configuration

### 4.1 Package.json Scripts

**Dosya:** `frontend/package.json`

```json
{
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "build:prod": "vite build --mode production",
    "build:dev": "vite build --mode development"
  }
}
```

### 4.2 Build Commands

```bash
# Development build (testing)
npm run build:dev

# Production build (Azure deployment)
npm run build:prod

# Preview production build locally
npm run preview
```

---

## 5. GitHub Actions

### 5.1 Azure Static Web Apps Workflow

**Dosya:** `.github/workflows/azure-static-web-apps-te4it-frontend.yml`

Bu dosya Azure Static Web Apps oluşturulduğunda otomatik olarak oluşturulmalı. Eğer yoksa:

```yaml
name: Azure Static Web Apps CI/CD

on:
  push:
    branches:
      - main
  pull_request:
    types: [opened, synchronize, reopened, closed]
    branches:
      - main

jobs:
  build_and_deploy_job:
    if: github.event_name == 'push' || (github.event_name == 'pull_request' && github.event.action != 'closed')
    runs-on: ubuntu-latest
    name: Build and Deploy Job
    steps:
      - uses: actions/checkout@v3
        with:
          submodules: true
          lfs: false
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'npm'
          cache-dependency-path: './frontend/package-lock.json'
      
      - name: Install dependencies
        run: npm ci
        working-directory: ./frontend
      
      - name: Build
        run: npm run build
        working-directory: ./frontend
        env:
          VITE_API_BASE_URL: ${{ secrets.VITE_API_BASE_URL || 'https://te4it-api.azurewebsites.net' }}
          VITE_APP_ENV: production
      
      - name: Build And Deploy
        id: builddeploy
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN_TE4IT_FRONTEND }}
          repo_token: ${{ secrets.GITHUB_TOKEN }}
          action: "upload"
          app_location: "./frontend"
          api_location: ""
          output_location: "dist"
          ###### End of Repository/Build Configurations ######

  close_pull_request_job:
    if: github.event_name == 'pull_request' && github.event.action == 'closed'
    runs-on: ubuntu-latest
    name: Close Pull Request Job
    steps:
      - name: Close Pull Request
        id: closepullrequest
        uses: Azure/static-web-apps-deploy@v1
        with:
          azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN_TE4IT_FRONTEND }}
          action: "close"
```

---

## 6. Deployment Checklist

### 6.1 Dosya Kontrolleri

```bash
# Environment files
✅ frontend/.env.production (yeni oluştur)
✅ frontend/.env.development (var mı kontrol et)

# Config files
✅ frontend/vite.config.ts (update et)
✅ frontend/src/config/api.ts (yeni oluştur)
✅ frontend/package.json (scripts kontrol et)

# Git files
✅ frontend/.gitignore (.env dosyaları hariç)
✅ .github/workflows/azure-static-web-apps-te4it-frontend.yml (Azure tarafından oluşturulacak)
```

### 6.2 Build Testleri

```bash
# 1. Development build test
cd frontend
npm run build:dev

# 2. Production build test
npm run build:prod

# 3. Preview test
npm run preview
# Tarayıcıda http://localhost:4173 kontrol et

# 4. API connection test
# Browser console'da API_BASE_URL kontrol et
```

### 6.3 Git Commit

```bash
# Değişiklikleri commit et
git add frontend/.env.production
git add frontend/.env.development
git add frontend/src/config/api.ts
git add frontend/vite.config.ts
git add frontend/package.json

git commit -m "Add Azure Static Web Apps configuration"
git push origin main
```

---

## 7. Testing

### 7.1 Local Development Test

```bash
# Terminal 1: Backend
cd src
dotnet run

# Terminal 2: Frontend
cd frontend
npm run dev

# Browser: http://localhost:3000
```

**Kontrol Edilecekler:**
- ✅ Frontend çalışıyor mu?
- ✅ API connection çalışıyor mu?
- ✅ Login/Register çalışıyor mu?
- ✅ Environment variables doğru mu?

### 7.2 Production Build Test

```bash
# Production build
npm run build:prod

# Preview
npm run preview

# Browser: http://localhost:4173
```

**Kontrol Edilecekler:**
- ✅ Build başarılı mı?
- ✅ dist klasörü oluşturuldu mu?
- ✅ Preview çalışıyor mu?
- ✅ API URL production URL'i mi?

### 7.3 Azure Deployment Test

**URL:** `https://te4it-frontend.azurestaticapps.net`

**Kontrol Edilecekler:**
- ✅ Site açılıyor mu?
- ✅ API connection çalışıyor mu?
- ✅ Login/Register çalışıyor mu?
- ✅ Production URL'ler doğru mu?

---

## 🚀 Cursor AI Kullanım Talimatları

### Beni Cursor'a Yapıştır ve Şunu Söyle:

```
"Bu dokümantasyona göre frontend projem için tüm değişiklikleri yap:

1. Environment files oluştur (.env.production, .env.development)
2. Vite config'i güncelle
3. API config dosyası oluştur
4. Existing API services'i güncelle
5. Package.json scripts'leri kontrol et
6. Build test'leri yap
7. Git commit için hazırla

Tüm değişiklikleri yap ama dosyaları kaydetme, sadece göster."
```

### Veya Her Adımı Teker Teker:

```
# 1. Environment files
"frontend klasöründe .env.production ve .env.development dosyaları oluştur"

# 2. Vite config
"frontend/vite.config.ts dosyasını güncelle"

# 3. API config
"frontend/src/config/api.ts dosyası oluştur"

# 4. API services
"frontend/src/services/api.ts dosyasını güncelle"
```

---

## 📞 Yardım ve Destek

### Sorun Giderme

1. **Build Hatası:** `npm run build` çalıştırın ve hataları kontrol edin
2. **API Connection Hatası:** Environment variables'ları kontrol edin
3. **Deployment Hatası:** GitHub Actions logs'u kontrol edin

### İletişim

- **Email:** infoarslanbas@gmail.com
- **GitHub:** https://github.com/burhanarslanbas/TE4IT
- **Azure Portal:** https://portal.azure.com

---

*Bu dokümantasyon TE4IT Frontend Azure Deployment Guide v1.0 için hazırlanmıştır.*

