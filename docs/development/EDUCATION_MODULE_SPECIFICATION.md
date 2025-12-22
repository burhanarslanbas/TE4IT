# Education Module - Kapsamlı Geliştirme Dokümanı

**Versiyon:** 1.0  
**Tarih:** 2025-01-27  
**Hazırlayan:** TE4IT Development Team  
**Durum:** ⏳ Geliştirme Aşamasında

---

## 📋 İçindekiler

1. [Genel Bakış](#1-genel-bakış)
2. [Mimari Yaklaşım](#2-mimari-yaklaşım)
3. [Veri Modeli](#3-veri-modeli)
4. [Fonksiyonel Gereksinimler](#4-fonksiyonel-gereksinimler)
5. [Kullanıcı Senaryoları](#5-kullanıcı-senaryoları)
6. [Yetkilendirme ve Roller](#6-yetkilendirme-ve-roller)
7. [API Tasarımı](#7-api-tasarımı)
8. [Roadmap Yapısı](#8-roadmap-yapısı)
9. [İçerik Tipleri](#9-içerik-tipleri)
10. [İlerleme Takibi](#10-ilerleme-takibi)
11. [Teknik Detaylar](#11-teknik-detaylar)

---

## 1. Genel Bakış

### 1.1 Amaç

TE4IT Eğitim Modülü, kullanıcıların yapılandırılmış eğitim içeriklerine erişmesini, adım adım öğrenme yolculuğunu takip etmesini ve ilerlemelerini görselleştirmesini sağlar. Modül, **roadmap tabanlı öğrenme** yaklaşımını benimser ve kullanıcıların sistematik bir şekilde eğitim içeriklerini tamamlamasını destekler.

### 1.2 Temel Özellikler

- ✅ **Kurs Yönetimi**: Admin ve Kurum Müdürü tarafından kurs oluşturma ve yönetimi
- ✅ **Roadmap Sistemi**: Her kurs için adım adım öğrenme yolu (roadmap)
- ✅ **Çoklu İçerik Desteği**: Video linkleri, doküman linkleri ve zengin metin içerikleri
- ✅ **İlerleme Takibi**: Kullanıcı bazlı detaylı ilerleme takibi
- ✅ **Kayıt Sistemi**: Kullanıcıların kurslara kayıt olması ve takip etmesi
- ✅ **Merkezi Erişim**: Tüm kullanılabilir kursların tek ekranda görüntülenmesi

### 1.3 Kullanıcı Tipleri

- **Admin (Administrator)**: Kurs oluşturma, düzenleme, silme yetkisi
- **Kurum Müdürü (OrganizationManager)**: Kurs oluşturma, düzenleme yetkisi
- **Tüm Kullanıcılar**: Kurs görüntüleme, kayıt olma, içerik erişimi, ilerleme takibi

---

## 2. Mimari Yaklaşım

### 2.1 Veritabanı Seçimi

**MongoDB** kullanılacak. Sebepleri:
- Farklı veritabanları ile çalışma deneyimi (PostgreSQL + MongoDB hibrit yaklaşım)
- Esnek şema yapısı ile eğitim içeriklerinin dinamik yapısına uygunluk
- Embedded documents ile roadmap yapısının doğal temsili
- Ölçeklenebilirlik ve performans avantajları
- NoSQL yaklaşımı ile esnek içerik yönetimi

**Hibrit Yaklaşım:**
- **PostgreSQL**: Proje yönetimi, kullanıcı yönetimi, görev takibi (mevcut)
- **MongoDB**: Eğitim modülü (kurslar, roadmap, içerikler, ilerleme)

### 2.2 Clean Architecture Katmanları

```
TE4IT.Domain
  └── Entities/Education/
      ├── Course.cs
      ├── CourseRoadmap.cs
      ├── RoadmapStep.cs
      ├── CourseContent.cs
      ├── Enrollment.cs
      └── Progress.cs

TE4IT.Application
  └── Features/Education/
      ├── Courses/
      │   ├── Commands/
      │   │   ├── CreateCourse/
      │   │   ├── UpdateCourse/
      │   │   └── DeleteCourse/
      │   └── Queries/
      │       ├── GetCourses/
      │       └── GetCourseById/
      ├── Roadmaps/
      │   ├── Commands/
      │   │   ├── CreateRoadmap/
      │   │   └── UpdateRoadmap/
      │   └── Queries/
      │       └── GetRoadmapByCourseId/
      ├── Enrollments/
      │   ├── Commands/
      │   │   └── EnrollInCourse/
      │   └── Queries/
      │       └── GetUserEnrollments/
      └── Progress/
      │   ├── Commands/
      │   │   └── UpdateProgress/
      │   └── Queries/
      │       └── GetUserProgress/
```

### 2.3 CQRS Pattern

Tüm işlemler **MediatR** ile Command/Query ayrımı yapılacak:
- **Commands**: Kurs oluşturma, güncelleme, kayıt, ilerleme güncelleme
- **Queries**: Kurs listeleme, detay görüntüleme, ilerleme sorgulama

---

## 3. Veri Modeli

### 3.1 MongoDB Collection Yapısı

**Collection'lar:**
1. **courses** - Kurs bilgileri (roadmap embedded)
2. **enrollments** - Kullanıcı kayıtları
3. **progresses** - İlerleme kayıtları

**Document Yapısı:**
```
Course Document (courses collection)
  ├── Roadmap (Embedded)
  │   └── Steps[] (Embedded)
  │       └── Contents[] (Embedded)
  
Enrollment Document (enrollments collection)
  ├── CourseId (Reference)
  └── UserId (Reference - PostgreSQL'den)

Progress Document (progress collection)
  ├── EnrollmentId (Reference)
  ├── ContentId (Reference - Course içinden)
  └── UserId (Reference - PostgreSQL'den)
```

### 3.2 Entity Detayları

#### 3.2.1 Course Document (courses collection)

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Course
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; }              // 3-200 karakter
    
    [BsonElement("description")]
    public string Description { get; set; }         // Max 2000 karakter
    
    [BsonElement("thumbnailUrl")]
    public string? ThumbnailUrl { get; set; }       // Opsiyonel görsel
    
    [BsonElement("createdBy")]
    public Guid CreatedBy { get; set; }            // Admin veya OrganizationManager (PostgreSQL UserId)
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
    
    [BsonElement("isActive")]
    public bool IsActive { get; set; }             // Aktif/Pasif durumu
    
    // Embedded Document
    [BsonElement("roadmap")]
    public CourseRoadmap? Roadmap { get; set; }
}

// Embedded Document - Roadmap
public class CourseRoadmap
{
    [BsonElement("title")]
    public string Title { get; set; }              // "C# Temelleri Yolu" gibi
    
    [BsonElement("description")]
    public string? Description { get; set; }       // Roadmap açıklaması
    
    [BsonElement("estimatedDurationMinutes")]
    public int EstimatedDurationMinutes { get; set; } // Tahmini süre (dakika)
    
    [BsonElement("steps")]
    public List<RoadmapStep> Steps { get; set; } = new();
}

// Embedded Document - Step
public class RoadmapStep
{
    [BsonElement("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [BsonElement("title")]
    public string Title { get; set; }             // "Adım 1: Temel Kavramlar"
    
    [BsonElement("description")]
    public string? Description { get; set; }       // Adım açıklaması
    
    [BsonElement("order")]
    public int Order { get; set; }                // Sıralama (1, 2, 3...)
    
    [BsonElement("isRequired")]
    public bool IsRequired { get; set; }          // Zorunlu mu?
    
    [BsonElement("estimatedDurationMinutes")]
    public int EstimatedDurationMinutes { get; set; }
    
    [BsonElement("contents")]
    public List<CourseContent> Contents { get; set; } = new();
}

// Embedded Document - Content
public enum ContentType
{
    Text = 1,           // Medium tarzı zengin metin
    VideoLink = 2,      // YouTube, Vimeo vb. video linki
    DocumentLink = 3,   // PDF, DOCX vb. doküman linki
    ExternalLink = 4    // Genel dış link
}

public class CourseContent
{
    [BsonElement("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [BsonElement("type")]
    public ContentType Type { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; }             // İçerik başlığı
    
    [BsonElement("content")]
    public string? Content { get; set; }          // Text için HTML/Markdown içerik
    
    [BsonElement("linkUrl")]
    public string? LinkUrl { get; set; }         // Video/Document/External için URL
    
    [BsonElement("embedUrl")]
    public string? EmbedUrl { get; set; }         // Frontend için embed URL
    
    [BsonElement("platform")]
    public string? Platform { get; set; }         // youtube, vimeo, vb.
    

    
    [BsonElement("order")]
    public int Order { get; set; }                // İçerik sıralaması
    
    [BsonElement("isRequired")]
    public bool IsRequired { get; set; }           // Zorunlu mu?
}
```

**İş Kuralları:**
- Title zorunlu, 3-200 karakter arası
- Description zorunlu, max 2000 karakter
- CreatedBy mutlaka Admin veya OrganizationManager rolüne sahip olmalı (PostgreSQL'den kontrol)
- Kurs silindiğinde (soft delete) IsActive=false yapılır, roadmap korunur
- Roadmap embedded document olarak saklanır (1:1 ilişki)
- Steps ve Contents nested array olarak saklanır

#### 3.2.2 Enrollment Document (enrollments collection)

```csharp
public class Enrollment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("userId")]
    public Guid UserId { get; set; }              // PostgreSQL UserId (Reference)
    
    [BsonElement("courseId")]
    public string CourseId { get; set; }         // MongoDB Course ObjectId (Reference)
    
    [BsonElement("enrolledAt")]
    public DateTime EnrolledAt { get; set; }
    
    [BsonElement("startedAt")]
    public DateTime? StartedAt { get; set; }     // İlk içerik erişimi
    
    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; set; }    // Kurs tamamlandığında
    
    [BsonElement("isActive")]
    public bool IsActive { get; set; }            // Aktif kayıt mı?
}

// Index: { userId: 1, courseId: 1 } - Unique
```

**İş Kuralları:**
- Bir kullanıcı aynı kursa sadece bir kez kayıt olabilir (unique index)
- Enrollment oluşturulduğunda StartedAt null olur
- İlk içerik erişiminde StartedAt set edilir
- Tüm zorunlu adımlar tamamlandığında CompletedAt set edilir

#### 3.2.3 Progress Document (progresses collection)

```csharp
public class Progress
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("userId")]
    public Guid UserId { get; set; }              // PostgreSQL UserId (Reference)
    
    [BsonElement("enrollmentId")]
    public string EnrollmentId { get; set; }       // MongoDB Enrollment ObjectId (Reference)
    
    [BsonElement("courseId")]
    public string CourseId { get; set; }          // MongoDB Course ObjectId (Reference)
    
    [BsonElement("stepId")]
    public string StepId { get; set; }             // RoadmapStep Id (embedded document içinden)
    
    [BsonElement("contentId")]
    public string ContentId { get; set; }         // CourseContent Id (embedded document içinden)
    
    [BsonElement("isCompleted")]
    public bool IsCompleted { get; set; }
    
    [BsonElement("completedAt")]
    public DateTime? CompletedAt { get; set; }
    
    [BsonElement("timeSpentMinutes")]
    public int? TimeSpentMinutes { get; set; }    // Opsiyonel süre takibi
    
    [BsonElement("lastAccessedAt")]
    public DateTime? LastAccessedAt { get; set; } // Son erişim zamanı
    
    [BsonElement("watchedPercentage")]
    public int? WatchedPercentage { get; set; }    // Video için izlenme yüzdesi
}

// Index: { userId: 1, contentId: 1, courseId: 1 } - Unique
```

**İş Kuralları:**
- UserId + ContentId + CourseId unique olmalı (bir kullanıcı bir içeriği bir kez tamamlar)
- IsCompleted=true olduğunda CompletedAt set edilir
- Progress kaydı oluşturulduğunda LastAccessedAt set edilir
- Her erişimde LastAccessedAt güncellenir

### 3.3 MongoDB Collection Yapısı ve İlişkiler

**Collection'lar:**
1. **courses** - Kurs bilgileri (roadmap embedded)
2. **enrollments** - Kullanıcı kayıtları (courseId reference)
3. **progresses** - İlerleme kayıtları (enrollmentId, courseId, contentId reference)

**İlişki Yapısı:**
```
Course Document (courses)
  └── Roadmap (Embedded)
      └── Steps[] (Embedded Array)
          └── Contents[] (Embedded Array)

Enrollment Document (enrollments)
  ├── courseId → Course._id (Reference)
  └── userId → PostgreSQL Users.Id (Reference)

Progress Document (progresses)
  ├── enrollmentId → Enrollment._id (Reference)
  ├── courseId → Course._id (Reference)
  ├── stepId → RoadmapStep.id (Embedded reference)
  ├── contentId → CourseContent.id (Embedded reference)
  └── userId → PostgreSQL Users.Id (Reference)
```

**Index'ler:**
```javascript
// enrollments collection
db.enrollments.createIndex({ userId: 1, courseId: 1 }, { unique: true });
db.enrollments.createIndex({ courseId: 1 });
db.enrollments.createIndex({ userId: 1, isActive: 1 });

// progresses collection
db.progresses.createIndex({ userId: 1, contentId: 1, courseId: 1 }, { unique: true });
db.progresses.createIndex({ enrollmentId: 1 });
db.progresses.createIndex({ courseId: 1, userId: 1 });

// courses collection
db.courses.createIndex({ isActive: 1, createdAt: -1 });
db.courses.createIndex({ "roadmap.steps.order": 1 }); // Step sıralaması için
```

---

## 4. Fonksiyonel Gereksinimler

### 4.1 Kurs Yönetimi

#### FR-EDU-001: Kurs Oluşturma
**Yetki:** Administrator, OrganizationManager  
**Açıklama:** Admin veya Kurum Müdürü yeni bir kurs oluşturabilir.

**Kabul Kriterleri:**
- ✅ Title (3-200 karakter) zorunlu
- ✅ Description (max 2000 karakter) zorunlu
- ✅ ThumbnailUrl opsiyonel (geçerli URL formatında)
- ✅ Kurs oluşturulduğunda IsActive=true olarak başlar
- ✅ CreatedBy bilgisi otomatik set edilir
- ✅ Kurs oluşturulduğunda CourseCreatedEvent domain event fırlatılır

#### FR-EDU-002: Kurs Güncelleme
**Yetki:** Administrator, OrganizationManager  
**Açıklama:** Mevcut kurs bilgileri güncellenebilir.

**Kabul Kriterleri:**
- ✅ Title, Description, ThumbnailUrl güncellenebilir
- ✅ UpdatedAt otomatik güncellenir
- ✅ Aktif kayıtları olan kurslar da güncellenebilir

#### FR-EDU-003: Kurs Silme (Soft Delete)
**Yetki:** Administrator  
**Açıklama:** Kurs silinir (soft delete), mevcut kayıtlar korunur.

**Kabul Kriterleri:**
- ✅ IsActive=false yapılır
- ✅ Mevcut enrollments ve progress kayıtları korunur
- ✅ Kullanıcılar artık yeni kayıt olamaz
- ✅ Mevcut kullanıcılar içeriklere erişmeye devam edebilir

#### FR-EDU-004: Kurs Listeleme
**Yetki:** Tüm kullanıcılar  
**Açıklama:** Tüm aktif kurslar listelenir.

**Kabul Kriterleri:**
- ✅ Sadece IsActive=true kurslar gösterilir
- ✅ Sayfalama desteği (pagination)
- ✅ Arama desteği (title, description üzerinde)
- ✅ Sıralama (CreatedAt, Title)
- ✅ Her kurs için: Title, Description, ThumbnailUrl, tahmini süre gösterilir

#### FR-EDU-005: Kurs Detay Görüntüleme
**Yetki:** Tüm kullanıcılar  
**Açıklama:** Kurs detayları ve roadmap görüntülenir.

**Kabul Kriterleri:**
- ✅ Kurs bilgileri (Title, Description, ThumbnailUrl)
- ✅ Roadmap bilgileri ve tüm adımlar
- ✅ Kullanıcının kayıt durumu (enrolled/not enrolled)
- ✅ Kullanıcının ilerleme durumu (varsa)

### 4.2 Roadmap Yönetimi

#### FR-EDU-006: Roadmap Oluşturma
**Yetki:** Administrator, OrganizationManager  
**Açıklama:** Kurs için roadmap oluşturulur.

**Kabul Kriterleri:**
- ✅ Her kurs için sadece bir roadmap olabilir
- ✅ Title zorunlu
- ✅ EstimatedDurationMinutes zorunlu (pozitif sayı)
- ✅ Roadmap oluşturulduğunda en az bir step eklenebilir
- ✅ Steps sıralı olmalı (Order: 1, 2, 3...)

#### FR-EDU-007: Roadmap Adımı Ekleme
**Yetki:** Administrator, OrganizationManager  
**Açıklama:** Roadmap'e yeni adım eklenir.

**Kabul Kriterleri:**
- ✅ Title zorunlu
- ✅ Order değeri benzersiz ve ardışık olmalı
- ✅ En az bir içerik (CourseContent) eklenmeli
- ✅ IsRequired varsayılan true olabilir

#### FR-EDU-008: İçerik Ekleme
**Yetki:** Administrator, OrganizationManager  
**Açıklama:** Roadmap adımına içerik eklenir.

**Kabul Kriterleri:**
- ✅ ContentType seçilmeli (Text, VideoLink, DocumentLink, ExternalLink)
- ✅ Type=Text ise Content zorunlu
- ✅ Type=VideoLink/DocumentLink/ExternalLink ise LinkUrl zorunlu
- ✅ LinkUrl geçerli URL formatında olmalı
- ✅ Order değeri step içinde benzersiz olmalı

### 4.3 Kayıt ve Erişim

#### FR-EDU-009: Kursa Kayıt Olma
**Yetki:** Tüm kullanıcılar  
**Açıklama:** Kullanıcı bir kursa kayıt olur.

**Kabul Kriterleri:**
- ✅ Kullanıcı aynı kursa sadece bir kez kayıt olabilir
- ✅ Enrollment oluşturulduğunda EnrolledAt set edilir
- ✅ StartedAt başlangıçta null olur
- ✅ İlk içerik erişiminde StartedAt set edilir
- ✅ EnrollmentCreatedEvent domain event fırlatılır

#### FR-EDU-010: İçerik Erişimi
**Yetki:** Kayıtlı kullanıcılar  
**Açıklama:** Kullanıcı kurs içeriğine erişir.

**Kabul Kriterleri:**
- ✅ Kullanıcı kursa kayıtlı olmalı
- ✅ İçerik erişiminde Progress kaydı oluşturulur/güncellenir
- ✅ LastAccessedAt güncellenir
- ✅ Zorunlu adımlar sırayla takip edilmeli (ön adım tamamlanmadan sonraki adıma geçilemez)

#### FR-EDU-011: İçerik Tamamlama
**Yetki:** Kayıtlı kullanıcılar  
**Açıklama:** Kullanıcı bir içeriği tamamlar.

**Kabul Kriterleri:**
- ✅ IsCompleted=true yapılır
- ✅ CompletedAt set edilir
- ✅ Zorunlu adımların tüm içerikleri tamamlandığında adım tamamlanır
- ✅ Tüm zorunlu adımlar tamamlandığında kurs tamamlanır (Enrollment.CompletedAt set edilir)

### 4.4 İlerleme Takibi

#### FR-EDU-012: Kullanıcı İlerleme Görüntüleme
**Yetki:** Kullanıcı kendi ilerlemesini görüntüleyebilir  
**Açıklama:** Kullanıcı kurs bazlı ilerlemesini görüntüler.

**Kabul Kriterleri:**
- ✅ Kurs bazında tamamlanma yüzdesi
- ✅ Adım bazında tamamlanma durumu
- ✅ İçerik bazında tamamlanma durumu
- ✅ Toplam geçirilen süre (TimeSpentMinutes toplamı)
- ✅ Tamamlanan adım sayısı / Toplam adım sayısı

#### FR-EDU-013: Genel İlerleme Dashboard
**Yetki:** Kullanıcı kendi dashboard'unu görüntüleyebilir  
**Açıklama:** Kullanıcı tüm kurslardaki ilerlemesini görüntüler.

**Kabul Kriterleri:**
- ✅ Kayıtlı olduğu tüm kurslar listelenir
- ✅ Her kurs için tamamlanma yüzdesi
- ✅ Aktif kurslar (devam eden)
- ✅ Tamamlanan kurslar
- ✅ Toplam tamamlanan kurs sayısı

---

## 5. Kullanıcı Senaryoları

### Senaryo 1: Admin Kurs Oluşturma ve Roadmap Hazırlama

**Aktör:** Admin (Ahmet)  
**Önkoşul:** Ahmet sisteme Admin rolü ile giriş yapmıştır.

**Akış:**
1. Ahmet "Eğitimler" menüsüne tıklar
2. "Yeni Kurs Oluştur" butonuna tıklar
3. Kurs bilgilerini girer:
   - Title: "C# Temelleri"
   - Description: "C# programlama dilinin temel kavramlarını öğrenin"
   - ThumbnailUrl: "https://example.com/csharp-thumbnail.jpg"
4. "Kaydet" butonuna tıklar
5. Sistem kursu oluşturur ve "Roadmap Oluştur" sayfasına yönlendirir
6. Ahmet roadmap bilgilerini girer:
   - Title: "C# Temelleri Yolu"
   - Description: "Sıfırdan C# öğrenmek için adım adım rehber"
   - EstimatedDurationMinutes: 480 (8 saat)
7. "Adım Ekle" butonuna tıklar
8. İlk adımı oluşturur:
   - Title: "Adım 1: C# Nedir?"
   - Description: "C# programlama diline giriş"
   - Order: 1
   - IsRequired: true
   - EstimatedDurationMinutes: 60
9. Bu adıma içerik ekler:
   - İçerik 1:
     - Type: Text
     - Title: "C# Nedir? - Giriş Yazısı"
     - Content: "<h1>C# Nedir?</h1><p>C# Microsoft tarafından...</p>"
     - Order: 1
   - İçerik 2:
     - Type: VideoLink
     - Title: "C# Tanıtım Videosu"
     - LinkUrl: "https://www.youtube.com/watch?v=..."
     - Order: 2
10. "Adım Ekle" ile ikinci adımı oluşturur:
    - Title: "Adım 2: Değişkenler ve Veri Tipleri"
    - Order: 2
    - İçerik ekler (Text + VideoLink)
11. Toplam 5 adım oluşturur
12. "Roadmap'i Kaydet" butonuna tıklar
13. Sistem roadmap'i kaydeder ve kurs aktif hale gelir

**Sonuç:** Kurs oluşturulur, roadmap hazırlanır ve kullanıcılar kayıt olabilir.

---

### Senaryo 2: Kullanıcı Kurs Keşfetme ve Kayıt Olma

**Aktör:** Kullanıcı (Ayşe)  
**Önkoşul:** Ayşe sisteme giriş yapmıştır (herhangi bir rol).

**Akış:**
1. Ayşe "Eğitimler" menüsüne tıklar
2. Sistem tüm aktif kursları listeler:
   - "C# Temelleri" (8 saat)
   - "JavaScript İleri Seviye" (12 saat)
   - "SQL Veritabanı Yönetimi" (6 saat)
3. Ayşe "C# Temelleri" kursuna tıklar
4. Kurs detay sayfası açılır:
   - Kurs açıklaması
   - Roadmap özeti (5 adım)
   - Tahmini süre: 8 saat
   - "Kayıt Ol" butonu görünür
5. Ayşe "Kayıt Ol" butonuna tıklar
6. Sistem kaydı oluşturur ve "Kursa Başla" sayfasına yönlendirir
7. Roadmap görüntülenir:
   - ✅ Adım 1: C# Nedir? (Başlamadı)
   - ⏳ Adım 2: Değişkenler ve Veri Tipleri (Kilitli)
   - ⏳ Adım 3: Kontrol Yapıları (Kilitli)
   - ...
8. Ayşe "Adım 1"e tıklar
9. İçerikler listelenir:
   - ✅ "C# Nedir? - Giriş Yazısı" (Text)
   - ⏳ "C# Tanıtım Videosu" (Video)
10. Ayşe ilk içeriğe tıklar ve okumaya başlar

**Sonuç:** Kullanıcı kursa kayıt olur ve içeriklere erişmeye başlar.

---

### Senaryo 3: Kullanıcı Adım Adım Öğrenme Yolculuğu

**Aktör:** Kullanıcı (Mehmet)  
**Önkoşul:** Mehmet "C# Temelleri" kursuna kayıtlıdır ve Adım 1'i tamamlamıştır.

**Akış:**
1. Mehmet "Eğitimlerim" sayfasına girer
2. "C# Temelleri" kursunu görür:
   - İlerleme: %20 (1/5 adım tamamlandı)
   - Aktif adım: Adım 2
3. "Devam Et" butonuna tıklar
4. Adım 2 içerikleri görüntülenir:
   - ✅ "Değişkenler Nedir?" (Text) - Tamamlandı
   - ⏳ "Değişkenler Örnek Videosu" (Video) - Devam ediyor
5. Mehmet video linkine tıklar, videoyu izler
6. Video izlendikten sonra "Tamamlandı Olarak İşaretle" butonuna tıklar
7. Sistem Progress kaydını günceller (IsCompleted=true)
8. Adım 2'nin tüm içerikleri tamamlandığı için adım tamamlanır
9. Adım 3 otomatik olarak açılır (kilit kalkar)
10. Mehmet Adım 3'e geçer ve içerikleri görüntüler
11. Adım 3'te bir Text içeriği ve bir DocumentLink içeriği vardır
12. Mehmet Text içeriğini okur, DocumentLink'e tıklar (PDF açılır)
13. PDF'i okuduktan sonra "Tamamlandı" olarak işaretler
14. Adım 3 tamamlanır, Adım 4 açılır
15. Bu şekilde tüm adımları tamamlar
16. Son adım tamamlandığında:
    - Kurs tamamlanır (%100)
    - Enrollment.CompletedAt set edilir
    - "Tebrikler! Kursu tamamladınız" mesajı gösterilir

**Sonuç:** Kullanıcı roadmap'i adım adım takip ederek kursu tamamlar.

---

### Senaryo 4: Kurum Müdürü Kurs Güncelleme

**Aktör:** Kurum Müdürü (Fatma)  
**Önkoşul:** Fatma OrganizationManager rolü ile giriş yapmıştır.

**Akış:**
1. Fatma "Eğitimler" menüsüne girer
2. Oluşturduğu kursları görür: "JavaScript İleri Seviye"
3. Kurs detayına girer ve "Düzenle" butonuna tıklar
4. Description'ı günceller: "ES6+ özellikleri ve modern JavaScript kavramları"
5. ThumbnailUrl'i günceller
6. "Kaydet" butonuna tıklar
7. Sistem kursu günceller (UpdatedAt set edilir)
8. Mevcut kayıtlı kullanıcılar etkilenmez, güncel bilgileri görürler

**Sonuç:** Kurs başarıyla güncellenir.

---

### Senaryo 5: Kullanıcı İlerleme Takibi

**Aktör:** Kullanıcı (Ali)  
**Önkoşul:** Ali birden fazla kursa kayıtlıdır.

**Akış:**
1. Ali "Eğitimlerim" sayfasına girer
2. Dashboard görüntülenir:
   - **Aktif Kurslar:**
     - C# Temelleri: %60 tamamlandı (3/5 adım)
     - SQL Veritabanı: %25 tamamlandı (1/4 adım)
   - **Tamamlanan Kurslar:**
     - JavaScript Temelleri: %100 (5/5 adım) - 2 gün önce tamamlandı
   - **İstatistikler:**
     - Toplam kurs: 3
     - Tamamlanan: 1
     - Devam eden: 2
     - Toplam geçirilen süre: 14 saat
3. Ali "C# Temelleri" kursuna tıklar
4. Detaylı ilerleme görüntülenir:
   - ✅ Adım 1: C# Nedir? (Tamamlandı - 1 saat)
   - ✅ Adım 2: Değişkenler (Tamamlandı - 2 saat)
   - ✅ Adım 3: Kontrol Yapıları (Tamamlandı - 1.5 saat)
   - ⏳ Adım 4: Diziler ve Koleksiyonlar (Devam ediyor - 0.5 saat)
   - 🔒 Adım 5: Nesne Yönelimli Programlama (Kilitli)
5. Ali Adım 4'teki içerikleri görüntüler:
   - ✅ Text içeriği tamamlandı
   - ⏳ Video içeriği devam ediyor
6. Video'yu tamamlar ve kursa devam eder

**Sonuç:** Kullanıcı detaylı ilerleme bilgilerini görüntüler.

---

### Senaryo 6: Zorunlu Adım Kontrolü

**Aktör:** Kullanıcı (Zeynep)  
**Önkoşul:** Zeynep bir kursa kayıtlıdır ve Adım 1'i tamamlamıştır.

**Akış:**
1. Zeynep Adım 2'deki içerikleri görüntüler
2. Adım 2'de 3 içerik var:
   - Text içeriği (IsRequired: true)
   - Video içeriği (IsRequired: true)
   - Ekstra kaynak (IsRequired: false)
3. Zeynep sadece Text içeriğini okur ve tamamlar
4. Video içeriğini atlamaya çalışır
5. Adım 2'nin tüm zorunlu içerikleri tamamlanmadığı için adım tamamlanmaz
6. Adım 3 kilitli kalır
7. Zeynep Video içeriğini de tamamlar
8. Artık Adım 3 açılır ve erişilebilir hale gelir

**Sonuç:** Zorunlu içerikler tamamlanmadan sonraki adıma geçilemez.

---

## 6. Yetkilendirme ve Roller

### 6.1 Rol Bazlı Yetkiler

#### Administrator (Admin)
- ✅ `Course.Create` - Kurs oluşturma
- ✅ `Course.Update` - Kurs güncelleme
- ✅ `Course.Delete` - Kurs silme
- ✅ `Course.View` - Kurs görüntüleme
- ✅ `Roadmap.Create` - Roadmap oluşturma
- ✅ `Roadmap.Update` - Roadmap güncelleme
- ✅ `Roadmap.Delete` - Roadmap silme
- ✅ `Content.Create` - İçerik ekleme
- ✅ `Content.Update` - İçerik güncelleme
- ✅ `Content.Delete` - İçerik silme

#### OrganizationManager (Kurum Müdürü)
- ✅ `Course.Create` - Kurs oluşturma
- ✅ `Course.Update` - Kurs güncelleme
- ❌ `Course.Delete` - Kurs silme (YOK)
- ✅ `Course.View` - Kurs görüntüleme
- ✅ `Roadmap.Create` - Roadmap oluşturma
- ✅ `Roadmap.Update` - Roadmap güncelleme
- ❌ `Roadmap.Delete` - Roadmap silme (YOK)
- ✅ `Content.Create` - İçerik ekleme
- ✅ `Content.Update` - İçerik güncelleme
- ❌ `Content.Delete` - İçerik silme (YOK)

#### Tüm Kullanıcılar (Employee, TeamLead, Trainer, Customer, Trial)
- ✅ `Course.View` - Kurs görüntüleme
- ✅ `Enrollment.Create` - Kursa kayıt olma
- ✅ `Content.Access` - İçerik erişimi
- ✅ `Progress.View` - Kendi ilerlemesini görüntüleme
- ✅ `Progress.Update` - İçerik tamamlama işaretleme

### 6.2 Permission Constants

```csharp
public static class Permissions
{
    public static class Education
    {
        public const string CourseCreate = "Education.Course.Create";
        public const string CourseUpdate = "Education.Course.Update";
        public const string CourseDelete = "Education.Course.Delete";
        public const string CourseView = "Education.Course.View";
        
        public const string RoadmapCreate = "Education.Roadmap.Create";
        public const string RoadmapUpdate = "Education.Roadmap.Update";
        public const string RoadmapDelete = "Education.Roadmap.Delete";
        public const string RoadmapView = "Education.Roadmap.View";
        
        public const string ContentCreate = "Education.Content.Create";
        public const string ContentUpdate = "Education.Content.Update";
        public const string ContentDelete = "Education.Content.Delete";
        public const string ContentAccess = "Education.Content.Access";
        
        public const string EnrollmentCreate = "Education.Enrollment.Create";
        public const string EnrollmentView = "Education.Enrollment.View";
        
        public const string ProgressView = "Education.Progress.View";
        public const string ProgressUpdate = "Education.Progress.Update";
    }
}
```

---

## 7. API Tasarımı

Bu bölüm, mevcut API endpoint'lerinin **kesin** tanımlarını içerir. Tüm endpoint'ler `api/v1/education` base path'i altındadır.

---

### 7.1 Courses (Kurslar) - CoursesController

Base Route: `/api/v1/education/courses`

---

#### 7.1.1 GET /api/v1/education/courses
**Açıklama:** Tüm aktif kursları sayfalı olarak listeler  
**Yetki:** `[Authorize]` - Tüm authenticated kullanıcılar  
**Query Parameters:**
- `page` (int, default: 1) - Sayfa numarası
- `pageSize` (int, default: 10) - Sayfa başına kayıt
- `search` (string?, optional) - Title veya description üzerinde arama

**Response Type:** `PagedResult<CourseListItemResponse>`
**Status Codes:**
- `200 OK` - Başarılı
- `400 Bad Request` - Geçersiz parametreler

**Response Örneği:**
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "C# Temelleri",
      "description": "C# programlama dilinin temel kavramları",
      "thumbnailUrl": "https://example.com/thumb.jpg",
      "estimatedDurationMinutes": 480,
      "stepCount": 5,
      "enrollmentCount": 150,
      "createdAt": "2025-01-15T10:00:00Z"
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

**CourseListItemResponse Schema:**
```typescript
{
  id: Guid,
  title: string,
  description: string,
  thumbnailUrl?: string,
  estimatedDurationMinutes?: number,
  stepCount?: number,
  enrollmentCount: number,
  createdAt: DateTime
}
```

---

#### 7.1.2 GET /api/v1/education/courses/{id}
**Açıklama:** Kurs detaylarını getirir (roadmap ve kullanıcının enrollment durumu dahil)  
**Yetki:** `[Authorize(Policy = "CourseView")]`  
**Path Parameters:**
- `id` (Guid) - Kurs ID'si

**Response Type:** `CourseResponse`
**Status Codes:**
- `200 OK` - Başarılı
- `404 Not Found` - Kurs bulunamadı

**Response Örneği:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "C# Temelleri",
  "description": "C# programlama dilinin temel kavramları",
  "thumbnailUrl": "https://example.com/thumb.jpg",
  "estimatedDurationMinutes": 480,
  "stepCount": 5,
  "enrollmentCount": 150,
  "createdAt": "2025-01-15T10:00:00Z",
  "roadmap": {
    "title": "C# Öğrenme Yolu",
    "description": "Adım adım C# rehberi",
    "estimatedDurationMinutes": 480,
    "steps": [
      {
        "id": "step-guid-1",
        "title": "Adım 1: Giriş",
        "description": "C# nedir?",
        "order": 1,
        "isRequired": true,
        "estimatedDurationMinutes": 60,
        "contents": [
          {
            "id": "content-guid-1",
            "title": "Video: C# Tanıtım",
            "description": null,
            "type": 2,
            "order": 1,
            "isRequired": true,
            "content": null,
            "linkUrl": "https://youtube.com/watch?v=...",
            "embedUrl": "https://youtube.com/embed/...",
            "platform": "youtube"
          }
        ]
      }
    ]
  },
  "userEnrollment": {
    "id": "enrollment-guid",
    "enrolledAt": "2025-01-20T10:00:00Z",
    "startedAt": "2025-01-20T10:05:00Z",
    "completedAt": null,
    "isActive": true
  },
  "progressPercentage": 45.5
}
```

**CourseResponse Schema:**
```typescript
{
  id: Guid,
  title: string,
  description: string,
  thumbnailUrl?: string,
  estimatedDurationMinutes?: number,
  stepCount: number,
  enrollmentCount: number,
  createdAt: DateTime,
  roadmap?: RoadmapResponse,
  userEnrollment?: EnrollmentResponse,
  progressPercentage: decimal
}
```

---

#### 7.1.3 POST /api/v1/education/courses
**Açıklama:** Yeni kurs oluşturur  
**Yetki:** `[Authorize(Policy = "CourseCreate")]` - Administrator, OrganizationManager

**Request Body:** `CreateCourseCommand`
```json
{
  "title": "C# İleri Seviye",
  "description": "İleri seviye C# konuları",
  "thumbnailUrl": "https://example.com/advanced-csharp.jpg"
}
```

**CreateCourseCommand Schema:**
```typescript
{
  title: string,          // 3-200 karakter
  description: string,    // Max 2000 karakter
  thumbnailUrl?: string   // Geçerli URL
}
```

**Response Type:** `CreateCourseCommandResponse`
**Status Codes:**
- `201 Created` - Başarılı, Location header ile `/courses/{id}` döner
- `400 Bad Request` - Validation hatası
- `403 Forbidden` - Yetki yok

**Response:**
```json
{
  "id": "new-course-guid"
}
```

---

#### 7.1.4 PUT /api/v1/education/courses/{id}
**Açıklama:** Kursu günceller  
**Yetki:** `[Authorize(Policy = "CourseUpdate")]`

**Path Parameters:**
- `id` (Guid) - Kurs ID'si

**Request Body:** `UpdateCourseRequest`
```json
{
  "title": "C# Temelleri (Güncellenmiş)",
  "description": "Yeni açıklama",
  "thumbnailUrl": "https://example.com/new-thumb.jpg"
}
```

**Response Type:** `204 No Content` (başarılı)
**Status Codes:**
- `204 No Content` - Başarılı
- `400 Bad Request` - Validation hatası
- `404 Not Found` - Kurs bulunamadı

---

#### 7.1.5 DELETE /api/v1/education/courses/{id}
**Açıklama:** Kursu siler (Soft Delete - IsActive=false)  
**Yetki:** `[Authorize(Policy = "CourseDelete")]` - Sadece Administrator

**Path Parameters:**
- `id` (Guid) - Kurs ID'si

**Response Type:** `204 No Content`
**Status Codes:**
- `204 No Content` - Başarılı
- `404 Not Found` - Kurs bulunamadı

---

### 7.2 Roadmaps - RoadmapsController

Base Route: `/api/v1/education/courses/{courseId}`

---

#### 7.2.1 GET /api/v1/education/courses/{courseId}/roadmap
**Açıklama:** Kursun roadmap'ini getirir  
**Yetki:** `[Authorize(Policy = "RoadmapView")]`

**Path Parameters:**
- `courseId` (Guid)

**Response Type:** `RoadmapResponse`
**Status Codes:**
- `200 OK`
- `404 Not Found`

**Response Örneği:**
```json
{
  "title": "C# Öğrenme Yolu",
  "description": "Sıfırdan ileri seviyeye C#",
  "estimatedDurationMinutes": 480,
  "steps": [
    {
      "id": "step-guid",
      "title": "Adım 1: Başlangıç",
      "description": "C# nedir?",
      "order": 1,
      "isRequired": true,
      "estimatedDurationMinutes": 60,
      "contents": [...]
    }
  ]
}
```

---

#### 7.2.2 GET /api/v1/education/courses/{courseId}/roadmap/steps
**Açıklama:** Sadece roadmap adımlarını döner (Optimizasyon için)  
**Yetki:** `[Authorize(Policy = "RoadmapView")]`

**Response Type:** `IReadOnlyList<StepResponse>`

---

#### 7.2.3 POST /api/v1/education/courses/{courseId}/roadmap
**Açıklama:** Kurs için roadmap oluşturur  
**Yetki:** `[Authorize(Policy = "RoadmapCreate")]`

**Request Body:** `CreateRoadmapCommand`
```json
{
  "courseId": "course-guid",  // Path'ten gelir, body'de override edilebilir
  "title": "Roadmap Başlığı",
  "description": "Açıklama",
  "estimatedDurationMinutes": 480,
  "steps": [
    {
      "title": "Adım 1",
      "description": "...",
      "order": 1,
      "isRequired": true,
      "estimatedDurationMinutes": 60,
      "contents": [
        {
          "type": 2,  // ContentType enum: 1=Text, 2=VideoLink, 3=DocumentLink, 4=ExternalLink
          "title": "Video",
          "description": null,
          "order": 1,
          "isRequired": true,
          "content": null,
          "linkUrl": "https://youtube.com/watch?v=..."
        }
      ]
    }
  ]
}
```

**CreateRoadmapCommand Schema:**
```typescript
{
  courseId: Guid,
  title: string,
  description?: string,
  estimatedDurationMinutes: number,
  steps: StepDto[]
}

StepDto {
  title: string,
  description?: string,
  order: number,
  isRequired: boolean,
  estimatedDurationMinutes: number,
  contents: ContentDto[]
}

ContentDto {
  type: ContentType (1|2|3|4),
  title: string,
  description?: string,
  order: number,
  isRequired: boolean,
  content?: string,    // Type=1 (Text) için zorunlu
  linkUrl?: string     // Type=2,3,4 için zorunlu
}
```

**Response Type:** `CreateRoadmapCommandResponse`
**Status Codes:**
- `201 Created`
- `400 Bad Request`
- `403 Forbidden`
- `404 Not Found` - Kurs bulunamadı

---

#### 7.2.4 PUT /api/v1/education/courses/{courseId}/roadmap
**Açıklama:** Roadmap'i günceller  
**Yetki:** `[Authorize(Policy = "RoadmapUpdate")]`

**Request Body:** `UpdateRoadmapCommand` (CreateRoadmapCommand ile aynı yapı)

**Response Type:** `204 No Content`
**Status Codes:**
- `204 No Content`
- `400 Bad Request`
- `404 Not Found`

---

### 7.3 Enrollments (Kayıtlar) - EnrollmentsController

Base Route: `/api/v1/education`

---

#### 7.3.1 POST /api/v1/education/courses/{courseId}/enroll
**Açıklama:** Kullanıcıyı kursa kaydeder  
**Yetki:** `[Authorize]` - Tüm kullanıcılar

**Path Parameters:**
- `courseId` (Guid)

**Request Body:** Yok (courseId path'ten alınır, userId auth'tan)

**Response Type:** `EnrollInCourseCommandResponse`
**Status Codes:**
- `201 Created`
- `400 Bad Request` - Zaten kayıtlı veya kurs bulunamadı
- `404 Not Found`

**Response:**
```json
{
  "enrollmentId": "new-enrollment-guid",
  "enrolledAt": "2025-01-27T10:00:00Z"
}
```

---

#### 7.3.2 GET /api/v1/education/enrollments
**Açıklama:** Kullanıcının kurs kayıtlarını listeler  
**Yetki:** `[Authorize]`

**Query Parameters:**
- `status` (string?, default: "all") - "active", "completed", "all"

**Response Type:** `IReadOnlyList<EnrollmentListItemResponse>`
**Status Codes:**
- `200 OK`
- `400 Bad Request`

**Response:**
```json
[
  {
    "id": "enrollment-guid",
    "courseId": "course-guid",
    "courseTitle": "C# Temelleri",
    "courseDescription": "...",
    "thumbnailUrl": "...",
    "enrolledAt": "2025-01-20T10:00:00Z",
    "startedAt": "2025-01-20T10:05:00Z",
    "completedAt": null,
    "isActive": true,
    "progressPercentage": 45.5
  }
]
```

**EnrollmentListItemResponse Schema:**
```typescript
{
  id: Guid,
  courseId: Guid,
  courseTitle: string,
  courseDescription: string,
  thumbnailUrl?: string,
  enrolledAt: DateTime,
  startedAt?: DateTime,
  completedAt?: DateTime,
  isActive: boolean,
  progressPercentage: decimal
}
```

---

### 7.4 Progress (İlerleme) - ProgressController

Base Route: `/api/v1/education`

---

#### 7.4.1 GET /api/v1/education/courses/{courseId}/progress
**Açıklama:** Kullanıcının belirtilen kurstaki detaylı ilerleme bilgisi  
**Yetki:** `[Authorize(Policy = "ProgressView")]`

**Path Parameters:**
- `courseId` (Guid)

**Response Type:** `CourseProgressResponse`
**Status Codes:**
- `200 OK`
- `404 Not Found` - Enrollment bulunamadı

**Response:**
```json
{
  "courseId": "course-guid",
  "courseTitle": "C# Temelleri",
  "progressPercentage": 60.0,
  "steps": [
    {
      "stepId": "step-guid",
      "title": "Adım 1",
      "order": 1,
      "progressPercentage": 100.0,
      "completedContentCount": 2,
      "totalContentCount": 2,
      "contents": [
        {
          "contentId": "content-guid",
          "title": "Video 1",
          "isCompleted": true,
          "completedAt": "2025-01-20T10:30:00Z",
          "timeSpentMinutes": 15,
          "watchedPercentage": 100
        }
      ]
    }
  ]
}
```

**CourseProgressResponse Schema:**
```typescript
{
  courseId: Guid,
  courseTitle: string,
  progressPercentage: decimal,
  steps: StepProgressItem[]
}

StepProgressItem {
  stepId: Guid,
  title: string,
  order: number,
  progressPercentage: decimal,
  completedContentCount: number,
  totalContentCount: number,
  contents: ContentProgressItem[]
}

ContentProgressItem {
  contentId: Guid,
  title: string,
  isCompleted: boolean,
  completedAt?: DateTime,
  timeSpentMinutes?: number,
  watchedPercentage?: number
}
```

---

#### 7.4.2 GET /api/v1/education/progress/dashboard
**Açıklama:** Kullanıcının genel eğitim ilerleme özeti  
**Yetki:** `[Authorize(Policy = "ProgressView")]`

**Response Type:** `ProgressDashboardResponse`
**Status Codes:**
- `200 OK`

**Response:**
```json
{
  "totalCourses": 3,
  "activeCourses": 2,
  "completedCourses": 1,
  "totalTimeSpentMinutes": 450,
  "enrollments": [
    {
      "enrollmentId": "enrollment-guid",
      "courseId": "course-guid",
      "progressPercentage": 45.5,
      "enrolledAt": "2025-01-20T10:00:00Z",
      "startedAt": "2025-01-20T10:05:00Z",
      "completedAt": null
    }
  ]
}
```

---

#### 7.4.3 POST /api/v1/education/contents/{contentId}/complete
**Açıklama:** İçeriği tamamlandı olarak işaretler  
**Yetki:** `[Authorize(Policy = "ProgressUpdate")]`

**Path Parameters:**
- `contentId` (Guid)

**Request Body:** `CompleteContentRequest`
```json
{
  "courseId": "course-guid",
  "timeSpentMinutes": 15,
  "watchedPercentage": 100
}
```

**CompleteContentRequest Schema:**
```typescript
{
  courseId: Guid,
  timeSpentMinutes?: number,
  watchedPercentage?: number
}
```

**Response Type:** `CompleteContentCommandResponse`
**Status Codes:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

**Response:**
```json
{
  "progressId": "progress-guid",
  "isStepCompleted": true,
  "isCourseCompleted": false
}
```

---

#### 7.4.4 POST /api/v1/education/contents/{contentId}/video-progress
**Açıklama:** Video izleme ilerlemesini kaydeder (Opsiyonel, real-time tracking)  
**Yetki:** `[Authorize(Policy = "ProgressUpdate")]`

**Path Parameters:**
- `contentId` (Guid)

**Request Body:** `UpdateVideoProgressRequest`
```json
{
  "courseId": "course-guid",
  "watchedPercentage": 75,
  "timeSpentSeconds": 450,
  "isCompleted": false
}
```

**UpdateVideoProgressRequest Schema:**
```typescript
{
  courseId: Guid,
  watchedPercentage: number,  // 0-100
  timeSpentSeconds: number,
  isCompleted: boolean        // default: false
}
```

**Response Type:** `UpdateVideoProgressCommandResponse`
**Status Codes:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

**Response:**
```json
{
  "progressId": "progress-guid"
}
```

**Not:** Bu endpoint her 10-30 saniyede bir çağrılabilir. `isCompleted: true` gönderildiğinde içerik tamamlanmış sayılır.

---

## 8. Roadmap Yapısı

### 8.1 Roadmap Kavramı

Roadmap, bir kursun **adım adım öğrenme yolunu** tanımlar. Her roadmap:
- **Sıralı adımlardan** (RoadmapStep) oluşur
- Her adım **bir veya daha fazla içerik** (CourseContent) içerir
- Adımlar **sırayla takip edilir** (ön adım tamamlanmadan sonraki adıma geçilemez)
- Adımlar **zorunlu veya opsiyonel** olabilir

### 8.2 Adım Sıralaması ve Kilitleme

**Kilit Mekanizması:**
- Kullanıcı bir adıma erişmek istediğinde:
  1. Önceki tüm zorunlu adımlar tamamlanmış olmalı
  2. Önceki adımların tüm zorunlu içerikleri tamamlanmış olmalı
- Eğer önceki adım tamamlanmamışsa, sonraki adım **kilitli** (locked) durumda kalır
- Kilitli adımlar görüntülenebilir ancak içeriklerine erişilemez

**Örnek Senaryo:**
```
Adım 1 (Zorunlu) → Adım 2 (Zorunlu) → Adım 3 (Opsiyonel) → Adım 4 (Zorunlu)

Durum 1: Adım 1 tamamlandı, Adım 2 açık ✅
Durum 2: Adım 1 tamamlanmadı, Adım 2 kilitli 🔒
Durum 3: Adım 2 tamamlandı, Adım 3 açık ✅ (opsiyonel ama önceki adım tamamlandığı için açık)
Durum 4: Adım 3 atlanabilir, Adım 4'e geçilebilir ✅
```

### 8.3 İçerik Sıralaması

Bir adım içindeki içerikler de sıralıdır ancak **kilitlenmez**. Kullanıcı:
- İçerikleri istediği sırayla görüntüleyebilir
- Ancak **zorunlu içerikler** tamamlanmadan adım tamamlanmaz
- Opsiyonel içerikler atlanabilir

---

## 9. İçerik Tipleri

### 9.1 Text (Zengin Metin)

**Kullanım:** Medium tarzı makale/yazı içerikleri  
**Format:** HTML veya Markdown  
**Özellikler:**
- Başlıklar, paragraflar, listeler
- Görsel ekleme (img tag)
- Kod blokları
- Linkler

**Örnek:**
```json
{
  "type": 1,
  "title": "C# Nedir? - Giriş Yazısı",
  "content": "<h1>C# Nedir?</h1><p>C# (C-Sharp), Microsoft tarafından geliştirilen...</p><pre><code>Console.WriteLine(\"Hello World\");</code></pre>",
  "order": 1,
  "isRequired": true
}
```

### 9.2 VideoLink (Video Linki)

**Kullanım:** YouTube, Vimeo, vb. video platformlarından içerik  
**Format:** URL  
**Özellikler:**
- Link doğrulama (geçerli URL kontrolü)
- Video platformu tespiti (opsiyonel, UI için)
- **Embed gösterimi**: Web ve mobilde video player ile gösterilir
- Video tamamlanma takibi (opsiyonel)

**Desteklenen Platformlar:**
- ✅ YouTube (`youtube.com`, `youtu.be`)
- ✅ Vimeo (`vimeo.com`)
- ✅ Diğer OEmbed destekleyen platformlar

**Örnek:**
```json
{
  "type": 2,
  "title": "C# Tanıtım Videosu",
  "linkUrl": "https://www.youtube.com/watch?v=abc123",
  "order": 2,
  "isRequired": true
}
```

#### 9.2.1 Video Gösterim Teknik Detayları

**Web (Frontend) Implementasyonu:**

**Önerilen Kütüphane:** `react-player` (YouTube, Vimeo ve diğer platformları destekler)

```typescript
import ReactPlayer from 'react-player';

// Video component örneği
<ReactPlayer
  url={content.linkUrl}
  width="100%"
  height="400px"
  controls={true}
  onProgress={(progress) => {
    // İlerleme takibi (opsiyonel)
    if (progress.playedSeconds > 0) {
      // Video izleniyor
    }
  }}
  onEnded={() => {
    // Video tamamlandığında otomatik tamamla (opsiyonel)
    handleVideoCompleted(content.id);
  }}
/>
```

**Responsive Tasarım:**
- Desktop: 16:9 aspect ratio, max-width: 1200px
- Tablet: 16:9 aspect ratio, full-width
- Mobile: 16:9 aspect ratio, full-width, touch-friendly controls

**Video Tamamlanma Takibi:**
- Kullanıcı video izlerken ilerleme kaydedilebilir
- Video %80+ izlendiğinde otomatik tamamlanmış sayılabilir (opsiyonel)
- Veya kullanıcı manuel olarak "Tamamlandı" butonuna tıklayabilir

**Mobil (Android) Implementasyonu:**

**Önerilen Kütüphane:** `ExoPlayer` (Google'ın resmi video player'ı)

```kotlin
// ExoPlayer ile video gösterimi
val player = ExoPlayer.Builder(context).build()
val playerView = PlayerView(context)
playerView.player = player

// YouTube URL'i için özel işleme gerekebilir
// YouTube IFrame API veya YouTube Data API kullanılabilir
// Alternatif: WebView içinde embed gösterimi

// WebView ile embed gösterimi (daha kolay)
webView.settings.javaScriptEnabled = true
webView.loadUrl("https://www.youtube.com/embed/VIDEO_ID")
```

**Mobil Video Gösterim Stratejileri:**

1. **WebView ile Embed (Önerilen):**
   - YouTube/Vimeo embed URL'lerini WebView'de göster
   - Daha kolay implementasyon
   - Platform native player'ları kullanır

2. **ExoPlayer ile Direct Stream:**
   - Daha fazla kontrol
   - Offline caching mümkün
   - Daha karmaşık implementasyon

**Video URL Dönüştürme:**

Backend'de video linklerini parse edip embed URL'lerine dönüştüren bir helper method:

```csharp
public static class VideoUrlHelper
{
    public static string? GetEmbedUrl(string originalUrl)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            return null;

        // YouTube
        if (originalUrl.Contains("youtube.com/watch?v="))
        {
            var videoId = ExtractYouTubeVideoId(originalUrl);
            return $"https://www.youtube.com/embed/{videoId}";
        }
        
        if (originalUrl.Contains("youtu.be/"))
        {
            var videoId = originalUrl.Split('/').Last().Split('?').First();
            return $"https://www.youtube.com/embed/{videoId}";
        }

        // Vimeo
        if (originalUrl.Contains("vimeo.com/"))
        {
            var videoId = originalUrl.Split('/').Last().Split('?').First();
            return $"https://player.vimeo.com/video/{videoId}";
        }

        return originalUrl; // Desteklenmeyen platformlar için orijinal URL
    }
}
```

**API Response'a Embed URL Ekleme:**

API response'unda hem orijinal link hem de embed URL'i döndürülebilir:

```json
{
  "id": "guid",
  "type": 2,
  "title": "C# Tanıtım Videosu",
  "linkUrl": "https://www.youtube.com/watch?v=abc123",
  "embedUrl": "https://www.youtube.com/embed/abc123",  // Frontend için
  "platform": "youtube",  // Platform bilgisi (UI için)
  "order": 2,
  "isRequired": true
}
```

**Video Tamamlanma Endpoint'i:**

Video içerikleri için özel bir endpoint:

```http
POST /api/v1/education/contents/{contentId}/video-progress
Content-Type: application/json

{
  "watchedPercentage": 85,  // İzlenen yüzde (0-100)
  "timeSpentSeconds": 1200,  // Geçirilen süre (saniye)
  "isCompleted": true        // Kullanıcı manuel tamamladı mı?
}
```

**Güvenlik ve Performans:**

- ✅ Video linklerinin doğrulanması (geçerli URL kontrolü)
- ✅ HTTPS zorunluluğu
- ✅ CORS ayarları (embed için)
- ✅ Lazy loading (video sadece görüntülendiğinde yüklenir)
- ✅ Video thumbnail gösterimi (yüklemeden önce)

### 9.3 DocumentLink (Doküman Linki)

**Kullanım:** PDF, DOCX, PPTX vb. dokümanlar  
**Format:** URL  
**Özellikler:**
- Dosya tipi tespiti (opsiyonel)
- İndirme veya görüntüleme
- Güvenlik: Sadece güvenli kaynaklardan (HTTPS)

**Örnek:**
```json
{
  "type": 3,
  "title": "C# Temel Kavramlar PDF",
  "linkUrl": "https://example.com/docs/csharp-basics.pdf",
  "order": 3,
  "isRequired": false
}
```

### 9.4 ExternalLink (Dış Link)

**Kullanım:** Genel web kaynakları, blog yazıları, dokümantasyon  
**Format:** URL  
**Özellikler:**
- Herhangi bir web sitesi
- Güvenlik uyarısı (opsiyonel)

**Örnek:**
```json
{
  "type": 4,
  "title": "Microsoft C# Dokümantasyonu",
  "linkUrl": "https://learn.microsoft.com/dotnet/csharp/",
  "order": 4,
  "isRequired": false
}
```

---

## 10. İlerleme Takibi

### 10.1 İlerleme Hesaplama

**Kurs İlerleme Yüzdesi:**
```
Tamamlanan Zorunlu Adımlar / Toplam Zorunlu Adımlar * 100
```

**Adım Tamamlanma Kriteri:**
- Adımdaki **tüm zorunlu içerikler** tamamlanmış olmalı
- Opsiyonel içerikler tamamlanmasa da adım tamamlanabilir

**İçerik Tamamlanma:**
- Kullanıcı "Tamamlandı" butonuna tıkladığında
- Progress kaydı oluşturulur/güncellenir
- IsCompleted=true, CompletedAt set edilir

### 10.2 Süre Takibi

**TimeSpentMinutes:**
- Kullanıcı içeriği tamamladığında opsiyonel olarak süre girebilir
- Veya sistem otomatik hesaplayabilir (LastAccessedAt - StartedAt)
- Toplam süre: Tüm içeriklerin TimeSpentMinutes toplamı

### 10.3 Kurs Tamamlanma

**Kurs Tamamlanma Kriteri:**
- Tüm zorunlu adımlar tamamlanmış olmalı
- Enrollment.CompletedAt set edilir
- Kullanıcıya tamamlanma bildirimi gösterilir

---

## 11. Teknik Detaylar

### 11.1 Domain Events

```csharp
// Course oluşturulduğunda
public record CourseCreatedEvent(Guid CourseId, Guid CreatedBy, string Title) : IDomainEvent;

// Kullanıcı kursa kayıt olduğunda
public record EnrollmentCreatedEvent(Guid EnrollmentId, Guid UserId, Guid CourseId) : IDomainEvent;

// İçerik tamamlandığında
public record ContentCompletedEvent(Guid ProgressId, Guid UserId, Guid ContentId) : IDomainEvent;

// Adım tamamlandığında
public record StepCompletedEvent(Guid UserId, Guid StepId, Guid CourseId) : IDomainEvent;

// Kurs tamamlandığında
public record CourseCompletedEvent(Guid EnrollmentId, Guid UserId, Guid CourseId) : IDomainEvent;
```

### 11.2 Validation Rules

**Course:**
- Title: 3-200 karakter, zorunlu
- Description: Max 2000 karakter, zorunlu
- ThumbnailUrl: Geçerli URL formatı, opsiyonel

**Roadmap:**
- Title: 3-200 karakter, zorunlu
- EstimatedDurationMinutes: Pozitif sayı, zorunlu
- Steps: En az 1 adım, zorunlu

**RoadmapStep:**
- Title: 3-200 karakter, zorunlu
- Order: Pozitif sayı, benzersiz (roadmap içinde)
- Contents: En az 1 içerik, zorunlu

**CourseContent:**
- Title: 3-200 karakter, zorunlu
- Type=Text: Content zorunlu
- Type=VideoLink/DocumentLink/ExternalLink: LinkUrl zorunlu, geçerli URL
- Order: Pozitif sayı, benzersiz (step içinde)

### 11.3 MongoDB Repository Pattern

**MongoDB.Driver Kullanımı:**

```csharp
using MongoDB.Driver;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdWithRoadmapAsync(string id, CancellationToken cancellationToken = default);
    Task<PagedResult<Course>> GetActiveCoursesAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<string> CreateAsync(Course course, CancellationToken cancellationToken = default);
    Task UpdateAsync(Course course, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

public class CourseRepository : ICourseRepository
{
    private readonly IMongoCollection<Course> _collection;

    public CourseRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Course>("courses");
    }

    public async Task<Course?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Course?> GetByIdWithRoadmapAsync(string id, CancellationToken cancellationToken = default)
    {
        // Roadmap zaten embedded olduğu için aynı sorgu
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<PagedResult<Course>> GetActiveCoursesAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<Course>.Filter;
        var filter = filterBuilder.Eq(c => c.IsActive, true);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchFilter = filterBuilder.Or(
                filterBuilder.Regex(c => c.Title, new BsonRegularExpression(search, "i")),
                filterBuilder.Regex(c => c.Description, new BsonRegularExpression(search, "i"))
            );
            filter = filterBuilder.And(filter, searchFilter);
        }

        var sort = Builders<Course>.Sort.Descending(c => c.CreatedAt);
        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var courses = await _collection
            .Find(filter)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Course>
        {
            Items = courses,
            TotalCount = (int)totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<string> CreateAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(course, cancellationToken: cancellationToken);
        return course.Id;
    }

    public async Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, course.Id);
        course.UpdatedAt = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(filter, course, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, id);
        return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Course>.Filter.Eq(c => c.Id, id);
        var update = Builders<Course>.Update.Set(c => c.IsActive, false).Set(c => c.UpdatedAt, DateTime.UtcNow);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, string courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolledAsync(Guid userId, string courseId, CancellationToken cancellationToken = default);
    Task<string> CreateAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
}

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly IMongoCollection<Enrollment> _collection;

    public EnrollmentRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Enrollment>("enrollments");
    }

    public async Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, string courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.And(
            Builders<Enrollment>.Filter.Eq(e => e.UserId, userId),
            Builders<Enrollment>.Filter.Eq(e => e.CourseId, courseId)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.UserId, userId);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEnrolledAsync(Guid userId, string courseId, CancellationToken cancellationToken = default)
    {
        var enrollment = await GetByUserAndCourseAsync(userId, courseId, cancellationToken);
        return enrollment != null && enrollment.IsActive;
    }

    public async Task<string> CreateAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(enrollment, cancellationToken: cancellationToken);
        return enrollment.Id;
    }

    public async Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Enrollment>.Filter.Eq(e => e.Id, enrollment.Id);
        await _collection.ReplaceOneAsync(filter, enrollment, cancellationToken: cancellationToken);
    }
}

public interface IProgressRepository
{
    Task<Progress?> GetByUserAndContentAsync(Guid userId, string contentId, string courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Progress>> GetUserProgressByCourseAsync(Guid userId, string courseId, CancellationToken cancellationToken = default);
    Task<int> GetCompletedContentCountAsync(Guid userId, string stepId, string courseId, CancellationToken cancellationToken = default);
    Task<string> CreateOrUpdateAsync(Progress progress, CancellationToken cancellationToken = default);
}

public class ProgressRepository : IProgressRepository
{
    private readonly IMongoCollection<Progress> _collection;

    public ProgressRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Progress>("progresses");
    }

    public async Task<Progress?> GetByUserAndContentAsync(Guid userId, string contentId, string courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, userId),
            Builders<Progress>.Filter.Eq(p => p.ContentId, contentId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, courseId)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Progress>> GetUserProgressByCourseAsync(Guid userId, string courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, userId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, courseId)
        );
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<int> GetCompletedContentCountAsync(Guid userId, string stepId, string courseId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, userId),
            Builders<Progress>.Filter.Eq(p => p.StepId, stepId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, courseId),
            Builders<Progress>.Filter.Eq(p => p.IsCompleted, true)
        );
        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<string> CreateOrUpdateAsync(Progress progress, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Progress>.Filter.And(
            Builders<Progress>.Filter.Eq(p => p.UserId, progress.UserId),
            Builders<Progress>.Filter.Eq(p => p.ContentId, progress.ContentId),
            Builders<Progress>.Filter.Eq(p => p.CourseId, progress.CourseId)
        );

        var options = new ReplaceOptions { IsUpsert = true };
        await _collection.ReplaceOneAsync(filter, progress, options, cancellationToken);
        return progress.Id;
    }
}
```

**MongoDB Connection ve Dependency Injection:**

```csharp
// Startup/Program.cs
services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = configuration.GetConnectionString("MongoDB");
    return new MongoClient(connectionString);
});

services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = configuration["MongoDB:DatabaseName"] ?? "te4it_education";
    return client.GetDatabase(databaseName);
});

services.AddScoped<ICourseRepository, CourseRepository>();
services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
services.AddScoped<IProgressRepository, ProgressRepository>();
```

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDB": {
    "DatabaseName": "te4it_education"
  }
}
```

### 11.4 Business Rules Service

```csharp
public interface ICourseProgressService
{
    Task<bool> CanAccessStepAsync(Guid userId, Guid stepId, CancellationToken cancellationToken = default);
    Task<bool> IsStepCompletedAsync(Guid userId, Guid stepId, CancellationToken cancellationToken = default);
    Task<bool> IsCourseCompletedAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<int> CalculateProgressPercentageAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<Guid?> GetNextUnlockedStepIdAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
}
```

### 11.5 Video URL Helper Service

```csharp
public interface IVideoUrlService
{
    /// <summary>
    /// Video URL'ini embed URL'ine dönüştürür
    /// </summary>
    string? GetEmbedUrl(string originalUrl);
    
    /// <summary>
    /// Video platformunu tespit eder (youtube, vimeo, unknown)
    /// </summary>
    string? DetectPlatform(string url);
    
    /// <summary>
    /// Video URL'inin geçerli olup olmadığını kontrol eder
    /// </summary>
    bool IsValidVideoUrl(string url);
}

public class VideoUrlService : IVideoUrlService
{
    public string? GetEmbedUrl(string originalUrl)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            return null;

        // YouTube: https://www.youtube.com/watch?v=VIDEO_ID
        if (originalUrl.Contains("youtube.com/watch?v="))
        {
            var videoId = ExtractYouTubeVideoId(originalUrl);
            return $"https://www.youtube.com/embed/{videoId}";
        }
        
        // YouTube Short: https://youtu.be/VIDEO_ID
        if (originalUrl.Contains("youtu.be/"))
        {
            var videoId = originalUrl.Split('/').Last().Split('?').First();
            return $"https://www.youtube.com/embed/{videoId}";
        }

        // Vimeo: https://vimeo.com/VIDEO_ID
        if (originalUrl.Contains("vimeo.com/"))
        {
            var videoId = originalUrl.Split('/').Last().Split('?').First();
            return $"https://player.vimeo.com/video/{videoId}";
        }

        return originalUrl; // Desteklenmeyen platformlar için orijinal URL
    }

    public string? DetectPlatform(string url)
    {
        if (url.Contains("youtube.com") || url.Contains("youtu.be"))
            return "youtube";
        
        if (url.Contains("vimeo.com"))
            return "vimeo";
        
        return "unknown";
    }

    public bool IsValidVideoUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        if (uri.Scheme != "https")
            return false;

        return DetectPlatform(url) != "unknown";
    }

    private string ExtractYouTubeVideoId(string url)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["v"] ?? string.Empty;
    }
}
```

**CourseContent DTO'ya Embed URL Ekleme:**

```csharp
public class CourseContentDto
{
    public Guid Id { get; set; }
    public ContentType Type { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public string? LinkUrl { get; set; }
    public string? EmbedUrl { get; set; }  // YENİ: Frontend için embed URL
    public string? Platform { get; set; }  // YENİ: Platform bilgisi (youtube, vimeo)
    public int Order { get; set; }
    public bool IsRequired { get; set; }
}
```

**Handler'da Embed URL Set Etme:**

```csharp
public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IVideoUrlService _videoUrlService;

    public async Task<CourseDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdWithRoadmapAsync(request.CourseId, cancellationToken);
        
        // Roadmap içindeki video içeriklerine embed URL ekle
        foreach (var step in course.Roadmap.Steps)
        {
            foreach (var content in step.Contents)
            {
                if (content.Type == ContentType.VideoLink && !string.IsNullOrEmpty(content.LinkUrl))
                {
                    content.EmbedUrl = _videoUrlService.GetEmbedUrl(content.LinkUrl);
                    content.Platform = _videoUrlService.DetectPlatform(content.LinkUrl);
                }
            }
        }
        
        return course;
    }
}
```

### 11.6 Authorization Policies

```csharp
// Startup/Program.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("CourseCreate", policy => 
        policy.RequireClaim(Permissions.Education.CourseCreate));
    
    options.AddPolicy("CourseUpdate", policy => 
        policy.RequireClaim(Permissions.Education.CourseUpdate));
    
    options.AddPolicy("CourseDelete", policy => 
        policy.RequireClaim(Permissions.Education.CourseDelete));
    
    options.AddPolicy("ContentAccess", policy => 
        policy.RequireClaim(Permissions.Education.ContentAccess));
});
```

### 11.7 Frontend/Mobile Video Implementasyon Notları

#### Web (React) Implementasyonu

**Gerekli Paketler:**
```bash
npm install react-player
```

**Video Component Örneği:**
```typescript
import ReactPlayer from 'react-player';
import { useState } from 'react';

interface VideoContentProps {
  content: {
    id: string;
    title: string;
    linkUrl: string;
    embedUrl?: string;  // Backend'den gelecek
    platform?: string;  // Backend'den gelecek
    isRequired: boolean;
  };
  onComplete: (contentId: string) => void;
}

export const VideoContent: React.FC<VideoContentProps> = ({ content, onComplete }) => {
  const [watchedPercentage, setWatchedPercentage] = useState(0);
  const [isCompleted, setIsCompleted] = useState(false);

  const handleProgress = (progress: { playedSeconds: number; played: number }) => {
    const percentage = Math.round(progress.played * 100);
    setWatchedPercentage(percentage);

    // %80+ izlendiğinde otomatik tamamla (opsiyonel)
    if (percentage >= 80 && !isCompleted) {
      handleComplete();
    }
  };

  const handleComplete = async () => {
    setIsCompleted(true);
    await fetch(`/api/v1/education/contents/${content.id}/complete`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        timeSpentMinutes: Math.round(watchedPercentage / 100 * 10) // Tahmini süre
      })
    });
    onComplete(content.id);
  };

  return (
    <div className="video-content">
      <h3>{content.title}</h3>
      <div className="video-wrapper" style={{ position: 'relative', paddingTop: '56.25%' }}>
        <ReactPlayer
          url={content.embedUrl || content.linkUrl}
          width="100%"
          height="100%"
          style={{ position: 'absolute', top: 0, left: 0 }}
          controls={true}
          onProgress={handleProgress}
          onEnded={handleComplete}
        />
      </div>
      <div className="video-progress">
        <p>İzlenme: %{watchedPercentage}</p>
        {!isCompleted && (
          <button onClick={handleComplete}>Tamamlandı Olarak İşaretle</button>
        )}
      </div>
    </div>
  );
};
```

**Responsive CSS:**
```css
.video-wrapper {
  position: relative;
  padding-top: 56.25%; /* 16:9 aspect ratio */
  height: 0;
  overflow: hidden;
}

.video-wrapper > * {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

@media (max-width: 768px) {
  .video-wrapper {
    padding-top: 56.25%; /* Mobile'da da aynı oran */
  }
}
```

#### Mobile (Android/Kotlin) Implementasyonu

**WebView ile Embed Gösterimi (Önerilen):**

```kotlin
import android.webkit.WebView
import android.webkit.WebViewClient

class VideoContentFragment : Fragment() {
    private lateinit var webView: WebView
    
    fun displayVideo(content: CourseContent) {
        webView.settings.apply {
            javaScriptEnabled = true
            domStorageEnabled = true
            mediaPlaybackRequiresUserGesture = false
        }
        
        webView.webViewClient = object : WebViewClient() {
            override fun onPageFinished(view: WebView?, url: String?) {
                super.onPageFinished(view, url)
                // Video yüklendi
            }
        }
        
        // Embed URL'i kullan (backend'den gelecek)
        val embedUrl = content.embedUrl ?: convertToEmbedUrl(content.linkUrl)
        webView.loadUrl(embedUrl)
    }
    
    private fun convertToEmbedUrl(originalUrl: String): String {
        // YouTube
        if (originalUrl.contains("youtube.com/watch?v=")) {
            val videoId = extractYouTubeVideoId(originalUrl)
            return "https://www.youtube.com/embed/$videoId"
        }
        // Vimeo
        if (originalUrl.contains("vimeo.com/")) {
            val videoId = originalUrl.split("/").last().split("?").first()
            return "https://player.vimeo.com/video/$videoId"
        }
        return originalUrl
    }
}
```

**ExoPlayer ile Direct Stream (Alternatif):**

```kotlin
import androidx.media3.exoplayer.ExoPlayer
import androidx.media3.ui.PlayerView

class VideoPlayerFragment : Fragment() {
    private var player: ExoPlayer? = null
    
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        
        player = ExoPlayer.Builder(requireContext()).build()
        val playerView = view.findViewById<PlayerView>(R.id.player_view)
        playerView.player = player
        
        // Video URL'i set et
        val videoUrl = content.embedUrl ?: content.linkUrl
        val mediaItem = MediaItem.fromUri(videoUrl)
        player?.setMediaItem(mediaItem)
        player?.prepare()
        player?.play()
    }
    
    override fun onDestroy() {
        super.onDestroy()
        player?.release()
    }
}
```

**Video Tamamlanma Tracking:**

```kotlin
class VideoProgressTracker {
    private var startTime: Long = 0
    private var watchedDuration: Long = 0
    
    fun onVideoStarted() {
        startTime = System.currentTimeMillis()
    }
    
    fun onVideoProgress(currentPosition: Long, duration: Long) {
        watchedDuration = currentPosition
        val percentage = (currentPosition * 100 / duration).toInt()
        
        // %80+ izlendiğinde otomatik tamamla
        if (percentage >= 80) {
            markAsCompleted()
        }
    }
    
    private fun markAsCompleted() {
        val timeSpentMinutes = ((System.currentTimeMillis() - startTime) / 60000).toInt()
        
        // API çağrısı
        apiService.completeContent(contentId, timeSpentMinutes)
    }
}
```

#### Video Gösterim Best Practices

**Performans:**
- ✅ Lazy loading: Video sadece görüntülendiğinde yüklenir
- ✅ Thumbnail gösterimi: Video yüklenmeden önce thumbnail göster
- ✅ Caching: Video metadata'sı cache'lenebilir

**Kullanıcı Deneyimi:**
- ✅ Responsive tasarım: Tüm ekran boyutlarında düzgün görünüm
- ✅ Touch-friendly controls: Mobilde dokunmatik kontroller
- ✅ Progress indicator: İzlenme yüzdesi gösterimi
- ✅ Auto-play kontrolü: Kullanıcı tercihine göre

**Güvenlik:**
- ✅ HTTPS zorunluluğu: Tüm video linkleri HTTPS olmalı
- ✅ URL validation: Backend'de video URL'leri doğrulanmalı
- ✅ CORS ayarları: Embed için gerekli CORS headers

**Video Tamamlanma Stratejileri:**

1. **Otomatik Tamamlama (%80+ izlendi):**
   - Kullanıcı video'nun %80'ini izlediğinde otomatik tamamlanır
   - Daha iyi kullanıcı deneyimi
   - Ancak kullanıcı kontrolü azalır

2. **Manuel Tamamlama:**
   - Kullanıcı "Tamamlandı" butonuna tıklar
   - Daha fazla kontrol
   - Ancak kullanıcı unutabilir

3. **Hibrit Yaklaşım (Önerilen):**
   - %80+ izlendiğinde otomatik tamamlanır
   - Kullanıcı isterse manuel olarak da tamamlayabilir
   - En iyi kullanıcı deneyimi

---

## 12. Geliştirme Adımları

### Faz 1: Temel Yapı ve MongoDB Kurulumu (1-2 Hafta)
1. ✅ MongoDB.Driver NuGet paketi ekleme
2. ✅ MongoDB connection string yapılandırması
3. ✅ Domain entities oluşturma (MongoDB document yapısı)
4. ✅ MongoDB collection setup ve index'ler
5. ✅ Repository interfaces ve MongoDB implementations
6. ✅ Permission constants ekleme
7. ✅ MongoDB dependency injection yapılandırması

### Faz 2: Kurs Yönetimi (1 Hafta)
1. ✅ CreateCourse command/handler
2. ✅ UpdateCourse command/handler
3. ✅ DeleteCourse command/handler
4. ✅ GetCourses query/handler
5. ✅ GetCourseById query/handler
6. ✅ CourseController

### Faz 3: Roadmap Yönetimi (1 Hafta)
1. ✅ CreateRoadmap command/handler
2. ✅ UpdateRoadmap command/handler
3. ✅ GetRoadmapByCourseId query/handler
4. ✅ RoadmapController

### Faz 4: Kayıt ve İlerleme (1 Hafta)
1. ✅ EnrollInCourse command/handler
2. ✅ GetUserEnrollments query/handler
3. ✅ UpdateProgress command/handler
4. ✅ GetUserProgress query/handler
5. ✅ EnrollmentController, ProgressController

### Faz 5: Business Logic (1 Hafta)
1. ✅ ICourseProgressService implementation
2. ✅ Adım kilitleme mantığı
3. ✅ İlerleme hesaplama
4. ✅ Kurs tamamlanma kontrolü

### Faz 6: Test ve Dokümantasyon (1 Hafta)
1. ✅ Unit testler (MongoDB mock'ları ile)
2. ✅ Integration testler (MongoDB test container veya in-memory)
3. ✅ API dokümantasyonu (Swagger)
4. ✅ Frontend entegrasyon rehberi
5. ✅ MongoDB collection yapısı dokümantasyonu

### MongoDB Collection Setup Script

```javascript
// MongoDB setup script (mongo shell veya MongoDB Compass)
use te4it_education;

// courses collection index'leri
db.courses.createIndex({ isActive: 1, createdAt: -1 });
db.courses.createIndex({ "roadmap.steps.order": 1 });

// enrollments collection index'leri
db.enrollments.createIndex({ userId: 1, courseId: 1 }, { unique: true });
db.enrollments.createIndex({ courseId: 1 });
db.enrollments.createIndex({ userId: 1, isActive: 1 });

// progresses collection index'leri
db.progresses.createIndex({ userId: 1, contentId: 1, courseId: 1 }, { unique: true });
db.progresses.createIndex({ enrollmentId: 1 });
db.progresses.createIndex({ courseId: 1, userId: 1 });
db.progresses.createIndex({ courseId: 1, stepId: 1 });
```

**C# ile Index Oluşturma:**

```csharp
public class MongoIndexService
{
    private readonly IMongoDatabase _database;

    public MongoIndexService(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task CreateIndexesAsync()
    {
        // Courses indexes
        var coursesCollection = _database.GetCollection<Course>("courses");
        await coursesCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Course>(Builders<Course>.IndexKeys
                .Ascending(c => c.IsActive)
                .Descending(c => c.CreatedAt)),
            new CreateIndexModel<Course>(Builders<Course>.IndexKeys
                .Ascending("roadmap.steps.order"))
        });

        // Enrollments indexes
        var enrollmentsCollection = _database.GetCollection<Enrollment>("enrollments");
        await enrollmentsCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Enrollment>(Builders<Enrollment>.IndexKeys
                .Ascending(e => e.UserId)
                .Ascending(e => e.CourseId), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<Enrollment>(Builders<Enrollment>.IndexKeys
                .Ascending(e => e.CourseId)),
            new CreateIndexModel<Enrollment>(Builders<Enrollment>.IndexKeys
                .Ascending(e => e.UserId)
                .Ascending(e => e.IsActive))
        });

        // Progress indexes
        var progressCollection = _database.GetCollection<Progress>("progress");
        await progressCollection.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Progress>(Builders<Progress>.IndexKeys
                .Ascending(p => p.UserId)
                .Ascending(p => p.ContentId)
                .Ascending(p => p.CourseId), new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<Progress>(Builders<Progress>.IndexKeys
                .Ascending(p => p.EnrollmentId)),
            new CreateIndexModel<Progress>(Builders<Progress>.IndexKeys
                .Ascending(p => p.CourseId)
                .Ascending(p => p.UserId)),
            new CreateIndexModel<Progress>(Builders<Progress>.IndexKeys
                .Ascending(p => p.CourseId)
                .Ascending(p => p.StepId))
        });
    }
}
```

---

## 13. Örnek Veri Yapısı

### Örnek Kurs: "C# Temelleri"

```json
{
  "course": {
    "id": "course-guid-1",
    "title": "C# Temelleri",
    "description": "C# programlama dilinin temel kavramlarını öğrenin",
    "thumbnailUrl": "https://example.com/csharp.jpg",
    "createdBy": "admin-guid",
    "createdAt": "2025-01-15T10:00:00Z",
    "isActive": true
  },
  "roadmap": {
    "id": "roadmap-guid-1",
    "title": "C# Temelleri Yolu",
    "estimatedDurationMinutes": 480,
    "steps": [
      {
        "id": "step-guid-1",
        "title": "Adım 1: C# Nedir?",
        "order": 1,
        "isRequired": true,
        "estimatedDurationMinutes": 60,
        "contents": [
          {
            "id": "content-guid-1",
            "type": 1,
            "title": "C# Nedir? - Giriş Yazısı",
            "content": "<h1>C# Nedir?</h1><p>C# Microsoft tarafından...</p>",
            "order": 1,
            "isRequired": true
          },
          {
            "id": "content-guid-2",
            "type": 2,
            "title": "C# Tanıtım Videosu",
            "linkUrl": "https://www.youtube.com/watch?v=abc123",
            "order": 2,
            "isRequired": true
          }
        ]
      },
      {
        "id": "step-guid-2",
        "title": "Adım 2: Değişkenler ve Veri Tipleri",
        "order": 2,
        "isRequired": true,
        "estimatedDurationMinutes": 90,
        "contents": [
          {
            "id": "content-guid-3",
            "type": 1,
            "title": "Değişkenler Nedir?",
            "content": "<h1>Değişkenler</h1><p>...</p>",
            "order": 1,
            "isRequired": true
          },
          {
            "id": "content-guid-4",
            "type": 2,
            "title": "Değişkenler Örnek Videosu",
            "linkUrl": "https://www.youtube.com/watch?v=def456",
            "order": 2,
            "isRequired": true
          },
          {
            "id": "content-guid-5",
            "type": 3,
            "title": "Veri Tipleri Referans Dokümanı",
            "linkUrl": "https://example.com/docs/data-types.pdf",
            "order": 3,
            "isRequired": false
          }
        ]
      }
    ]
  }
}
```

---

## 14. Sorular ve Cevaplar

### S1: Public/Private kurs ayrımı yapılacak mı?
**Cevap:** Hayır, şu an için tüm kurslar herkese açık. Gelecekte `IsPublic` property'si eklenebilir.

### S2: Bir kursa birden fazla roadmap eklenebilir mi?
**Cevap:** Hayır, her kurs için sadece bir roadmap olabilir (1:1 ilişki).

### S3: Roadmap adımları silinebilir mi?
**Cevap:** Evet, Admin ve Kurum Müdürü adımları silebilir. Ancak aktif kayıtları olan adımların silinmesi durumunda kullanıcılar etkilenir, bu yüzden dikkatli olunmalı.

### S4: İçerikler düzenlenebilir mi?
**Cevap:** Evet, Admin ve Kurum Müdürü içerikleri güncelleyebilir. Mevcut kullanıcılar güncel içeriği görür.

### S5: Kurs tamamlandıktan sonra tekrar alınabilir mi?
**Cevap:** Şu an için bir kursa sadece bir kez kayıt olunabilir. Gelecekte "Yeniden Başlat" özelliği eklenebilir.

### S6: Video ve dokümanlar sistem içinde mi saklanacak?
**Cevap:** Hayır, sadece link olarak saklanacak. Dosya yükleme özelliği gelecekte eklenebilir.

### S10: Neden MongoDB kullanılıyor?
**Cevap:** 
- Farklı veritabanları ile çalışma deneyimi (PostgreSQL + MongoDB hibrit yaklaşım)
- Eğitim içeriklerinin esnek şema yapısına uygunluk
- Embedded documents ile roadmap yapısının doğal temsili
- NoSQL yaklaşımı ile dinamik içerik yönetimi

### S11: PostgreSQL ile MongoDB arasında veri senkronizasyonu nasıl yapılacak?
**Cevap:**
- **User bilgileri:** PostgreSQL'de kalır, MongoDB'de sadece UserId (Guid) reference olarak saklanır
- **Kurs verileri:** Tamamen MongoDB'de saklanır
- **İlişkiler:** MongoDB document'lerinde PostgreSQL UserId'leri Guid olarak saklanır
- **Senkronizasyon:** Gerekli durumlarda (kullanıcı silme vb.) MongoDB'deki referanslar kontrol edilir

### S7: Videolar mobilde ve web'de nasıl gösterilecek?
**Cevap:** 
- **Web:** `react-player` kütüphanesi kullanılacak. Backend'den gelen `embedUrl` ile video embed edilecek.
- **Mobil:** WebView ile embed gösterimi yapılacak (önerilen) veya ExoPlayer kullanılabilir.
- Backend video URL'lerini otomatik olarak embed URL'lerine dönüştürecek (`VideoUrlService`).
- API response'unda hem `linkUrl` hem de `embedUrl` döndürülecek.

### S8: Video tamamlanma takibi nasıl yapılacak?
**Cevap:** 
- **Otomatik:** Video %80+ izlendiğinde otomatik tamamlanır (opsiyonel).
- **Manuel:** Kullanıcı "Tamamlandı" butonuna tıklayabilir.
- **Hibrit (Önerilen):** %80+ izlendiğinde otomatik tamamlanır, kullanıcı isterse manuel de tamamlayabilir.
- Video izlenirken ilerleme kaydedilebilir (`/video-progress` endpoint'i ile).

### S9: Hangi video platformları desteklenecek?
**Cevap:** 
- ✅ YouTube (`youtube.com`, `youtu.be`)
- ✅ Vimeo (`vimeo.com`)
- ✅ Diğer OEmbed destekleyen platformlar
- Desteklenmeyen platformlar için orijinal URL gösterilir.

---

## 15. Sonuç

Bu doküman, TE4IT Eğitim Modülü'nün kapsamlı bir spesifikasyonunu içermektedir. Modül, **roadmap tabanlı öğrenme** yaklaşımı ile kullanıcıların sistematik bir şekilde eğitim içeriklerini tamamlamasını sağlar.

**Temel Özellikler:**
- ✅ Admin ve Kurum Müdürü kurs oluşturabilir
- ✅ Çoklu içerik desteği (Text, Video, Document, External Link)
- ✅ Adım adım roadmap takibi
- ✅ Detaylı ilerleme takibi
- ✅ Zorunlu/opsiyonel içerik ve adım yönetimi

**Geliştirme Süresi:** Yaklaşık 6-7 hafta (tahmini)

**Sonraki Adımlar:**
1. Domain entities oluşturma
2. Database migrations
3. Repository implementations
4. CQRS handlers
5. API controllers
6. Business logic services
7. Testing

---

**Doküman Versiyonu:** 1.0  
**Son Güncelleme:** 2025-01-27  
**Hazırlayan:** TE4IT Development Team

