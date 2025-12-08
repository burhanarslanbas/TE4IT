# 📦 Module Management - Kapsamlı Analiz Dokümanı

## 📋 İçindekiler
1. [Genel Bakış](#genel-bakış)
2. [Domain Modeli](#domain-modeli)
3. [Kullanıcı Senaryoları](#kullanıcı-senaryoları)
4. [Authorization Matrix](#authorization-matrix)
5. [API Endpoints](#api-endpoints)
6. [Business Rules](#business-rules)
7. [Edge Cases](#edge-cases)
8. [Kod Analizi](#kod-analizi)

---

## 🎯 Genel Bakış

### Module Nedir?
**Module**, bir projeyi mantıksal bölümlere ayırmak için kullanılan yapı taşıdır.
- Her module bir **Project**'e aittir
- Module içinde birden fazla **UseCase** bulunur
- Module başlangıç tarihi ve aktiflik durumu vardır

### Temel Özellikler
- ✅ Proje bazında modül oluşturma
- ✅ Modül güncelleme (Title, Description)
- ✅ Modül durumu değiştirme (Active/Archived)
- ✅ Modül silme (Hard delete)
- ✅ Modül listeleme (Pagination, Search, Filter by IsActive)
- ✅ Modül detay görüntüleme

---

## 🏗️ Domain Modeli

### Module Entity

```csharp
public class Module : AggregateRoot
{
    public Guid ProjectId { get; private set; }        // Hangi projeye ait?
    public UserId CreatorId { get; private set; }      // Kim oluşturdu?
    public string Title { get; private set; }          // Modül başlığı (zorunlu)
    public string? Description { get; private set; }   // Modül açıklaması (opsiyonel)
    public DateTime StartedDate { get; private set; }  // Başlangıç tarihi (otomatik: UtcNow)
    public bool IsActive { get; private set; }         // Aktif mi? (default: true)

    // Domain Methods
    public static Module Create(Guid projectId, UserId creatorId, string title, string? description);
    public void UpdateTitle(string title);
    public void UpdateDescription(string? description);
    public void Activate();    // IsActive = true
    public void Archive();     // IsActive = false
}
```

### İlişkiler
```
Project (1) ──────< (N) Module
                        │
                        └──< (N) UseCase
                                  │
                                  └──< (N) Task
```

**Cascade Davranışı:**
- ⚠️ **Proje silinirse:** Module'ler ne olur? (Kontrol edilmeli)
- ⚠️ **Module silinirse:** UseCase'ler ne olur? (Cascade delete mi?)

---

## 👤 Kullanıcı Senaryoları

### Scenario 1: Module Oluşturma

**Başlık:** Kullanıcı yeni bir module oluşturur

**Preconditions:**
- Kullanıcı sisteme giriş yapmış
- Kullanıcı proje üyesi (en az **Member** veya **Owner**)
- Proje **aktif** durumda

**Flow:**
1. Kullanıcı "Modules" sayfasında "Create Module" butonuna tıklar
2. Form açılır: Title (zorunlu), Description (opsiyonel)
3. Kullanıcı bilgileri doldurur ve "Create" butonuna basar
4. Sistem validasyon yapar:
   - Title boş mu?
   - Kullanıcının projede modül oluşturma yetkisi var mı? (`CanCreateModule`)
   - Proje aktif mi?
5. Module oluşturulur:
   - `CreatorId = currentUser.Id`
   - `StartedDate = DateTime.UtcNow`
   - `IsActive = true`
6. Success mesajı gösterilir ve modül listesine yönlendirilir

**Postconditions:**
- Yeni module veritabanına kaydedildi
- Module listesinde görünür

**Edge Cases:**
- ❌ **Viewer rolündeki kullanıcı oluşturmaya çalışırsa?** → 403 Forbidden
- ❌ **Arşivlenmiş projede oluşturmaya çalışırsa?** → `BusinessRuleViolationException`
- ❌ **Proje üyesi olmayan biri oluşturmaya çalışırsa?** → `ProjectAccessDeniedException`
- ✅ **Administrator oluşturabilir mi?** → EVET (her projede)

---

### Scenario 2: Module Listeleme

**Başlık:** Kullanıcı projeye ait modülleri listeler

**Preconditions:**
- Kullanıcı sisteme giriş yapmış
- Kullanıcı proje üyesi (Viewer, Member veya Owner)

**Flow:**
1. Kullanıcı proje detay sayfasından "Modules" sekmesine tıklar
2. Sistem filtreleme seçeneklerini gösterir:
   - Search (Title'da arama)
   - IsActive (null = hepsi, true = aktifler, false = arşivler)
   - Pagination (Page, PageSize)
3. Sistem modülleri listeler:
   - `Id, Title, IsActive, StartedDate, UseCaseCount`
4. Her modül için UseCase sayısı gösterilir (N+1 problemi var mı?)

**Postconditions:**
- Kullanıcı modülleri görür ve filtreleyebilir

**Edge Cases:**
- ❌ **Proje üyesi olmayan biri listeye erişirse?** → `ProjectAccessDeniedException`
- ✅ **Viewer rolü listeye erişebilir mi?** → EVET (sadece görüntüleme)
- ✅ **Administrator tüm projelerin modüllerini görebilir mi?** → EVET
- ⚠️ **N+1 Query Problem:** Her module için `CountByModuleAsync` çağrılıyor

---

### Scenario 3: Module Detay Görüntüleme

**Başlık:** Kullanıcı modül detayını görüntüler

**Preconditions:**
- Kullanıcı sisteme giriş yapmış
- Kullanıcı proje üyesi (Viewer, Member, Owner)

**Flow:**
1. Kullanıcı modül listesinden bir modüle tıklar
2. Sistem yetki kontrolü yapar (`CanAccessProject`)
3. Modül detayları gösterilir:
   - Title, Description, IsActive, StartedDate, ProjectId
4. İlgili UseCase listesi gösterilebilir (başka endpoint)

**Postconditions:**
- Kullanıcı modül bilgilerini görür

**Edge Cases:**
- ❌ **Proje üyesi olmayan biri erişirse?** → `ProjectAccessDeniedException`
- ❌ **Modül bulunamazsa?** → `ResourceNotFoundException`

---

### Scenario 4: Module Güncelleme

**Başlık:** Kullanıcı modül bilgilerini günceller

**Preconditions:**
- Kullanıcı sisteme giriş yapmış
- Kullanıcı **Owner** veya **Member** rolüne sahip
- Proje **aktif** durumda

**Flow:**
1. Kullanıcı modül detay sayfasında "Edit" butonuna tıklar
2. Form açılır: Title, Description
3. Kullanıcı değişiklikleri yapar ve "Update" butonuna basar
4. Sistem validasyon yapar:
   - Title boş mu?
   - Kullanıcının düzenleme yetkisi var mı? (`CanEditProject`)
   - Proje aktif mi?
5. Module güncellenir:
   - `UpdateTitle()` ve `UpdateDescription()` çağrılır
   - `UpdatedDate = DateTime.UtcNow`
6. Success mesajı gösterilir

**Postconditions:**
- Module güncellenmiş durumda

**Edge Cases:**
- ❌ **Viewer rolündeki kullanıcı güncellerse?** → 403 Forbidden
- ❌ **Arşivlenmiş projede güncellerse?** → `BusinessRuleViolationException`
- ✅ **Administrator güncelleyebilir mi?** → EVET

---

### Scenario 5: Module Durum Değiştirme (Archive/Activate)

**Başlık:** Kullanıcı modülü aktif/arşiv yapar

**Preconditions:**
- Kullanıcı sisteme giriş yapmış
- Kullanıcı **Owner** veya **Member** rolüne sahip

**Flow:**
1. Kullanıcı modül listesinde "Archive" veya "Activate" butonuna tıklar
2. Onay modalı açılır (opsiyonel)
3. Sistem yetki kontrolü yapar (`CanEditProject`)
4. Modül durumu değiştirilir:
   - `module.Activate()` veya `module.Archive()`
5. Success mesajı gösterilir

**Postconditions:**
- Module durumu değişmiş
- Listede `IsActive` flag'i güncellenmiş

**Edge Cases:**
- ❌ **Viewer rolündeki kullanıcı değiştirirse?** → 403 Forbidden
- ⚠️ **Arşivlenen modül içindeki UseCase'ler ne olur?** → İş kuralı gerekli
- ⚠️ **Arşivlenen modüle yeni UseCase eklenebilir mi?** → Muhtemelen hayır

---

### Scenario 6: Module Silme

**Başlık:** Kullanıcı modülü siler

**Preconditions:**
- Kullanıcı sisteme giriş yapmış
- Kullanıcı **Owner** veya **Member** rolüne sahip

**Flow:**
1. Kullanıcı modül detayında "Delete" butonuna tıklar
2. Onay modalı açılır: "Bu modül ve içindeki tüm UseCase'ler silinecek. Emin misiniz?"
3. Sistem yetki kontrolü yapar (`CanEditProject`)
4. Module silinir (`writeRepository.Remove()`)
5. Success mesajı gösterilir ve modül listesine yönlendirilir

**Postconditions:**
- Module veritabanından silinmiş
- İlişkili UseCase'ler ne oldu? (Cascade delete mi?)

**Edge Cases:**
- ❌ **Viewer rolündeki kullanıcı silerse?** → 403 Forbidden
- ⚠️ **İçinde UseCase olan modül silinebilir mi?** → Business rule gerekli
- ⚠️ **Cascade delete var mı?** → Kontrol edilmeli
- ✅ **Administrator silebilir mi?** → EVET

---

## 🔒 Authorization Matrix

| İşlem | Administrator | Owner | Member | Viewer | Non-Member |
|-------|--------------|-------|--------|--------|------------|
| **Create Module** | ✅ | ✅ | ✅ | ❌ | ❌ |
| **List Modules** | ✅ | ✅ | ✅ | ✅ | ❌ |
| **Get Module Detail** | ✅ | ✅ | ✅ | ✅ | ❌ |
| **Update Module** | ✅ | ✅ | ✅ | ❌ | ❌ |
| **Change Status** | ✅ | ✅ | ✅ | ❌ | ❌ |
| **Delete Module** | ✅ | ✅ | ✅ | ❌ | ❌ |

### Yetki Metotları (UserPermissionService)

```csharp
// Module Create için
CanCreateModule(UserId userId, Project project)
├─ IsSystemAdministrator → ✅ true
├─ ProjectRole.Owner → ✅ true
├─ ProjectRole.Member → ✅ true
└─ ProjectRole.Viewer → ❌ false

// Module Edit/Delete/ChangeStatus için
CanEditProject(UserId userId, Project project)
├─ IsSystemAdministrator → ✅ true
├─ ProjectRole.Owner → ✅ true
├─ ProjectRole.Member → ✅ true
└─ ProjectRole.Viewer → ❌ false

// Module List/GetById için
CanAccessProject(UserId userId, Project project)
├─ IsSystemAdministrator → ✅ true
├─ Project.CreatorId == userId → ✅ true
└─ ProjectMember exists → ✅ true
```

---

## 🌐 API Endpoints

### 1. Create Module
```http
POST /api/v1/projects/{projectId}/modules
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "title": "Authentication Module",
  "description": "User authentication and authorization"
}

Response: 201 Created
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Handler:** `CreateModuleCommandHandler`
**Validations:**
- Title: Required, MaxLength(200)
- ProjectId: Valid UUID
- User must be Owner or Member

---

### 2. List Modules
```http
GET /api/v1/projects/{projectId}/modules?page=1&pageSize=10&isActive=true&search=auth
Authorization: Bearer {token}

Response: 200 OK
{
  "items": [
    {
      "id": "guid",
      "title": "Authentication Module",
      "isActive": true,
      "startedDate": "2024-01-15T10:00:00Z",
      "useCaseCount": 5
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10
}
```

**Handler:** `ListModulesQueryHandler`
**Filters:**
- `search`: Title içinde arama (case-insensitive)
- `isActive`: null (all), true (active), false (archived)

---

### 3. Get Module by ID
```http
GET /api/v1/modules/{moduleId}
Authorization: Bearer {token}

Response: 200 OK
{
  "id": "guid",
  "projectId": "guid",
  "title": "Authentication Module",
  "description": "User authentication and authorization",
  "isActive": true,
  "startedDate": "2024-01-15T10:00:00Z"
}
```

**Handler:** `GetModuleByIdQueryHandler`

---

### 4. Update Module
```http
PUT /api/v1/modules/{moduleId}
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "title": "Updated Authentication Module",
  "description": "Enhanced authentication system"
}

Response: 200 OK
{
  "success": true
}
```

**Handler:** `UpdateModuleCommandHandler`
**Validations:**
- Title: Required, MaxLength(200)
- User must be Owner or Member
- Project must be active

---

### 5. Change Module Status
```http
PATCH /api/v1/modules/{moduleId}/status
Authorization: Bearer {token}
Content-Type: application/json

Request Body:
{
  "isActive": false
}

Response: 200 OK
{
  "success": true
}
```

**Handler:** `ChangeModuleStatusCommandHandler`

---

### 6. Delete Module
```http
DELETE /api/v1/modules/{moduleId}
Authorization: Bearer {token}

Response: 204 No Content
```

**Handler:** `DeleteModuleCommandHandler`

---

## 📜 Business Rules

### BR-1: Module Oluşturma Kuralları
1. ✅ **Proje aktif olmalı** → Arşivlenmiş projede modül oluşturulamaz
2. ✅ **Kullanıcı Owner veya Member olmalı** → Viewer oluşturamaz
3. ✅ **Title zorunludur** → Boş olamaz
4. ✅ **StartedDate otomatik atanır** → `DateTime.UtcNow`
5. ✅ **IsActive default true** → Yeni modül aktif olarak oluşturulur

### BR-2: Module Güncelleme Kuralları
1. ✅ **Proje aktif olmalı** → Arşivlenmiş projede güncellenemez
2. ✅ **Kullanıcı Owner veya Member olmalı**
3. ✅ **Title zorunludur**
4. ✅ **UpdatedDate otomatik güncellenir**

### BR-3: Module Durum Değiştirme Kuralları
1. ⚠️ **İçinde UseCase olan modül arşivlenebilir mi?**
   - **Şu anki kod:** EVET (kontrol yok)
   - **Öneri:** UseCase'lerin de arşivlenmesi gerekebilir (cascade)
2. ⚠️ **Arşivlenmiş modüle UseCase eklenebilir mi?**
   - **Kontrol edilmeli:** UseCase create handler'da kontrol var mı?

### BR-4: Module Silme Kuralları
1. ⚠️ **İçinde UseCase olan modül silinebilir mi?**
   - **Şu anki kod:** Kontrol YOK (muhtemelen FK constraint hatası verir)
   - **Öneri:** Cascade delete veya önce UseCase kontrolü
2. ✅ **Kullanıcı Owner veya Member olmalı**

### BR-5: İlişkisel Kurallar
1. ⚠️ **Proje silinirse Module'ler ne olur?**
   - Cascade delete mi?
   - Soft delete mi?
2. ⚠️ **Module silinirse UseCase'ler ne olur?**
   - Cascade delete mi?
   - Orphan record kalır mı?

---

## ⚠️ Edge Cases & Potential Issues

### 1. N+1 Query Problem (ListModules)
**Konum:** `ListModulesQueryHandler.cs:44-54`

```csharp
foreach (var module in page.Items)
{
    var useCaseCount = await useCaseRepository.CountByModuleAsync(module.Id, cancellationToken);
    // Her module için ayrı sorgu! ❌
}
```

**Problem:**
- 10 module varsa → 10 + 1 = 11 query
- 100 module varsa → 100 + 1 = 101 query

**Çözüm:**
```csharp
// Tek sorguda tüm module ID'leri için count al
var moduleIds = page.Items.Select(m => m.Id).ToList();
var useCaseCounts = await useCaseRepository.CountByModuleIdsAsync(moduleIds, cancellationToken);

// Dictionary'den eşleştir
foreach (var module in page.Items)
{
    var count = useCaseCounts.TryGetValue(module.Id, out var c) ? c : 0;
    items.Add(new ModuleListItemResponse { ..., UseCaseCount = count });
}
```

---

### 2. Cascade Delete Eksikliği
**Konum:** `DeleteModuleCommandHandler.cs`

**Senaryo:**
1. Module silinir
2. İçinde 10 UseCase var
3. UseCase'ler ne olur?

**Şu anki kod:**
```csharp
writeRepository.Remove(module, cancellationToken); // Sadece module siliniyor
await unitOfWork.SaveChangesAsync(cancellationToken);
```

**Olası sonuçlar:**
- ❌ **FK Constraint hatası** (eğer CASCADE delete yoksa)
- ❌ **Orphan UseCase'ler** (eğer FK NULL ise)
- ✅ **Cascade delete** (eğer DB'de tanımlıysa)

**Kontrol edilmeli:**
- `AppDbContext` configuration dosyaları
- `OnDelete(DeleteBehavior.Cascade)` var mı?

---

### 3. Arşivlenmiş Proje Kontrolü
**Konum:** Tüm Write Operations

**Durum:**
- ✅ **CreateModule:** Kontrol VAR
- ✅ **UpdateModule:** Kontrol VAR
- ❌ **ChangeStatus:** Kontrol YOK (arşivlenmiş projede modül aktif edilebilir mi?)
- ❌ **DeleteModule:** Kontrol YOK (ama mantıksal olarak sorun yok, silinebilir)

**Öneri:**
- ChangeStatus handler'a proje aktiflik kontrolü ekle

---

### 4. Arşivlenmiş Modül Kontrolü
**Konum:** UseCase Create (başka modülde)

**Soru:** Arşivlenmiş modüle UseCase eklenebilir mi?

**Kontrol edilmeli:**
- `CreateUseCaseCommandHandler` içinde `module.IsActive` kontrolü var mı?

---

### 5. Administrator Bypass
**Durum:** ✅ **ÇALIŞIYOR**

Administrator:
- Her projeye erişebilir
- Her modül üzerinde tüm işlemleri yapabilir
- Proje üyesi olmasa bile

**Örnek:**
```csharp
// CreateModuleCommandHandler.cs:31
if (!userPermissionService.CanCreateModule(creatorId, project))
    throw new ProjectAccessDeniedException(...);

// CanCreateModule metodu:
if (IsSystemAdministrator(userId)) return true; // ✅ Bypass
```

---

## 🧪 Kod Analizi

### Handler'lar

#### ✅ CreateModuleCommandHandler
**Durum:** ÇALIŞIYOR

**Pozitif Yönler:**
- ✅ Proje varlık kontrolü
- ✅ Yetki kontrolü (`CanCreateModule`)
- ✅ Proje aktiflik kontrolü
- ✅ Domain metodu kullanımı (`Module.Create`)

**Potansiyel İyileştirmeler:**
- ⚠️ Trial kullanıcı için modül limit kontrolü (Project'te olduğu gibi)

---

#### ✅ UpdateModuleCommandHandler
**Durum:** ÇALIŞIYOR

**Pozitif Yönler:**
- ✅ Modül varlık kontrolü
- ✅ Proje varlık kontrolü
- ✅ Yetki kontrolü (`CanEditProject`)
- ✅ Proje aktiflik kontrolü
- ✅ Domain metotları kullanımı

**Kontrol edilmeli:**
- Return değeri `false` dönüyor (module null ise) → Exception fırlatması daha tutarlı olabilir

---

#### ⚠️ ChangeModuleStatusCommandHandler
**Durum:** KISMEN ÇALIŞIYOR

**Eksikler:**
- ❌ Proje aktiflik kontrolü YOK
- ⚠️ Arşivlenmiş projede modül aktif edilebilir

**Öneri:**
```csharp
// Ekle:
if (!project.IsActive && request.IsActive)
    throw new BusinessRuleViolationException("Arşivlenmiş projede modül aktif edilemez.");
```

---

#### ⚠️ DeleteModuleCommandHandler
**Durum:** ÇALIŞIYOR - FAKAT KONTROL EKSİK

**Eksikler:**
- ❌ İçinde UseCase var mı kontrolü YOK
- ⚠️ Cascade delete davranışı belirsiz

**Öneri:**
```csharp
// UseCase kontrolü ekle:
var useCaseCount = await useCaseRepository.CountByModuleAsync(module.Id, cancellationToken);
if (useCaseCount > 0)
    throw new BusinessRuleViolationException(
        $"Modül içinde {useCaseCount} adet UseCase bulunmaktadır. Önce UseCase'leri silin.");
```

---

#### ⚠️ ListModulesQueryHandler
**Durum:** ÇALIŞIYOR - FAKAT PERFORMANCE SORUNU

**Problem:**
- ❌ N+1 Query problemi (her module için ayrı count sorgusu)

**Çözüm yukarıda belirtildi** (Edge Cases bölümü)

---

#### ✅ GetModuleByIdQueryHandler
**Durum:** ÇALIŞIYOR

**Pozitif Yönler:**
- ✅ Modül varlık kontrolü
- ✅ Proje varlık kontrolü
- ✅ Yetki kontrolü (`CanAccessProject`)
- ✅ Exception fırlatma tutarlı

---

## 📊 Öncelik Matrisi

### 🔴 Kritik (Bu Hafta)
1. **N+1 Query problemi düzelt** (ListModules)
2. **Cascade delete davranışını netleştir** (DeleteModule)
3. **UseCase count kontrolü ekle** (DeleteModule)

### 🟡 Orta Öncelik (Gelecek Sprint)
4. **Proje aktiflik kontrolü ekle** (ChangeModuleStatus)
5. **Trial kullanıcı limit kontrolü** (CreateModule)
6. **Arşivlenmiş modüle UseCase kontrolü** (CreateUseCase handler'da)

### 🟢 Düşük Öncelik (Backlog)
7. **Soft delete değerlendirmesi**
8. **Modül istatistikleri endpoint'i** (toplam task, tamamlanma oranı)

---

## 🎯 Sonuç

### ✅ İyi Taraflar
- Clean Architecture prensiplerine uygun
- Yetki kontrolleri güçlü
- Domain metotları kullanılıyor
- Validation katmanı mevcut

### ⚠️ İyileştirme Gereken Alanlar
1. **Performance:** N+1 Query sorunu
2. **Business Rules:** Cascade delete belirsizliği
3. **Validation:** İçinde UseCase olan modül silme kontrolü
4. **Consistency:** ChangeModuleStatus'ta proje aktiflik kontrolü eksik

### 📌 Aksiyonlar
- [ ] N+1 Query çözümü implement et
- [ ] Cascade delete davranışını tanımla ve test et
- [ ] DeleteModule'a UseCase kontrolü ekle
- [ ] ChangeModuleStatus'a proje aktiflik kontrolü ekle
- [ ] Integration testler yaz

---

**Hazırlayan:** AI Assistant  
**Tarih:** {{ current_date }}  
**Versiyon:** 1.0
