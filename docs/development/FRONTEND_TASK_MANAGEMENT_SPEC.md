# Frontend Task Management - Geliştirici Dokümanı

## 📋 Genel Bakış

Bu doküman, frontend geliştiricisi için **Project**, **Module**, **UseCase** ve **Task** yönetimi sayfalarının detaylı spesifikasyonlarını içerir. Backend API'leri hazır olduğunda bu sayfalar geliştirilecektir.

## 🎯 Hiyerarşik Yapı

```
Projects List (⏳ Yapılacak)
  └── Project Detail (⏳ Yapılacak)
      └── Modules List (⏳ Yapılacak)
          └── Module Detail (⏳ Yapılacak)
              └── UseCases List (⏳ Yapılacak)
                  └── UseCase Detail (⏳ Yapılacak)
                      └── Tasks List (⏳ Yapılacak)
                          └── Task Detail (⏳ Yapılacak)
```

---

## 📄 0. Projects List Page

### 0.1 Sayfa Yapısı

**Route**: `/projects`

**Layout**:
```
┌─────────────────────────────────────────────────────────┐
│ Projects                                                 │
│ [+ Create Project] [Filter ▼] [Search...]               │
├─────────────────────────────────────────────────────────┤
│ Title              | Status | Started Date | Actions    │
│────────────────────|────────|──────────────|───────────│
│ E-Commerce Platform| Active | 2025-01-15   | [View]     │
│ Mobile App         | Active | 2025-01-20   | [View]     │
│ Legacy System      | Archived| 2024-12-01  | [View]     │
└─────────────────────────────────────────────────────────┘
[Pagination: 1 2 3 ... 10]
```

### 0.2 Özellikler

- **Project Listesi**: Pagination ile proje listesi
- Her proje için:
  - Title (tıklanabilir, Project Detail'e gider)
  - Status badge (Active/Archived)
  - Started Date
  - Actions: View (Project Detail'e gider)
- **Create Project** butonu (yetki kontrolü: `ProjectCreate`)
- **Filter**: Status (All, Active, Archived)
- **Search**: Title ve Description'a göre arama
- **Pagination**: Sayfa başına 20 item (varsayılan)

### 0.3 API Endpoints

- `GET /api/v1/projects?page=1&pageSize=20&isActive=true&search=`
- `POST /api/v1/projects` (Create Project modal/form)

### 0.4 Formlar ve Modaller

#### Create Project Modal
```
┌─────────────────────────────────────┐
│ Create Project                      │
├─────────────────────────────────────┤
│ Title *                            │
│ [___________________________]       │
│                                     │
│ Description                         │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ [Cancel] [Create]                   │
└─────────────────────────────────────┘
```

**Validasyon**:
- Title: Zorunlu, 3-120 karakter
- Description: Opsiyonel, max 1000 karakter

**Yetkilendirme**:
- Listeleme: `ProjectRead` permission (varsayılan, tüm authenticated kullanıcılar)
- Oluşturma: `ProjectCreate` permission
- Güncelleme: `ProjectUpdate` permission
- Silme: `ProjectDelete` permission

---

## 📄 0.5. Project Detail Page

### 0.5.1 Sayfa Yapısı

**Route**: `/projects/{projectId}`

**Breadcrumb**:
```
Projects > E-Commerce Platform
```

**Layout**:
```
┌─────────────────────────────────────────────────────────┐
│ [← Back to Projects]                                    │
│                                                          │
│ Project: E-Commerce Platform                            │
│ Status: [Active] [Archive]                              │
│                                                          │
│ Description:                                            │
│ Modern e-commerce platform with microservices           │
│                                                          │
│ Started: 2025-01-15                                     │
│                                                          │
│ [Edit Project] [Delete Project]                        │
├─────────────────────────────────────────────────────────┤
│ Modules                                                  │
│ [+ Create Module] [Filter ▼] [Search...]                │
├─────────────────────────────────────────────────────────┤
│ Title              | Status | Use Cases | Actions       │
│────────────────────|────────|───────────|───────────────│
│ User Management    | Active | 5         | [Edit] [View] │
│ Payment System     | Active | 3         | [Edit] [View] │
│ Product Catalog    | Archived| 0        | [Edit] [View] │
└─────────────────────────────────────────────────────────┘
```

### 0.5.2 Özellikler

#### Project Bilgileri Bölümü
- **Title**: Proje başlığı
- **Description**: Proje açıklaması
- **Status Badge**: Active/Archived
- **Status Actions**: 
  - Active ise: [Archive] butonu
  - Archived ise: [Activate] butonu
- **Started Date**: Proje başlangıç tarihi (read-only)
- **Actions**:
  - [Edit Project] - Modal/form açılır
  - [Delete Project] - Onay dialogu ile silme

#### Module Listesi Bölümü
- Module listesi (pagination ile)
- Her modül için:
  - Title
  - Status badge (Active/Archived)
  - Use case sayısı (tıklanabilir, UseCase listesine yönlendirir)
  - Actions: Edit, View (Module Detail'e gider)
- **Create Module** butonu (yetki kontrolü: `ModuleCreate`)
- **Filter**: Status (All, Active, Archived)
- **Search**: Title'a göre arama
- **Pagination**: Sayfa başına 20 item

### 0.5.3 API Endpoints

- `GET /api/v1/projects/{projectId}` - Project detayı
- `PUT /api/v1/projects/{projectId}` - Project güncelleme
- `PATCH /api/v1/projects/{projectId}/status` - Status değiştirme
- `DELETE /api/v1/projects/{projectId}` - Project silme
- `GET /api/v1/projects/{projectId}/modules?page=1&pageSize=20&status=Active&search=`
- `POST /api/v1/projects/{projectId}/modules` - Module oluşturma

### 0.5.4 Formlar ve Modaller

#### Edit Project Modal
```
┌─────────────────────────────────────┐
│ Edit Project                         │
├─────────────────────────────────────┤
│ Title *                            │
│ [___________________________]       │
│                                     │
│ Description                         │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ [Cancel] [Save]                     │
└─────────────────────────────────────┘
```

**Validasyon**:
- Title: Zorunlu, 3-120 karakter
- Description: Opsiyonel, max 1000 karakter

---

## 📄 1. Project Detail Page

### 1.1 Sayfa Yapısı

**Route**: `/projects/{projectId}`

**Breadcrumb**:
```
Projects > E-Commerce Platform
```

**Layout**:
```
┌─────────────────────────────────────────────────────────┐
│ [← Back to Projects]                                    │
│                                                          │
│ Project: E-Commerce Platform                            │
│ Status: [Active] [Archive]                             │
│                                                          │
│ Description:                                            │
│ Modern e-commerce platform with microservices           │
│                                                          │
│ Started: 2025-01-15                                     │
│                                                          │
│ [Edit Project] [Delete Project]                        │
├─────────────────────────────────────────────────────────┤
│ Modules                                                  │
│ [+ Create Module] [Filter ▼] [Search...]                │
├─────────────────────────────────────────────────────────┤
│ Title              | Status | Use Cases | Actions       │
│────────────────────|────────|───────────|───────────────│
│ User Management    | Active | 5         | [Edit] [View] │
│ Payment System     | Active | 3         | [Edit] [View] │
│ Product Catalog    | Archived| 0        | [Edit] [View] │
└─────────────────────────────────────────────────────────┘
```

### 1.2 Özellikler

#### Project Bilgileri Bölümü
- **Title**: Proje başlığı
- **Description**: Proje açıklaması
- **Status Badge**: Active/Archived
- **Status Actions**: 
  - Active ise: [Archive] butonu
  - Archived ise: [Activate] butonu
- **Started Date**: Proje başlangıç tarihi (read-only)
- **Actions**:
  - [Edit Project] - Modal/form açılır
  - [Delete Project] - Onay dialogu ile silme

#### Module Listesi Bölümü
- Module listesi (pagination ile)
- Her modül için:
  - Title
  - Status badge (Active/Archived)
  - Use case sayısı (tıklanabilir, UseCase listesine yönlendirir)
  - Actions: Edit, View (Module Detail'e gider)
- **Create Module** butonu (yetki kontrolü: `ModuleCreate`)
- **Filter**: Status (All, Active, Archived)
- **Search**: Title'a göre arama
- **Pagination**: Sayfa başına 20 item

### 1.3 API Endpoints

- `GET /api/v1/projects/{projectId}` - Project detayı
- `PUT /api/v1/projects/{projectId}` - Project güncelleme
- `PATCH /api/v1/projects/{projectId}/status` - Status değiştirme
- `DELETE /api/v1/projects/{projectId}` - Project silme
- `GET /api/v1/projects/{projectId}/modules?page=1&pageSize=20&status=Active&search=`
- `POST /api/v1/projects/{projectId}/modules` - Module oluşturma

### 1.4 Formlar ve Modaller

#### Edit Project Modal
```
┌─────────────────────────────────────┐
│ Edit Project                         │
├─────────────────────────────────────┤
│ Title *                            │
│ [___________________________]       │
│                                     │
│ Description                         │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ [Cancel] [Save]                     │
└─────────────────────────────────────┘
```

**Validasyon**:
- Title: Zorunlu, 3-120 karakter
- Description: Opsiyonel, max 1000 karakter

**Yetkilendirme**:
- Listeleme: `ProjectRead` permission
- Oluşturma: `ProjectCreate` permission
- Güncelleme: `ProjectUpdate` permission
- Silme: `ProjectDelete` permission
- Module listeleme: `ModuleRead` permission
- Module oluşturma: `ModuleCreate` permission

---

## 📄 2. Module Detail Page

### 2.1 Sayfa Yapısı

**Route**: `/projects/{projectId}/modules/{moduleId}`

**Breadcrumb**:
```
Projects > E-Commerce Platform > User Management
```

**Layout**:
```
┌─────────────────────────────────────────────────────────┐
│ [← Back to Project]                                     │
│                                                          │
│ Module: User Management                                │
│ Status: [Active] [Archive]                             │
│                                                          │
│ Description:                                            │
│ User authentication and authorization module            │
│                                                          │
│ [Edit Module] [Delete Module]                           │
├─────────────────────────────────────────────────────────┤
│ Use Cases                                                │
│ [+ Create Use Case] [Filter ▼] [Search...]              │
├─────────────────────────────────────────────────────────┤
│ Title              | Status | Tasks | Actions            │
│────────────────────|────────|───────|───────────────────│
│ User Login         | Active | 8     | [Edit] [View]    │
│ User Registration  | Active | 5     | [Edit] [View]    │
│ Password Reset     | Archived| 0     | [Edit] [View]    │
└─────────────────────────────────────────────────────────┘
```

### 2.2 Özellikler

#### Module Bilgileri Bölümü
- **Title**: Modül başlığı
- **Description**: Modül açıklaması
- **Status Badge**: Active/Archived
- **Status Actions**: 
  - Active ise: [Archive] butonu
  - Archived ise: [Activate] butonu
- **Actions**:
  - [Edit Module] - Modal/form açılır
  - [Delete Module] - Onay dialogu ile silme

#### Use Case Listesi Bölümü
- Use case listesi (pagination ile)
- Her use case için:
  - Title
  - Status badge (Active/Archived)
  - Task sayısı (tıklanabilir, Task listesine yönlendirir)
  - Actions: Edit, View (UseCase Detail'e gider)
- **Create Use Case** butonu (yetki kontrolü: `UseCaseCreate`)
- **Filter**: Status (All, Active, Archived)
- **Search**: Title'a göre arama
- **Pagination**: Sayfa başına 20 item

### 2.3 API Endpoints

- `GET /api/v1/modules/{moduleId}` - Module detayı
- `PUT /api/v1/modules/{moduleId}` - Module güncelleme
- `PATCH /api/v1/modules/{moduleId}/status` - Status değiştirme
- `DELETE /api/v1/modules/{moduleId}` - Module silme
- `GET /api/v1/modules/{moduleId}/usecases?page=1&pageSize=20&status=Active&search=`
- `POST /api/v1/modules/{moduleId}/usecases` - Use case oluşturma

### 2.4 Formlar ve Modaller

#### Create/Edit Module Modal
```
┌─────────────────────────────────────┐
│ Create Module                       │
├─────────────────────────────────────┤
│ Title *                            │
│ [___________________________]       │
│                                     │
│ Description                         │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ [Cancel] [Create]                   │
└─────────────────────────────────────┘
```

**Validasyon**:
- Title: Zorunlu, 3-100 karakter
- Description: Opsiyonel, max 1000 karakter

---

## 📄 3. UseCase Detail Page

### 3.1 Sayfa Yapısı

**Route**: `/projects/{projectId}/modules/{moduleId}/usecases/{useCaseId}`

**Breadcrumb**:
```
Projects > E-Commerce Platform > User Management > User Login
```

**Layout**:
```
┌─────────────────────────────────────────────────────────┐
│ [← Back to Module]                                      │
│                                                          │
│ Use Case: User Login                                    │
│ Status: [Active] [Archive]                              │
│                                                          │
│ Description:                                            │
│ User authentication flow with email and password        │
│                                                          │
│ Important Notes:                                        │
│ ⚠️ Remember to implement rate limiting                  │
│                                                          │
│ [Edit Use Case] [Delete Use Case]                       │
├─────────────────────────────────────────────────────────┤
│ Tasks                                                    │
│ [+ Create Task] [Filter ▼] [View: List/Kanban]         │
├─────────────────────────────────────────────────────────┤
│ [Kanban View veya List View]                            │
│                                                          │
│ Not Started | In Progress | Completed | Cancelled       │
│ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐          │
│ │ Task 1  │ │ Task 2  │ │ Task 3  │ │ Task 4 │          │
│ │ [View]  │ │ [View]  │ │ [View]  │ │ [View] │          │
│ └─────────┘ └─────────┘ └─────────┘ └────────┘          │
└─────────────────────────────────────────────────────────┘
```

### 3.2 Özellikler

#### Use Case Bilgileri Bölümü
- **Title**: Use case başlığı
- **Description**: Use case açıklaması
- **Important Notes**: Önemli notlar (⚠️ ikonu ile vurgulanmış)
- **Status Badge**: Active/Archived
- **Status Actions**: 
  - Active ise: [Archive] butonu
  - Archived ise: [Activate] butonu
- **Actions**:
  - [Edit Use Case] - Modal/form açılır
  - [Delete Use Case] - Onay dialogu ile silme

#### Task Listesi Bölümü
- **View Toggle**: List View / Kanban View
- **List View**:
  ```
  Title          | Type      | State      | Assignee | Due Date | Actions
  ──────────────|───────────|────────────|──────────|──────────|────────
  Design Login   | Feature   | InProgress | John Doe | 2025-02-01| [View]
  Write Tests    | Test      | NotStarted | Jane Doe | 2025-02-05| [View]
  Fix Bug #123   | Bug       | Completed  | John Doe | 2025-01-30| [View]
  ```
- **Kanban View**:
  - 4 sütun: Not Started, In Progress, Completed, Cancelled
  - Drag & drop ile state değiştirme
  - Her task kartında: Title, Type badge, Assignee, Due Date
- **Filter**: 
  - Status (All, NotStarted, InProgress, Completed, Cancelled)
  - Type (All, Documentation, Feature, Test, Bug)
  - Assignee (All, Me, Specific user)
  - Due Date (All, Overdue, Today, This Week, This Month)
- **Search**: Title, Description'a göre arama
- **Pagination**: Sayfa başına 20 item (List View için)

### 3.3 API Endpoints

- `GET /api/v1/usecases/{useCaseId}` - Use case detayı
- `PUT /api/v1/usecases/{useCaseId}` - Use case güncelleme
- `PATCH /api/v1/usecases/{useCaseId}/status` - Status değiştirme
- `DELETE /api/v1/usecases/{useCaseId}` - Use case silme
- `GET /api/v1/usecases/{useCaseId}/tasks?page=1&pageSize=20&state=InProgress&type=Feature&assignee=&dueDate=`
- `POST /api/v1/usecases/{useCaseId}/tasks` - Task oluşturma

### 3.4 Formlar ve Modaller

#### Create/Edit Use Case Modal
```
┌─────────────────────────────────────┐
│ Create Use Case                     │
├─────────────────────────────────────┤
│ Title *                            │
│ [___________________________]       │
│                                     │
│ Description                         │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ Important Notes                     │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ [Cancel] [Create]                   │
└─────────────────────────────────────┘
```

**Validasyon**:
- Title: Zorunlu, 3-100 karakter
- Description: Opsiyonel, max 1000 karakter
- Important Notes: Opsiyonel, max 500 karakter

---

## 📄 4. Task Detail Page

### 4.1 Sayfa Yapısı

**Route**: `/projects/{projectId}/modules/{moduleId}/usecases/{useCaseId}/tasks/{taskId}`

**Breadcrumb**:
```
Projects > E-Commerce Platform > User Management > User Login > Design Login UI
```

**Layout**:
```
┌─────────────────────────────────────────────────────────┐
│ [← Back to Use Case]                                    │
│                                                          │
│ Task: Design Login UI                                   │
│ Type: [Feature] State: [In Progress]                    │
│                                                          │
│ Description:                                            │
│ Design modern login UI with email and password fields    │
│                                                          │
│ Important Notes:                                        │
│ ⚠️ Follow Material Design guidelines                    │
│                                                          │
│ Assignee: [John Doe ▼]                                  │
│ Started: 2025-01-15 | Due: 2025-02-01                   │
│                                                          │
│ State Actions:                                          │
│ [Start] [Complete] [Cancel] [Revert]                    │
│                                                          │
│ [Edit Task] [Delete Task]                               │
├─────────────────────────────────────────────────────────┤
│ Task Relations                                           │
│ [+ Add Relation]                                         │
├─────────────────────────────────────────────────────────┤
│ Type      | Related Task        | Actions                │
│───────────|─────────────────────|───────────────────────│
│ Blocks    | Write Tests         | [View] [Remove]      │
│ RelatesTo | Design Registration | [View] [Remove]       │
└─────────────────────────────────────────────────────────┘
```

### 4.2 Özellikler

#### Task Bilgileri Bölümü
- **Title**: Task başlığı
- **Type Badge**: Documentation, Feature, Test, Bug (renkli badge'ler)
- **State Badge**: NotStarted, InProgress, Completed, Cancelled (renkli badge'ler)
- **Description**: Task açıklaması
- **Important Notes**: Önemli notlar (⚠️ ikonu ile)
- **Assignee**: 
  - Dropdown ile kullanıcı seçimi
  - [Assign] butonu (yetki kontrolü: `TaskAssign`)
- **Dates**:
  - Started Date: Gösterilir (read-only)
  - Due Date: Date picker ile güncellenebilir
  - Overdue uyarısı: Due date geçmişse kırmızı uyarı gösterilir
- **State Actions**:
  - **Start**: NotStarted → InProgress (sadece NotStarted durumunda görünür)
  - **Complete**: InProgress → Completed (sadece InProgress durumunda görünür, bloklanmışsa disabled)
  - **Cancel**: NotStarted/InProgress → Cancelled (sadece NotStarted/InProgress durumunda görünür)
  - **Revert**: InProgress/Cancelled → NotStarted (sadece InProgress/Cancelled durumunda görünür)
- **Actions**:
  - [Edit Task] - Modal/form açılır
  - [Delete Task] - Onay dialogu ile silme

#### Task Relations Bölümü
- İlişkili task'ların listesi
- Her ilişki için:
  - Relation Type badge (Blocks, RelatesTo, Fixes, Duplicates)
  - Related Task title (tıklanabilir, ilgili task'a gider)
  - [View] butonu (ilgili task detayına gider)
  - [Remove] butonu (ilişkiyi kaldırır)
- **Add Relation** butonu:
  - Modal açılır
  - Target Task seçimi (dropdown veya search)
  - Relation Type seçimi (radio buttons veya dropdown)

### 4.3 API Endpoints

- `GET /api/v1/tasks/{taskId}` - Task detayı
- `PUT /api/v1/tasks/{taskId}` - Task güncelleme
- `PATCH /api/v1/tasks/{taskId}/state` - State değiştirme
- `POST /api/v1/tasks/{taskId}/assign` - Task atama
- `DELETE /api/v1/tasks/{taskId}` - Task silme
- `GET /api/v1/tasks/{taskId}/relations` - Task ilişkileri
- `POST /api/v1/tasks/{taskId}/relations` - Task ilişkisi oluşturma
- `DELETE /api/v1/tasks/{taskId}/relations/{relationId}` - Task ilişkisini silme

### 4.4 Formlar ve Modaller

#### Create/Edit Task Modal
```
┌─────────────────────────────────────┐
│ Create Task                         │
├─────────────────────────────────────┤
│ Title *                            │
│ [___________________________]       │
│                                     │
│ Type *                             │
│ ( ) Documentation                   │
│ ( ) Feature                        │
│ ( ) Test                           │
│ ( ) Bug                            │
│                                     │
│ Description                         │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ Important Notes                     │
│ [___________________________]       │
│ [Multi-line text area]              │
│                                     │
│ Due Date                           │
│ [Date Picker]                       │
│                                     │
│ [Cancel] [Create]                   │
└─────────────────────────────────────┘
```

**Validasyon**:
- Title: Zorunlu, 3-200 karakter
- Type: Zorunlu
- Description: Opsiyonel, max 2000 karakter
- Important Notes: Opsiyonel, max 1000 karakter
- Due Date: Opsiyonel, Started Date'den sonra olmalı

#### Add Task Relation Modal
```
┌─────────────────────────────────────┐
│ Add Task Relation                   │
├─────────────────────────────────────┤
│ Related Task *                     │
│ [Search/Select Task...]            │
│                                     │
│ Relation Type *                    │
│ ( ) Blocks                         │
│ ( ) RelatesTo                      │
│ ( ) Fixes                          │
│ ( ) Duplicates                     │
│                                     │
│ [Cancel] [Add Relation]             │
└─────────────────────────────────────┘
```

---

## 📄 5. Dashboard/Overview Page

### 5.1 Sayfa Yapısı

**Route**: `/dashboard` veya `/projects/{projectId}/overview`

**Layout**:
```
┌─────────────────────────────────────────────────────────┐
│ Project Overview: E-Commerce Platform                    │
├─────────────────────────────────────────────────────────┤
│ Statistics                                               │
│ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐     │
│ │ Modules  │ │ UseCases │ │  Tasks   │ │Completed │     │
│ │    5     │ │   23     │ │   87     │ │   45     │     │
│ └──────────┘ └──────────┘ └──────────┘ └──────────┘     │
├─────────────────────────────────────────────────────────┤
│ Task Status Distribution                                 │
│ [Pie Chart veya Bar Chart]                              │
│ Not Started: 20 | In Progress: 22 | Completed: 45      │
├─────────────────────────────────────────────────────────┤
│ Overdue Tasks                                            │
│ ┌──────────────────────────────────────────────────┐   │
│ │ Design Login UI        | Due: 2025-01-20 | [View]│   │
│ │ Write Tests            | Due: 2025-01-25 | [View]│   │
│ └──────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────┤
│ Upcoming Deadlines (Next 7 Days)                        │
│ ┌──────────────────────────────────────────────────┐   │
│ │ Fix Bug #123          | Due: 2025-02-01 | [View] │   │
│ │ Implement Payment     | Due: 2025-02-03 | [View] │   │
│ └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### 5.2 Özellikler

- **Statistics Cards**: 
  - Toplam Module sayısı
  - Toplam UseCase sayısı
  - Toplam Task sayısı
  - Tamamlanan Task sayısı
- **Task Status Distribution**: 
  - Görsel grafik (pie chart veya bar chart)
  - NotStarted, InProgress, Completed, Cancelled dağılımı
- **Overdue Tasks**: 
  - Gecikmiş task'ların listesi
  - Due date'i geçmiş ve Completed olmayan task'lar
- **Upcoming Deadlines**: 
  - Önümüzdeki 7 gün içinde due date'i olan task'lar
  - Tarihe göre sıralı

### 5.3 API Endpoints

- `GET /api/v1/projects/{projectId}/statistics` - İstatistikler (opsiyonel, frontend'de hesaplanabilir)
- Task listesi endpoint'lerinden veri çekilerek frontend'de hesaplanabilir

---

## 🎨 6. UI/UX Gereksinimleri

### 6.1 Tasarım Prensipleri

1. **Consistency**: Mevcut Project sayfalarıyla tutarlı tasarım
2. **Responsive**: Mobil, tablet, desktop uyumlu
3. **Accessibility**: WCAG 2.1 AA standartlarına uygun
4. **Performance**: Lazy loading, pagination, virtual scrolling

### 6.2 Renk Paleti (Task States)

- **NotStarted**: Gri (#9E9E9E)
- **InProgress**: Mavi (#2196F3)
- **Completed**: Yeşil (#4CAF50)
- **Cancelled**: Kırmızı (#F44336)

### 6.3 Renk Paleti (Task Types)

- **Documentation**: Turuncu (#FF9800)
- **Feature**: Mavi (#2196F3)
- **Test**: Mor (#9C27B0)
- **Bug**: Kırmızı (#F44336)

### 6.4 İkonlar

- Module: 📦
- UseCase: 📋
- Task: ✅
- TaskRelation: 🔗
- Overdue: ⚠️
- Important Notes: ⚠️

### 6.5 Animasyonlar

- State değişikliklerinde smooth transition
- Drag & drop animasyonları (Kanban view)
- Loading states (skeleton loaders)

---

## 🔐 7. Yetkilendirme Kontrolleri

### 7.1 Permission Kontrolü

Frontend'de JWT token'dan permission'ları alıp kontrol et:

```typescript
// Permission kontrolü örneği
const hasPermission = (permission: string): boolean => {
  const token = localStorage.getItem('accessToken');
  const decoded = jwt.decode(token);
  const permissions = decoded['permission'] || [];
  return permissions.includes(permission);
};

// Kullanım
{hasPermission('ModuleCreate') && (
  <button onClick={handleCreateModule}>Create Module</button>
)}
```

### 7.2 Permission Listesi

- `ProjectCreate`, `ProjectRead`, `ProjectUpdate`, `ProjectDelete`
- `ModuleCreate`, `ModuleRead`, `ModuleUpdate`, `ModuleDelete`
- `UseCaseCreate`, `UseCaseRead`, `UseCaseUpdate`, `UseCaseDelete`
- `TaskCreate`, `TaskRead`, `TaskUpdate`, `TaskAssign`, `TaskStateChange`, `TaskDelete`
- `TaskRelationCreate`, `TaskRelationDelete`

---

## 📱 8. Responsive Design

### 8.1 Breakpoints

- **Mobile**: < 768px
- **Tablet**: 768px - 1024px
- **Desktop**: > 1024px

### 8.2 Mobile Optimizasyonları

- Hamburger menu
- Collapsible sections
- Touch-friendly buttons (min 44x44px)
- Swipe gestures (opsiyonel)

---

## 🧪 9. Test Senaryoları

### 9.1 Unit Tests

- Form validasyonları
- Permission kontrolleri
- State management

### 9.2 Integration Tests

- API çağrıları
- Error handling
- Loading states

### 9.3 E2E Tests

- Kullanıcı akışları:
  1. Project → Module oluşturma
  2. Module → UseCase oluşturma
  3. UseCase → Task oluşturma
  4. Task state değiştirme
  5. Task atama
  6. Task relation ekleme

---

## 📝 10. Notlar

1. **Backend Hazır Olmadan**: Mock data ile UI geliştirilebilir
2. **API Entegrasyonu**: Backend hazır olduğunda API servisleri entegre edilecek
3. **Error Handling**: Tüm API çağrılarında error handling yapılmalı
4. **Loading States**: Tüm async işlemlerde loading indicator gösterilmeli
5. **Optimistic Updates**: Mümkün olduğunca optimistic update kullanılmalı (UX için)

---

## 🚀 11. Geliştirme Öncelik Sırası

1. **Faz 1**: Projects List Page
2. **Faz 2**: Project Detail Page (Project bilgileri + Module listesi)
3. **Faz 3**: Module Detail Page
4. **Faz 4**: UseCase Detail Page (List View)
5. **Faz 5**: Task Detail Page
6. **Faz 6**: Kanban View (UseCase Detail)
7. **Faz 7**: Dashboard/Overview Page
8. **Faz 8**: Task Relations yönetimi

---

## 📚 12. Referanslar

- Mevcut Project sayfaları (tutarlılık için)
- API dokümantasyonu (Swagger)
- Design system (varsa)

