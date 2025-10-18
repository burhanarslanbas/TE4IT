# TE4IT Dokümantasyon Merkezi

**Sürüm:** 1.0  
**Tarih:** Ocak 2025  
**Proje:** TE4IT - Proje Yönetimi ve Eğitim Platformu

---

## 📚 Dokümantasyon Klasör Yapısı

### 🚀 [API Dokümantasyonu](./api/)
API endpoint'leri ve kullanım kılavuzları
- **[Authentication API](./api/AUTH_API_DOCUMENTATION.md)** - Kimlik doğrulama endpoint'leri
- **Projects API** - Proje yönetimi endpoint'leri (yakında)
- **Tasks API** - Görev yönetimi endpoint'leri (yakında)
- **Education API** - Eğitim modülü endpoint'leri (yakında)

### 🏗️ [Mimari Dokümantasyonu](./architecture/)
Sistem mimarisi ve teknik tasarım
- **[Sistem Diyagramları](./architecture/DIAGRAMS.md)** - Tüm sistem diyagramları
- **[Task Management Diyagramı](./architecture/DIAGRAM_TaskManagement.md)** - Görev yönetimi sınıf diyagramı
- **[AI Entegrasyonu](./architecture/AI_INTEGRATION.md)** - AI servisi entegrasyonu
- **[Backend AI Entegrasyonu](./architecture/BACKEND_AI_INTEGRATION.md)** - Backend AI implementasyonu

### 👨‍💻 [Geliştirme Dokümantasyonu](./development/)
Geliştirici kılavuzları ve süreçler
- **[Geliştirme Kılavuzu](./development/DEVELOPMENT.md)** - Proje kurulumu ve geliştirme
- **[Git Branching Stratejisi](./development/GIT_BRANCHING.md)** - Git workflow ve branch yönetimi

### 📋 [Proje Yönetimi](./project-management/)
Proje gereksinimleri ve planlama
- **[Ürün Gereksinimleri Dokümanı (PRD)](./project-management/PRD_TE4IT_FINAL.md)** - Ana PRD dokümanı

---

## 🎯 Hızlı Başlangıç

### API'leri Test Etmek İçin:
1. **[Authentication API](./api/AUTH_API_DOCUMENTATION.md)** dokümanını inceleyin
2. API'yi çalıştırın: `dotnet run`
3. Swagger UI'ya gidin: `https://localhost:5001/swagger`

### Proje Mimarisi İçin:
1. **[Sistem Diyagramları](./architecture/DIAGRAMS.md)** ile başlayın
2. **[PRD Dokümanı](./project-management/PRD_TE4IT_FINAL.md)** ile detayları öğrenin

### Geliştirme İçin:
1. **[Geliştirme Kılavuzu](./development/DEVELOPMENT.md)** ile projeyi kurun
2. **[Git Branching](./development/GIT_BRANCHING.md)** ile workflow'u öğrenin

---

## 🔗 Hızlı Linkler

### API Endpoints
- **Authentication**: `POST /api/v1/auth/login`
- **Projects**: `GET /api/v1/projects`
- **Tasks**: `GET /api/v1/tasks`
- **Education**: `GET /api/v1/courses`

### Swagger UI
- **Local**: https://localhost:5001/swagger
- **API Base URL**: https://localhost:5001/api/v1

### Teknoloji Stack
- **Backend**: .NET 9 Web API
- **Frontend**: React + TypeScript
- **Mobile**: Kotlin + Jetpack Compose
- **AI Service**: FastAPI (Python)
- **Database**: PostgreSQL + MongoDB

---

## 📞 Destek ve İletişim

**Sorun yaşarsanız:**
1. İlgili dokümantasyonu kontrol edin
2. Swagger UI'da test edin
3. GitHub Issues'da sorun bildirin

**İletişim:**
- **GitHub**: https://github.com/burhanarslanbas/TE4IT
- **Swagger UI**: https://localhost:5001/swagger
- **Team Email**: team@te4it.com
---

## 📝 Dokümantasyon Güncellemeleri

| Tarih | Güncelleme | Açıklama |
|-------|------------|----------|
| Ocak 2025 | v1.0 | İlk dokümantasyon yapısı oluşturuldu |
| Ocak 2025 | v1.1 | Authentication API dokümantasyonu eklendi |
| Ocak 2025 | v1.2 | Klasör yapısı organize edildi |

---

*Bu dokümantasyon merkezi TE4IT projesi için hazırlanmıştır. Son güncelleme: Ocak 2025*
