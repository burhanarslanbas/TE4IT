# Mimari Dokümantasyonu

Bu klasör TE4IT projesinin sistem mimarisi, diyagramları ve teknik tasarım dokümantasyonlarını içerir.

## 📋 İçindekiler

### 🏗️ Sistem Diyagramları
- **[DIAGRAMS.md](./DIAGRAMS.md)** - Tüm sistem diyagramları
  - Sistem Mimarisi Diyagramı
  - Veritabanı ER Diyagramı
  - Kullanıcı Akış Diyagramı
  - API Endpoint Diyagramı
  - Sprint Timeline Diyagramı
  - Teknoloji Stack Diyagramı
  - Güvenlik Mimarisi Diyagramı

### 📊 Task Management
- **[DIAGRAM_TaskManagement.md](./DIAGRAM_TaskManagement.md)** - Görev yönetimi sınıf diyagramı
  - Domain entity'leri ve ilişkileri
  - Mermaid diyagramı ile görselleştirme

### 🤖 AI Entegrasyonu
- **[AI_INTEGRATION.md](./AI_INTEGRATION.md)** - AI servisi entegrasyonu
- **[BACKEND_AI_INTEGRATION.md](./BACKEND_AI_INTEGRATION.md)** - Backend AI implementasyonu

## 🎯 Mimari Genel Bakış

### Onion Architecture
```
┌─────────────────────────────────────┐
│           API Layer                 │ ← Controllers, Middleware
├─────────────────────────────────────┤
│        Application Layer            │ ← Use Cases, Handlers
├─────────────────────────────────────┤
│          Domain Layer               │ ← Entities, Business Rules
├─────────────────────────────────────┤
│       Infrastructure Layer          │ ← Database, External Services
└─────────────────────────────────────┘
```

### Teknoloji Stack
- **Backend**: .NET 9 Web API + Onion Architecture + CQRS
- **Frontend**: React 18 + TypeScript + Material-UI
- **Mobile**: Kotlin + Jetpack Compose + MVVM
- **AI Service**: FastAPI (Python) + scikit-learn
- **Database**: PostgreSQL (Tasks) + MongoDB (Education)
- **Cache**: Redis
- **Authentication**: JWT + Refresh Token

### Veritabanı Mimarisi
- **PostgreSQL**: Task Module (ACID uyumlu)
- **MongoDB**: Education Module (esnek şema)
- **Redis**: Cache ve session yönetimi

## 🔗 Hızlı Linkler

- **[Sistem Diyagramları](./DIAGRAMS.md)** - Tüm diyagramları görüntüle
- **[Task Management Diyagramı](./DIAGRAM_TaskManagement.md)** - Domain modeli
- **[PRD Dokümanı](../project-management/PRD_TE4IT_FINAL.md)** - Detaylı gereksinimler

## 📚 Mimari Prensipler

### Clean Architecture
- **Dependency Inversion**: İç katmanlar dış katmanlara bağımlı değil
- **Separation of Concerns**: Her katmanın tek sorumluluğu var
- **Testability**: Domain katmanı bağımsız test edilebilir

### CQRS Pattern
- **Command**: Veriyi değiştiren işlemler
- **Query**: Veriyi okuma işlemleri
- **MediatR**: Command/Query handler'ları

### Domain-Driven Design (DDD)
- **Aggregate Root**: Veri tutarlılığını sağlar
- **Value Objects**: Immutable nesneler
- **Domain Events**: Önemli olayları bildirir

## 🛠️ Geliştirme Araçları

### Diyagram Oluşturma
- **Mermaid**: Markdown içinde diyagramlar
- **PlantUML**: UML diyagramları
- **Draw.io**: Görsel diyagramlar

### Mimari Dokümantasyon
- **Swagger**: API dokümantasyonu
- **XML Comments**: Kod dokümantasyonu
- **Markdown**: Genel dokümantasyon

## 📞 Destek

Mimari konularında sorun yaşarsanız:
1. İlgili diyagramı kontrol edin
2. PRD dokümanını inceleyin
3. GitHub Issues'da sorun bildirin
