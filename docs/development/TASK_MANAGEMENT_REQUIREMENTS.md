# Task Management - Genel İsterler Dokümanı

## 1. Genel Bakış

Bu doküman, TE4IT projesinde **Project**, **Module**, **UseCase** ve **Task** yönetimi için genel gereksinimleri tanımlar. Proje hiyerarşisi şu şekildedir:

```
Project (✅ Backend Tamamlandı, ⏳ Frontend Yapılacak)
  └── Module (⏳ Yapılacak)
      └── UseCase (⏳ Yapılacak)
          └── Task (⏳ Yapılacak)
              └── TaskRelation (⏳ Yapılacak)
```

## 2. Entity Hiyerarşisi ve İlişkiler

### 2.1 Hiyerarşik Yapı

- **Project**: En üst seviye entity (✅ Backend tamamlandı, ⏳ Frontend yapılacak)
  - Bir proje birden fazla **Module** içerebilir
  - Proje silinirse, modülleri de silinir (CASCADE veya RESTRICT - karar verilecek)
  - Proje oluşturulduğunda `ProjectCreatedEvent` domain event fırlatılır
  - Proje durumu değiştiğinde `ProjectStatusChangedEvent` domain event fırlatılır

- **Module**: Proje altında modül yönetimi
  - Bir modül bir **Project**'e aittir (ProjectId FK)
  - Bir modül birden fazla **UseCase** içerebilir
  - Modül silinirse, use case'leri de silinir (CASCADE veya RESTRICT)

- **UseCase**: Modül altında kullanım senaryosu yönetimi
  - Bir use case bir **Module**'e aittir (ModuleId FK)
  - Bir use case birden fazla **Task** içerebilir
  - Use case silinirse, task'ları da silinir (CASCADE veya RESTRICT)

- **Task**: Use case altında görev yönetimi
  - Bir task bir **UseCase**'e aittir (UseCaseId FK)
  - Bir task birden fazla **TaskRelation** içerebilir
  - Task'lar arasında bağımlılık ilişkileri kurulabilir

- **TaskRelation**: Task'lar arası ilişki yönetimi
  - Bir task relation bir **SourceTask** ve bir **TargetTask** içerir
  - İlişki tipleri: Blocks, RelatesTo, Fixes, Duplicates

### 2.2 Entity Özellikleri

#### Project
- `Id` (Guid, PK)
- `CreatorId` (UserId, FK → AspNetUsers)
- `Title` (string, 3-120 karakter)
- `Description` (string?, max 1000 karakter)
- `StartedDate` (DateTime)
- `IsActive` (bool)
- `CreatedDate`, `UpdatedDate`, `DeletedDate` (Audit fields)

#### Module
- `Id` (Guid, PK)
- `ProjectId` (Guid, FK → Projects)
- `CreatorId` (UserId, FK → AspNetUsers)
- `Title` (string, 3-100 karakter)
- `Description` (string?, max 1000 karakter)
- `StartedDate` (DateTime)
- `IsActive` (bool)
- `CreatedDate`, `UpdatedDate`, `DeletedDate` (Audit fields)

#### UseCase
- `Id` (Guid, PK)
- `ModuleId` (Guid, FK → Modules)
- `CreatorId` (UserId, FK → AspNetUsers)
- `Title` (string, 3-100 karakter)
- `Description` (string?, max 1000 karakter)
- `ImportantNotes` (string?, max 500 karakter)
- `StartedDate` (DateTime)
- `IsActive` (bool)
- `CreatedDate`, `UpdatedDate`, `DeletedDate` (Audit fields)

#### Task
- `Id` (Guid, PK)
- `UseCaseId` (Guid, FK → UseCases)
- `CreatorId` (UserId, FK → AspNetUsers)
- `AssigneeId` (UserId, FK → AspNetUsers)
- `Title` (string, 3-200 karakter)
- `Description` (string?, max 2000 karakter)
- `ImportantNotes` (string?, max 1000 karakter)
- `StartedDate` (DateTime)
- `DueDate` (DateTime?, nullable)
- `TaskType` (enum: Documentation, Feature, Test, Bug)
- `TaskState` (enum: NotStarted, InProgress, Completed, Cancelled)
- `CreatedDate`, `UpdatedDate`, `DeletedDate` (Audit fields)

#### TaskRelation
- `Id` (Guid, PK)
- `SourceTaskId` (Guid, FK → Tasks)
- `TargetTaskId` (Guid, FK → Tasks)
- `RelationType` (enum: Blocks, RelatesTo, Fixes, Duplicates)
- `CreatedDate`, `UpdatedDate`, `DeletedDate` (Audit fields)

## 3. İş Kuralları (Business Rules)

### 3.1 Project İş Kuralları

1. **Oluşturma**
   - Title zorunludur (3-120 karakter)
   - Description opsiyoneldir (max 1000 karakter)
   - Varsayılan olarak `IsActive = true` ve `StartedDate = UtcNow`
   - Trial kullanıcılar en fazla 1 proje oluşturabilir (quota kontrolü)
   - Proje oluşturulduğunda `ProjectCreatedEvent` domain event fırlatılır

2. **Güncelleme**
   - Sadece title ve description güncellenebilir
   - Proje aktif olmalıdır (arşivlenmiş projeler güncellenemez)

3. **Durum Yönetimi**
   - `Activate()`: Projeyi aktif yapar
   - `Archive()`: Projeyi arşivler (IsActive = false)
   - Durum değişikliğinde `ProjectStatusChangedEvent` domain event fırlatılır
   - Arşivlenmiş projede yeni modül oluşturulamaz

4. **Silme**
   - Proje silinirse, altındaki tüm modüller de silinir (CASCADE veya RESTRICT - karar verilecek)
   - Soft delete kullanılıyorsa, `DeletedDate` set edilir

### 3.2 Module İş Kuralları

1. **Oluşturma**
   - Module oluşturmak için projenin aktif olması gerekir
   - Title zorunludur (3-100 karakter)
   - Description opsiyoneldir (max 1000 karakter)
   - Varsayılan olarak `IsActive = true` ve `StartedDate = UtcNow`

2. **Güncelleme**
   - Sadece title ve description güncellenebilir
   - Module'ün projesi aktif olmalıdır

3. **Durum Yönetimi**
   - `Activate()`: Module'ü aktif yapar
   - `Archive()`: Module'ü arşivler (IsActive = false)
   - Arşivlenmiş modülde yeni use case oluşturulamaz

4. **Silme**
   - Module silinirse, altındaki tüm use case'ler de silinir (CASCADE veya RESTRICT - karar verilecek)

### 3.3 UseCase İş Kuralları

1. **Oluşturma**
   - Use case oluşturmak için modülün aktif olması gerekir
   - Title zorunludur (3-100 karakter)
   - Description ve ImportantNotes opsiyoneldir
   - Varsayılan olarak `IsActive = true` ve `StartedDate = UtcNow`

2. **Güncelleme**
   - Title, description ve important notes güncellenebilir
   - Use case'in modülü aktif olmalıdır

3. **Durum Yönetimi**
   - `Activate()`: Use case'i aktif yapar
   - `Archive()`: Use case'i arşivler (IsActive = false)
   - Arşivlenmiş use case'de yeni task oluşturulamaz

4. **Silme**
   - Use case silinirse, altındaki tüm task'lar da silinir (CASCADE veya RESTRICT)

### 3.4 Task İş Kuralları

1. **Oluşturma**
   - Task oluşturmak için use case'in aktif olması gerekir
   - Title zorunludur (3-200 karakter)
   - Description ve ImportantNotes opsiyoneldir
   - TaskType zorunludur (Documentation, Feature, Test, Bug)
   - Varsayılan olarak `TaskState = NotStarted`
   - Varsayılan olarak `AssigneeId = CreatorId` (oluşturan kişiye atanır)
   - `StartedDate = UtcNow`

2. **Güncelleme**
   - Title, description, important notes, due date güncellenebilir
   - Task'ın use case'i aktif olmalıdır

3. **Durum Geçişleri (State Transitions)**
   - `Start()`: NotStarted → InProgress
     - Sadece NotStarted durumundan InProgress'e geçilebilir
   - `Complete()`: InProgress → Completed
     - Sadece InProgress durumundan Completed'e geçilebilir
     - Bloklanmış task'lar tamamlanamaz (Blocks ilişkisi kontrol edilir)
   - `Cancel()`: NotStarted/InProgress → Cancelled
     - Completed durumundan Cancelled'e geçilemez
   - `Revert()`: InProgress/Cancelled → NotStarted
     - Completed durumundan NotStarted'e geçilemez

4. **Atama (Assignment)**
   - `AssignTo(userId, assignerId)`: Task'ı bir kullanıcıya atar
   - Atama işlemi domain event fırlatır (TaskAssignedEvent)

5. **Bağımlılık Yönetimi**
   - `AddRelation(relation)`: Task'a bağımlılık ekler
   - `RemoveRelation(relationId)`: Bağımlılığı kaldırır
   - Döngüsel bağımlılık kontrolü yapılmalıdır (uygulama seviyesinde)
   - `CanBeCompleted()`: Task'ın tamamlanabilir olup olmadığını kontrol eder
     - InProgress durumunda olmalı
     - Bloklanmış task'ları olmamalı

6. **Validasyonlar**
   - `IsOverdue()`: DueDate geçmişse ve Completed değilse gecikmiş sayılır
   - DueDate, StartedDate'den önce olamaz

### 3.5 TaskRelation İş Kuralları

1. **Oluşturma**
   - SourceTaskId ve TargetTaskId farklı olmalıdır
   - Döngüsel bağımlılık kontrolü yapılmalıdır
   - RelationType zorunludur

2. **İlişki Tipleri**
   - **Blocks**: Bu task tamamlanmadan diğer task ilerleyemez
   - **RelatesTo**: Görevler birbiriyle bağlantılıdır (zorunlu bağımlılık yok)
   - **Fixes**: Bu task, belirli bir bug veya problemi düzeltir
   - **Duplicates**: Bu task veya bug başka bir task'la aynıdır (tekrar)

3. **Silme**
   - TaskRelation silinirse, ilgili task'ların bağımlılık durumu güncellenir

## 4. Yetkilendirme (Authorization)

### 4.1 Project Yetkilendirme

- **ProjectCreate**: Administrator, OrganizationManager, TeamLead, Trial
- **ProjectRead**: Administrator, OrganizationManager, TeamLead, Employee, Trial (varsayılan, tüm authenticated kullanıcılar)
- **ProjectUpdate**: Administrator, OrganizationManager, TeamLead, Trial
- **ProjectDelete**: Administrator, OrganizationManager

### 4.2 Module Yetkilendirme

- **ModuleCreate**: Administrator, OrganizationManager, TeamLead, Trial
- **ModuleRead**: Administrator, OrganizationManager, TeamLead, Employee, Trial
- **ModuleUpdate**: Administrator, OrganizationManager, TeamLead, Trial
- **ModuleDelete**: Administrator, OrganizationManager

### 4.3 UseCase Yetkilendirme

- **UseCaseCreate**: Administrator, OrganizationManager, TeamLead, Trial
- **UseCaseRead**: Administrator, OrganizationManager, TeamLead, Employee, Trial
- **UseCaseUpdate**: Administrator, OrganizationManager, TeamLead, Trial
- **UseCaseDelete**: Administrator, OrganizationManager

### 4.4 Task Yetkilendirme

- **TaskCreate**: Administrator, OrganizationManager, TeamLead, Employee, Trial
- **TaskRead**: Administrator, OrganizationManager, TeamLead, Employee, Trial
- **TaskUpdate**: Administrator, OrganizationManager, TeamLead, Employee (sadece kendi task'ları)
- **TaskAssign**: Administrator, OrganizationManager, TeamLead
- **TaskStateChange**: Administrator, OrganizationManager, TeamLead, Employee (sadece kendi task'ları)
- **TaskDelete**: Administrator, OrganizationManager

### 4.5 TaskRelation Yetkilendirme

- **TaskRelationCreate**: Administrator, OrganizationManager, TeamLead
- **TaskRelationDelete**: Administrator, OrganizationManager, TeamLead

## 5. API Endpoint Gereksinimleri

### 5.1 Project Endpoints

- `GET /api/v1/projects` - Projeleri listele (pagination, filtering)
- `GET /api/v1/projects/{id}` - Proje detayını getir
- `POST /api/v1/projects` - Proje oluştur
- `PUT /api/v1/projects/{id}` - Proje güncelle
- `PATCH /api/v1/projects/{id}/status` - Proje durumunu değiştir (activate/archive)
- `DELETE /api/v1/projects/{id}` - Proje sil

### 5.2 Module Endpoints

- `GET /api/v1/projects/{projectId}/modules` - Proje modüllerini listele (pagination)
- `GET /api/v1/modules/{id}` - Modül detayını getir
- `POST /api/v1/projects/{projectId}/modules` - Modül oluştur
- `PUT /api/v1/modules/{id}` - Modül güncelle
- `PATCH /api/v1/modules/{id}/status` - Modül durumunu değiştir (activate/archive)
- `DELETE /api/v1/modules/{id}` - Modül sil

### 5.3 UseCase Endpoints

- `GET /api/v1/modules/{moduleId}/usecases` - Modül use case'lerini listele (pagination)
- `GET /api/v1/usecases/{id}` - Use case detayını getir
- `POST /api/v1/modules/{moduleId}/usecases` - Use case oluştur
- `PUT /api/v1/usecases/{id}` - Use case güncelle
- `PATCH /api/v1/usecases/{id}/status` - Use case durumunu değiştir (activate/archive)
- `DELETE /api/v1/usecases/{id}` - Use case sil

### 5.4 Task Endpoints

- `GET /api/v1/usecases/{useCaseId}/tasks` - Use case task'larını listele (pagination, filtering)
- `GET /api/v1/tasks/{id}` - Task detayını getir
- `POST /api/v1/usecases/{useCaseId}/tasks` - Task oluştur
- `PUT /api/v1/tasks/{id}` - Task güncelle
- `PATCH /api/v1/tasks/{id}/state` - Task durumunu değiştir (start/complete/cancel/revert)
- `POST /api/v1/tasks/{id}/assign` - Task'ı bir kullanıcıya ata
- `DELETE /api/v1/tasks/{id}` - Task sil

### 5.5 TaskRelation Endpoints

- `GET /api/v1/tasks/{taskId}/relations` - Task ilişkilerini listele
- `POST /api/v1/tasks/{taskId}/relations` - Task ilişkisi oluştur
- `DELETE /api/v1/tasks/{taskId}/relations/{relationId}` - Task ilişkisini sil

## 6. Frontend Gereksinimleri

### 6.1 Navigasyon Hiyerarşisi

```
Projects List (⏳ Yapılacak)
  └── Project Detail (⏳ Yapılacak)
      └── Modules List (⏳ Yapılacak)
          └── Module Detail (⏳ Yapılacak)
              └── UseCases List (⏳ Yapılacak)
                  └── UseCase Detail (⏳ Yapılacak)
                      └── Tasks List (⏳ Yapılacak)
                          └── Task Detail (⏳ Yapılacak)
                              └── Task Relations (⏳ Yapılacak)
```

### 6.2 Sayfa Gereksinimleri

1. **Projects List Page** (⏳ Yapılacak)
   - Proje listesi görüntüleme (pagination)
   - Yeni proje oluşturma butonu
   - Proje filtreleme/arama (status, search)
   - Proje detayına yönlendirme

2. **Project Detail Page** (⏳ Yapılacak)
   - Proje bilgileri (title, description, status, started date)
   - Proje durum değiştirme (activate/archive)
   - Proje güncelleme/silme
   - Modül listesi görüntüleme
   - Yeni modül oluşturma butonu
   - Modül filtreleme/arama

3. **Module Detail Page** (⏳ Yapılacak)
   - Use case listesi görüntüleme
   - Yeni use case oluşturma butonu
   - Use case filtreleme/arama
   - Modül bilgileri (title, description, status)

4. **UseCase Detail Page** (⏳ Yapılacak)
   - Task listesi görüntüleme (Kanban veya liste görünümü)
   - Yeni task oluşturma butonu
   - Task filtreleme/arama (state, type, assignee)
   - Use case bilgileri (title, description, important notes, status)

5. **Task Detail Page** (⏳ Yapılacak)
   - Task detay bilgileri
   - Durum değiştirme butonları (Start, Complete, Cancel, Revert)
   - Atama işlemi (user picker)
   - Task ilişkileri yönetimi
   - Due date takibi
   - Gecikmiş task uyarıları

6. **Dashboard/Overview Page** (⏳ Yapılacak)
   - Proje istatistikleri
   - Task durum dağılımı
   - Gecikmiş task'lar
   - Yaklaşan deadline'lar

### 6.3 UI/UX Gereksinimleri

1. **Breadcrumb Navigation**
   - Her sayfada hiyerarşik navigasyon (Project > Module > UseCase > Task)

2. **Kanban Board** (Opsiyonel)
   - Task'ları state'e göre sütunlarda gösterme
   - Drag & drop ile state değiştirme

3. **Filtreleme ve Arama**
   - Tüm listelerde filtreleme (status, type, assignee, date range)
   - Arama (title, description)

4. **Responsive Design**
   - Mobil uyumlu tasarım
   - Tablet ve desktop optimizasyonu

## 7. Performans Gereksinimleri

1. **Listeleme**
   - Pagination: Varsayılan 20, maksimum 100 item per page
   - Response time: < 500ms (1000 item için)

2. **Detay Getirme**
   - Response time: < 200ms

3. **Oluşturma/Güncelleme**
   - Response time: < 300ms

## 8. Test Gereksinimleri

1. **Unit Tests**
   - Domain entity testleri (Module, UseCase, Task)
   - Handler testleri (Commands, Queries)
   - Validator testleri

2. **Integration Tests**
   - Repository testleri
   - API endpoint testleri

3. **E2E Tests** (Frontend)
   - Kullanıcı akışları testleri
   - Form validasyon testleri

## 9. Notlar ve Kararlar

1. **Project Backend Durumu**: 
   - ✅ Backend tamamlandı (Commands, Queries, Handlers, Validators, API Controller)
   - ⏳ Frontend geliştirilecek (Projects List, Project Detail sayfaları)
   - Trial kullanıcı quota kontrolü mevcut (en fazla 1 proje)

2. **CASCADE vs RESTRICT**: 
   - Project/Module/UseCase/Task silme işlemlerinde CASCADE mi RESTRICT mi kullanılacağına karar verilmeli
   - Şu an için RESTRICT kullanılıyor (foreign key constraint'lerde)

3. **Soft Delete**: 
   - Tüm entity'lerde soft delete kullanılıyor mu kontrol edilmeli
   - `DeletedDate` field'ı mevcut, repository'lerde filtrelenmeli

4. **Audit Trail**: 
   - Tüm değişiklikler audit log'a kaydedilmeli
   - `CreatedDate`, `UpdatedDate`, `DeletedDate` field'ları mevcut

5. **Domain Events**: 
   - Project: `ProjectCreatedEvent`, `ProjectStatusChangedEvent` mevcut
   - Task: State değişiklikleri, atamalar domain event fırlatmalı
   - Task domain events: `TaskStartedEvent`, `TaskCompletedEvent`, `TaskCancelledEvent`, `TaskRevertedEvent`, `TaskAssignedEvent` mevcut

