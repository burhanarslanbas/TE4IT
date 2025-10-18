# TE4IT - Ürün Gereksinimleri Dokümanı (PRD)

**Sürüm:** 1.0  
**Tarih:** Ocak 2025  
**Hazırlayan:** Geliştirme Ekibi  

---

## 1. Yürütücü Özeti

### 1.1 Ürün Vizyonu
TE4IT, yazılım geliştirme ekipleri ve BT öğrencileri için proje yönetimi ile eğitimi tek platformda birleştiren kapsamlı bir sistemdir.

### 1.2 Temel Değer Önerileri
- **Birleşik Platform**: Dağınık araçları tek sistemde toplar
- **Eğitim Entegrasyonu**: Proje çalışmasını öğrenme hedefleriyle harmanlar  
- **AI Destekli İçgörüler**: Akıllı analiz ve öneriler sağlar

### 1.3 İş Hedefleri
- Araç parçalanmasını %80 azaltmak
- Ekip verimliliğini %25 artırmak
- Proje teslim sürelerini %20 kısaltmak

---

## 2. Ürün Genel Bakışı

### 2.1 Ürün Tanımı
TE4IT, yazılım geliştirme ekiplerinin projelerini hiyerarşik yapıda yönetebileceği, eğitim içeriği ve AI destekli analitiklerle entegre bir SaaS platformudur.

### 2.2 Temel Özellikler
1. **Proje Yönetimi**: Hiyerarşik organizasyon (Proje → Modül → Kullanım Senaryosu → Görev)
2. **Görev Yönetimi**: Gelişmiş görev takibi ve bağımlılık yönetimi
3. **Eğitim İçeriği**: Kurs ve ders yönetimi ile ilerleme takibi
4. **AI Analitikleri**: Akıllı proje analizi ve görev tahminleri

### 2.3 Platform Mimarisi
- **Backend**: .NET 9 Web API (Onion Architecture + CQRS)
- **Frontend**: React + TypeScript
- **Mobile**: React Native (okuma odaklı)
- **AI Servisi**: FastAPI (Python)
- **Veritabanı**: PostgreSQL

---

## 3. Pazar Analizi ve Problem Tanımı

### 3.1 Mevcut Pazar Sorunları
- **Araç Dağınıklığı**: Ekipler 5-7 farklı araç kullanıyor (Jira, Trello, Slack, vb.)
- **Bağlam Değiştirme**: Sürekli araç değiştirme verimliliği düşürüyor
- **Sınırlı Eğitim Entegrasyonu**: Proje çalışması ve eğitim ayrı kalıyor

### 3.2 Hedef Pazar Segmentleri
1. **Akademik Kurumlar**: Bilgisayar mühendisliği bölümleri ve BT programları
2. **Yazılım Geliştirme Ekipleri**: Küçük-orta ölçekli geliştirme ekipleri

### 3.3 Rekabet Analizi
- **Direkt Rakip**: Jira, Asana, Monday.com
- **Farklılaşma**: Proje yönetimi ve eğitimin AI destekli entegrasyonu

---

## 4. Hedef Kullanıcılar ve Persona'lar

### 4.1 Ana Kullanıcı Tipleri

#### 4.1.1 Şirket Sahibi (Ahmet)
- **Kim**: 15 yıllık deneyimli yazılım şirketi kurucusu
- **Ne İstiyor**: Ekiplerinin verimliliğini artırmak, maliyetleri düşürmek
- **Sorunu**: Ekipler farklı araçlar kullanıyor, lisans maliyetleri yüksek
- **Çözüm**: Tek platform, maliyet tasarrufu, ekip performans raporları

#### 4.1.2 Yazılım Müşterisi (Zeynep)
- **Kim**: E-ticaret şirketi IT müdürü, yazılım geliştirme talep ediyor
- **Ne İstiyor**: Projelerinin zamanında teslimini sağlamak, kaliteyi artırmak
- **Sorunu**: Geliştirme sürecini göremiyor, iletişim kopukluğu var
- **Çözüm**: Proje takibi, ilerleme raporları, şeffaf süreç

#### 4.1.3 Proje Yöneticisi (Ayşe)
- **Kim**: 5 yıllık deneyimli proje yöneticisi
- **Ne İstiyor**: Ekiplerini organize etmek, projeleri takip etmek
- **Sorunu**: Jira, Slack, Notion arasında sürekli geçiş yapmak zorunda
- **Çözüm**: Tek platformda dashboard, raporlama, AI önerileri

#### 4.1.4 Yazılım Geliştirici (Mehmet)
- **Kim**: 3 yıllık deneyimli backend geliştirici
- **Ne İstiyor**: Görevlerini verimli tamamlamak, yeni teknolojiler öğrenmek
- **Sorunu**: Hangi görevin öncelikli olduğunu bilmiyor, bağımlılıkları takip edemiyor
- **Çözüm**: Görev yönetimi, bağımlılık takibi, eğitim içeriği

#### 4.1.5 BT Öğrencisi (Elif)
- **Kim**: Bilgisayar mühendisliği 3. sınıf öğrencisi
- **Ne İstiyor**: Projelerini organize etmek, öğrendiklerini uygulamak
- **Sorunu**: Proje yönetimi ile ders çalışması ayrı kalıyor
- **Çözüm**: Mobil uygulama, kurs takibi, ilerleme raporları

### 4.2 Kullanıcı Yolculuğu
- **Ahmet'in Günü**: Dashboard → Ekip Performansı → Maliyet Analizi → ROI Raporu
- **Zeynep'in Günü**: Giriş → Proje Durumu → İlerleme Raporu → Müşteri Geri Bildirimi
- **Ayşe'nin Günü**: Giriş → Dashboard → Yeni Proje → Modül Ekle → Görev Ata
- **Mehmet'in Günü**: Giriş → Atanan Görevler → Görev Başlat → Kod Yaz → Durum Güncelle
- **Elif'in Günü**: Mobil Giriş → Kurslar → Ders İzle → Proje Güncelle → İlerleme Kontrol Et

---

## 5. Ürün Hedefleri

### 5.1 Ana Hedefler
1. **Birleşik Platform**: Dağınık araçları tek sistemde toplayarak %80 azaltmak
2. **Verimlilik Artışı**: Ekip verimliliğini %25 artırmak
3. **Veri Odaklı Kararlar**: AI destekli içgörülerle daha iyi karar verme

### 5.2 Başarı Kriterleri
- Kullanıcılar tek platformda çalışabilir
- Proje teslim süreleri kısalır
- Ekip koordinasyonu artar

---

## 6. Fonksiyonel Gereksinimler

### 6.1 Kimlik Doğrulama ve Yetkilendirme
- **Kullanıcı Kaydı**: Email ve şifre ile hesap oluşturma
- **Giriş/Çıkış**: JWT token tabanlı kimlik doğrulama
- **Rol Yönetimi**: Yönetici, Proje Yöneticisi, Geliştirici, Öğrenci rolleri

### 6.2 Proje Yönetimi
- **Proje CRUD**: Proje oluşturma, düzenleme, listeleme
- **Hiyerarşik Yapı**: Proje → Modül → Kullanım Senaryosu → Görev
- **Durum Yönetimi**: Aktif/Arşivlenmiş proje durumları

### 6.3 Görev Yönetimi
- **Görev CRUD**: Görev oluşturma, atama, güncelleme
- **Durum Geçişleri**: Başlamadı → Devam Ediyor → Tamamlandı
- **Bağımlılık Yönetimi**: Görevler arası ilişki ve bağımlılık takibi

### 6.4 Eğitim İçeriği
- **Kurs Yönetimi**: Kurs oluşturma ve düzenleme
- **İlerleme Takibi**: Kullanıcı kurs ilerlemesi ve tamamlama oranları

### 6.5 AI Destekli Özellikler
- **Proje Analizi**: Risk değerlendirmesi ve öneriler
- **Görev Tahmini**: AI destekli süre ve effort tahmini

---

## 7. Fonksiyonel Olmayan Gereksinimler

### 7.1 Performans Gereksinimleri
- **API Yanıt Süresi**: < 800ms ortalama yanıt süresi
- **Eşzamanlı Kullanıcı**: 1000+ eşzamanlı kullanıcı desteği
- **Veritabanı**: Sorgu süreleri < 200ms

### 7.2 Güvenilirlik ve Erişilebilirlik
- **Sistem Uptime**: %99.9 erişilebilirlik
- **Hata Oranı**: < 0.1% hata oranı
- **Yedekleme**: Günlük otomatik veri yedekleme

### 7.3 Güvenlik Gereksinimleri
- **Kimlik Doğrulama**: JWT token tabanlı güvenlik
- **Veri Şifreleme**: HTTPS ve veritabanı şifreleme
- **Giriş Güvenliği**: Başarısız giriş denemesi sınırlaması

### 7.4 Kullanılabilirlik
- **Öğrenme Eğrisi**: Yeni kullanıcılar 30 dakikada temel işlemleri öğrenebilir
- **Responsive Tasarım**: Web ve mobil uyumlu arayüz
- **Erişilebilirlik**: WCAG 2.1 AA standartlarına uygunluk

---

## 8. Teknik Mimari

### 8.1 Sistem Mimarisi
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Web Client    │    │  Mobile Client  │    │   AI Service    │
│   (React/TS)    │    │   (Kotlin)      │    │   (FastAPI)     │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          └──────────────────────┼──────────────────────┘
                                 │
                    ┌─────────────┴─────────────┐
                    │    .NET 9 Web API         │
                    │  (Onion Architecture,     │
                    │   CQRS, MediatR)          │
                    └─────────────┬─────────────┘
                                  │
                    ┌─────────────┴─────────────┐
                    │    Veritabanı Katmanı     │
                    │  ┌─────────┐ ┌─────────┐  │
                    │  │PostgreSQL│ │ MongoDB │ │
                    │  │(Görevler)│ │(Eğitim) │ │
                    │  └─────────┘ └─────────┘  │
                    └───────────────────────────┘
```

### 8.2 Teknoloji Stack'i

#### 8.2.1 Backend Teknolojileri
- **Framework**: .NET 9 Web API
- **Mimari**: Onion Architecture + CQRS Pattern
- **Görev Veritabanı**: PostgreSQL 15+ + Entity Framework Core 9.0
- **Eğitim Veritabanı**: MongoDB + MongoDB Driver
- **Kimlik Doğrulama**: JWT + Refresh Token
- **Validasyon**: FluentValidation
- **Mediation**: MediatR (CQRS implementasyonu)
- **Logging**: Serilog (structured logging)
- **Cache**: Redis (distributed caching)

#### 8.2.2 Frontend Teknolojileri
- **Framework**: React 18 + TypeScript
- **State Management**: Redux Toolkit
- **UI Library**: Material-UI (MUI)
- **Routing**: React Router v6
- **HTTP Client**: Axios (interceptors ile)
- **Build Tool**: Vite
- **Testing**: Jest + React Testing Library

#### 8.2.3 Mobil Teknolojileri
- **Uygulama Dili**: Kotlin
- **UI Framework**: Jetpack Compose
- **Mimari Desen**: MVVM (Model-ViewModel-View)
- **Veri İletişimi**: REST API
- **HTTP Client**: Retrofit
- **Veri Depolama**: Room Database
- **Push Bildirimler**: Firebase Cloud Messaging (FCM)
- **Offline Desteği**: Room database ile cache

#### 8.2.4 AI Servisi Teknolojileri
- **Framework**: FastAPI (Python)
- **ML Kütüphaneleri**: scikit-learn, pandas, numpy
- **Web Scraping**: BeautifulSoup, requests
- **Cache**: Redis (response caching)
- **Dokümantasyon**: OpenAPI/Swagger

### 8.3 Veritabanı Tasarımı

#### 8.3.1 PostgreSQL (Görev Modülü)
```sql
-- Kullanıcılar ve Kimlik Doğrulama
Users (Id, Email, UserName, PasswordHash, CreatedAt, UpdatedAt)
Roles (Id, Name, Description, CreatedAt)
UserRoles (UserId, RoleId, AssignedAt)
RefreshTokens (Id, UserId, Token, ExpiresAt, CreatedAt)

-- Proje Yönetimi
Projects (Id, Title, Description, CreatorId, StartedDate, IsActive, CreatedAt, UpdatedAt)
Modules (Id, ProjectId, Title, Description, CreatorId, StartedDate, IsActive, CreatedAt, UpdatedAt)
UseCases (Id, ModuleId, Title, Description, ImportantNotes, CreatorId, StartedDate, IsActive, CreatedAt, UpdatedAt)
Tasks (Id, UseCaseId, Title, Description, ImportantNotes, CreatorId, AssigneeId, StartedDate, DueDate, TaskType, TaskState, CreatedAt, UpdatedAt)
TaskRelations (Id, SourceTaskId, TargetTaskId, RelationType, CreatedAt)
```

#### 8.3.2 MongoDB (Eğitim Modülü)
```javascript
// Kurslar Koleksiyonu
Courses: {
  _id: ObjectId,
  title: String,
  description: String,
  instructor: String,
  duration: Number, // dakika
  difficulty: String, // beginner, intermediate, advanced
  tags: [String],
  createdAt: Date,
  updatedAt: Date
}

// Dersler Koleksiyonu
Lessons: {
  _id: ObjectId,
  courseId: ObjectId,
  title: String,
  content: String,
  order: Number,
  duration: Number, // dakika
  videoUrl: String,
  attachments: [String],
  createdAt: Date
}

// Kayıtlar Koleksiyonu
Enrollments: {
  _id: ObjectId,
  userId: String, // PostgreSQL'den referans
  courseId: ObjectId,
  enrolledAt: Date,
  completedAt: Date,
  progress: Number // 0-100
}

// İlerleme Koleksiyonu
Progress: {
  _id: ObjectId,
  userId: String, // PostgreSQL'den referans
  lessonId: ObjectId,
  percentage: Number, // 0-100
  completedAt: Date,
  timeSpent: Number // dakika
}
```

#### 8.3.3 İndeksleme Stratejisi

**PostgreSQL İndeksleri:**
- **Primary Keys**: Tüm ID kolonlarında clustered indeksler
- **Foreign Keys**: Foreign key kolonlarında non-clustered indeksler
- **Arama Alanları**: Title ve description alanlarında full-text indeksler
- **Performans**: Yaygın sorgu desenleri için composite indeksler
- **Audit**: CreatedAt ve UpdatedAt için zaman tabanlı sorgular için indeksler

**MongoDB İndeksleri:**
- **Compound Indexes**: userId + courseId kombinasyonları için
- **Text Indexes**: Kurs ve ders içeriklerinde arama için
- **TTL Indexes**: Geçici veriler için otomatik silme
- **Sparse Indexes**: Opsiyonel alanlar için

### 8.4 API Mimarisi

#### 8.4.1 RESTful API Tasarımı
- **Base URL**: `https://api.te4it.com/v1`
- **Kimlik Doğrulama**: Authorization header'da Bearer token
- **Content Type**: `application/json`
- **Hata Formatı**: RFC 7807 Problem Details
- **Sayfalama**: Cursor-based pagination
- **Rate Limiting**: Kullanıcı başına saatte 1000 istek

#### 8.4.2 API Versiyonlama
- **Versiyonlama Stratejisi**: URL path versiyonlama (`/v1/`, `/v2/`)
- **Geriye Uyumluluk**: 12 ay boyunca geriye uyumluluk korunur
- **Deprecation Policy**: API kullanımdan kaldırma için 6 aylık bildirim
- **Dokümantasyon**: OpenAPI 3.0 spesifikasyonu

---

## 9. API Spesifikasyonları

### 9.1 Kimlik Doğrulama Endpoint'leri

#### 9.1.1 Kullanıcı Kaydı
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "userName": "john.doe",
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

**Response (201 Created):**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "userName": "john.doe",
  "email": "john.doe@example.com",
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-13T15:30:00Z"
}
```

#### 9.1.2 Kullanıcı Girişi
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "SecurePass123!"
}
```

### 9.2 Proje Yönetimi Endpoint'leri

#### 9.2.1 Proje Oluşturma
```http
POST /api/v1/projects
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "title": "E-Commerce Platform",
  "description": "Modern e-commerce platform with microservices architecture"
}
```

### 9.3 Görev Yönetimi Endpoint'leri

#### 9.3.1 Görev Oluşturma
```http
POST /api/v1/tasks
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "useCaseId": "789e0123-e89b-12d3-a456-426614174002",
  "title": "Implement User Authentication",
  "description": "Create JWT-based authentication system",
  "taskType": "Feature"
}
```

### 9.4 Eğitim Endpoint'leri

#### 9.4.1 Kurs Listesi
```http
GET /api/v1/courses
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (200 OK):**
```json
{
  "data": [
    {
      "_id": "507f1f77bcf86cd799439011",
      "title": "React Fundamentals",
      "description": "Learn React from scratch",
      "instructor": "John Doe",
      "duration": 480,
      "difficulty": "beginner",
      "tags": ["react", "javascript", "frontend"]
    }
  ]
}
```

### 9.5 Hata Yanıt Formatı
```json
{
  "type": "https://api.te4it.com/problems/validation-error",
  "title": "Validation Error",
  "status": 400,
  "detail": "The request contains invalid data",
  "errors": [
    {
      "field": "title",
      "message": "Title must be between 3 and 120 characters"
    }
  ]
}
```

---

## 10. Proje Zaman Çizelgesi ve Kilometre Taşları

### 10.1 Sprint Planlaması (6 Sprint - 6 Hafta)

#### Sprint 1: Temel Altyapı (1 hafta)
**Backend Developer:**
- .NET 9 Web API kurulumu
- Onion Architecture implementasyonu
- PostgreSQL ve MongoDB bağlantıları
- JWT kimlik doğrulama sistemi

**Frontend Developer:**
- React projesi kurulumu
- Material-UI entegrasyonu
- Temel routing yapısı

**Mobile Developer:**
- Kotlin Android projesi kurulumu
- Jetpack Compose setup
- Retrofit HTTP client kurulumu

**AI Developer:**
- FastAPI projesi kurulumu
- Temel ML kütüphaneleri kurulumu

#### Sprint 2: Proje Yönetimi (1 hafta)
**Backend Developer:**
- Proje CRUD API'leri
- Modül ve kullanım senaryosu API'leri
- Entity Framework migrations

**Frontend Developer:**
- Proje listesi ve detay sayfaları
- Proje oluşturma formu
- Modül yönetimi arayüzü

**Mobile Developer:**
- Proje listesi ekranı (okuma)
- Proje detay ekranı (okuma)
- Room database kurulumu

**AI Developer:**
- Temel proje analizi algoritması
- Risk değerlendirme modeli

#### Sprint 3: Görev Yönetimi (1 hafta)
**Backend Developer:**
- Görev CRUD API'leri
- Görev durum geçişleri
- Bağımlılık yönetimi API'leri

**Frontend Developer:**
- Görev listesi ve detay sayfaları
- Görev oluşturma ve düzenleme
- Durum geçiş arayüzü

**Mobile Developer:**
- Görev listesi ekranı (okuma)
- Görev detay ekranı (okuma)
- Offline cache implementasyonu

**AI Developer:**
- Görev tahmin algoritması
- Effort estimation modeli

#### Sprint 4: Eğitim Modülü (1 hafta)
**Backend Developer:**
- MongoDB kurs API'leri
- Ders içerik API'leri
- İlerleme takibi API'leri

**Frontend Developer:**
- Kurs listesi ve detay sayfaları
- Ders içerik görüntüleme
- İlerleme takip arayüzü

**Mobile Developer:**
- Kurs listesi ekranı (okuma)
- Ders içerik ekranı (okuma)
- İlerleme takip ekranı

**AI Developer:**
- Öğrenme analizi algoritması
- Kişiselleştirilmiş öneriler

#### Sprint 5: Frontend ve Mobil (1 hafta)
**Backend Developer:**
- API optimizasyonları
- Performance iyileştirmeleri
- Error handling

**Frontend Developer:**
- Dashboard implementasyonu
- Raporlama arayüzleri
- Responsive tasarım

**Mobile Developer:**
- Push notification entegrasyonu
- Offline sync implementasyonu
- UI/UX iyileştirmeleri

**AI Developer:**
- AI servisi optimizasyonu
- Caching implementasyonu

#### Sprint 6: AI ve Finalizasyon (1 hafta)
**Backend Developer:**
- Production hazırlığı
- Security hardening
- Performance tuning

**Frontend Developer:**
- Final UI polish
- Bug fixes
- User testing

**Mobile Developer:**
- Final testing
- App store hazırlığı
- Performance optimization

**AI Developer:**
- AI model fine-tuning
- Production deployment
- Monitoring setup

### 10.2 Kilometre Taşları
- **MVP Release**: 4 hafta
- **Beta Test**: 5 hafta
- **Production Release**: 6 hafta

### 10.3 Takım Yapısı
- **Backend Developer**: .NET API ve veritabanları
- **Frontend Developer**: React web uygulaması
- **Mobile Developer**: Kotlin Android uygulaması
- **AI Developer**: FastAPI servisi
- **QA Engineer**: Test ve kalite güvencesi

---

## 11. Risk Değerlendirmesi

### 11.1 Teknik Riskler

#### 11.1.1 Yüksek Risk
**Risk**: AI Servisi Entegrasyon Karmaşıklığı
- **Olasılık**: Orta
- **Etki**: Yüksek
- **Mitigasyon**: Basit AI özellikleriyle başla, fallback mekanizmaları ekle

**Risk**: Performans Yük Altında
- **Olasılık**: Orta
- **Etki**: Yüksek
- **Mitigasyon**: Kapsamlı performans testleri, caching stratejileri

### 11.2 İş Riskleri

#### 11.2.1 Orta Risk
**Risk**: Kapsam Genişlemesi
- **Olasılık**: Yüksek
- **Etki**: Orta
- **Mitigasyon**: Net gereksinim dokümantasyonu, değişiklik kontrol süreci

**Risk**: Zaman Baskısı
- **Olasılık**: Orta
- **Etki**: Yüksek
- **Mitigasyon**: Gerçekçi zaman tahminleri, buffer süreleri

### 11.3 Dış Riskler

#### 11.3.1 Düşük Risk
**Risk**: Teknoloji Bağımlılıkları
- **Olasılık**: Düşük
- **Etki**: Orta
- **Mitigasyon**: Kararlı, iyi desteklenen teknolojiler kullan

### 11.4 Risk İzleme
- **Risk Kaydı**: Kapsamlı risk dokümantasyonu
- **Düzenli İncelemeler**: Haftalık risk değerlendirme güncellemeleri
- **Mitigasyon Takibi**: Risk azaltma eylemlerinin ilerlemesi

---

## 12. Sonuç

TE4IT projesi, yazılım geliştirme ekiplerinin verimliliğini artırmak ve araç dağınıklığını azaltmak için tasarlanmış kapsamlı bir proje yönetim platformudur. Onion Architecture ve modern teknolojilerle geliştirilecek sistem, 6 haftalık sprint süreci sonunda MVP olarak piyasaya sunulacaktır.

### 12.1 Proje Özeti
- **Toplam Geliştirme Süresi**: 6 hafta (6 sprint)
- **Takım Büyüklüğü**: 5 geliştirici
- **Hedef Kullanıcı**: 100+ aktif kullanıcı
- **Teknoloji Stack**: .NET 9, React, Kotlin, FastAPI
- **Veritabanları**: PostgreSQL (görevler) + MongoDB (eğitim)

### 12.2 Başarı Kriterleri
- Kullanıcılar tek platformda çalışabilir
- Proje teslim süreleri kısalır
- Ekip koordinasyonu artar
- AI destekli içgörüler sağlanır

### 12.3 Gelecek Planları
- Kullanıcı geri bildirimlerine göre özellik geliştirme
- Performans optimizasyonları
- Yeni AI özellikleri ekleme
- Mobil uygulama genişletme

---

**Doküman Kontrolü:**
- **Versiyon**: 1.0
- **Son Güncelleme**: Ocak 2025
- **Sonraki İnceleme**: Şubat 2025
- **Onaylayan**: Geliştirme Ekibi
- **Dağıtım**: Takım Üyeleri, Paydaşlar

---

*Bu doküman TE4IT Geliştirme Ekibi'ne özeldir. Yetkisiz dağıtım yasaktır.*