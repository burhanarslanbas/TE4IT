# TE4IT Mobile App - Product Specification

> **Hedef Kitle:** Mobil Geliştiriciler (Android Kotlin)  
> **Versiyon:** 1.0  
> **Güncelleme:** 2024-12-08  
> **Platform:** Android (Kotlin)

## 📱 Uygulama Konsepti

TE4IT Mobile App, **üst düzey yöneticiler ve proje takipçileri** için tasarlanmış bir **monitoring ve dashboard** uygulamasıdır.

### 🎯 Ana Hedef
Yöneticilerin ve yetkililerin hareket halindeyken:
- Proje ilerlemesini takip etmesi
- Task durumlarını görmesi
- Ekip performansını izlemesi
- Kritik görevleri takip etmesi
- Bildirim alması

### ❌ Ne DEĞİLDİR
- **CRUD uygulaması değildir** - Kullanıcılar proje/modül/task **oluşturamaz**
- **Tam özellikli yönetim aracı değildir** - Detaylı düzenleme işlemleri web'de yapılır
- **Geliştirici aracı değildir** - Teknik task detayları için web kullanılır

### ✅ Nedir
- **Dashboard uygulaması** - Özet bilgiler ve metrikler
- **Monitoring tool** - Gerçek zamanlı izleme
- **Bildirim platformu** - Önemli güncellemeleri bildirme
- **Quick view** - Hızlı bilgi erişimi

---

## 🏗️ Mimari Yaklaşım

### Read-Only First Approach
```
WEB PLATFORM (Full CRUD)
└── Proje/Modül/UseCase/Task Yönetimi
└── Detaylı düzenleme
└── Ekip yönetimi
└── Raporlama

MOBILE APP (Read + Limited Actions)
└── Dashboard & Monitoring
└── İzleme ve takip
└── Bildirimler
└── Hızlı durum değişikliği (Task state)
```

### Permission Model
```
Kullanıcı Rolü          Web Yetkisi              Mobil Erişim
─────────────────────────────────────────────────────────────
Owner                   Full CRUD                Dashboard + Monitoring
Member                  Create/Edit              Dashboard + Monitoring  
Viewer                  Read Only                Dashboard + Monitoring
Admin                   System Admin             All Projects Dashboard
```

---

## 📊 Ekranlar ve Özellikler

### 1. 🏠 Dashboard (Ana Ekran)

**Amaç:** Kullanıcının genel durumu görmesi

**İçerik:**
```
┌─────────────────────────────────────┐
│  📊 Dashboard                    👤 │
├─────────────────────────────────────┤
│                                     │
│  Projelerim (5)          [Tümü →]  │
│  ┌────────────┬────────────────┐   │
│  │ E-Ticaret  │ CRM System     │   │
│  │ 🟢 Aktif   │ 🟡 Risk        │   │
│  │ 75% ██████ │ 45% ███        │   │
│  │ 12/16 task │ 9/20 task      │   │
│  └────────────┴────────────────┘   │
│                                     │
│  Görevlerim (8)          [Tümü →]  │
│  ┌─────────────────────────────┐   │
│  │ 🚀 Devam Eden (3)           │   │
│  │ 📋 Bekleyen (5)             │   │
│  │ ✅ Tamamlanan (24)          │   │
│  └─────────────────────────────┘   │
│                                     │
│  Bu Hafta                           │
│  ┌─────────────────────────────┐   │
│  │ ⏰ Bitiş Tarihi Yaklaşan: 3 │   │
│  │ 🔴 Geciken: 1               │   │
│  │ ✅ Tamamlanan: 12           │   │
│  └─────────────────────────────┘   │
│                                     │
│  Son Aktiviteler                    │
│  • Jane Login bug'ını tamamladı    │
│  • Bob yeni task ekledi            │
│  • Alice kullanıcı kayıt yaptı     │
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /projects` - Kullanıcının projelerini getir
- `GET /tasks/usecases/{useCaseId}` - Kullanıcıya atanan task'ları getir (assigneeId filtreli)
- `GET /projects/{id}/members` - Proje üyelerini getir

**Metrikler:**
- Toplam proje sayısı
- Aktif proje sayısı
- Kullanıcıya atanan task sayısı (durumlara göre)
- Bu hafta tamamlanan task sayısı
- Geciken task sayısı
- İlerleme yüzdeleri

---

### 2. 📁 Proje Listesi

**Amaç:** Kullanıcının dahil olduğu tüm projeleri görmesi

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Dashboard    Projeler            │
├─────────────────────────────────────┤
│  🔍 [Ara...]         [🔽 Filtrele]  │
├─────────────────────────────────────┤
│                                     │
│  ┌─────────────────────────────────┐│
│  │ E-Ticaret Platformu             ││
│  │ 🟢 Aktif                         ││
│  │                                  ││
│  │ 📦 Modül: 3  📋 UseCase: 12     ││
│  │ ✅ Task: 45/60 (75%)             ││
│  │ 👥 Üye: 8                        ││
│  │                                  ││
│  │ Son Güncelleme: 2 saat önce     ││
│  └─────────────────────────────────┘│
│                                     │
│  ┌─────────────────────────────────┐│
│  │ CRM System                      ││
│  │ 🟡 Risk (Gecikmeler var)        ││
│  │                                  ││
│  │ 📦 Modül: 2  📋 UseCase: 8      ││
│  │ ✅ Task: 18/40 (45%)            ││
│  │ 👥 Üye: 5                        ││
│  │ 🔴 3 geciken task               ││
│  │                                  ││
│  │ Son Güncelleme: 5 saat önce     ││
│  └─────────────────────────────────┘│
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /projects?page=1&pageSize=20&search=...&isActive=...`

**Özellikler:**
- Arama (proje adına göre)
- Filtreleme (Aktif/Pasif)
- Sıralama (En son güncellenen, Alfabetik)
- Progress bar (tamamlanma oranı)
- Risk göstergeleri (geciken task varsa)

**Tıklama:** Proje detay sayfasına git

---

### 3. 📊 Proje Detay

**Amaç:** Projenin genel durumunu ve metriklerini görmesi

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Projeler    E-Ticaret            │
├─────────────────────────────────────┤
│                                     │
│  🟢 Aktif                           │
│  Online alışveriş platformu         │
│                                     │
│  Genel İstatistikler                │
│  ┌─────────────────────────────────┐│
│  │ Modül: 3        UseCase: 12     ││
│  │ Toplam Task: 60                 ││
│  │                                  ││
│  │ Tamamlanan: 45 (75%) ████████   ││
│  │ Devam Eden: 12 (20%) ██         ││
│  │ Bekleyen: 3 (5%)                ││
│  └─────────────────────────────────┘│
│                                     │
│  📦 Modüller              [Tümü →] │
│  ┌─────────────────────────────────┐│
│  │ Kullanıcı Yönetimi              ││
│  │ 5 UseCase • 20/25 Task (80%)    ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ Ürün Kataloğu                   ││
│  │ 4 UseCase • 15/20 Task (75%)    ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ Sepet Yönetimi                  ││
│  │ 3 UseCase • 10/15 Task (67%)    ││
│  └─────────────────────────────────┘│
│                                     │
│  👥 Ekip (8 kişi)         [Tümü →] │
│  • John Doe (Owner) - 12 task      │
│  • Jane Smith (Member) - 8 task    │
│  • Bob Wilson (Member) - 10 task   │
│                                     │
│  ⏰ Yaklaşan Deadline'lar           │
│  • API entegrasyonu (2 gün)        │
│  • Test senaryoları (5 gün)        │
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /projects/{id}`
- `GET /modules/projects/{projectId}`
- `GET /projects/{projectId}/members`
- `GET /tasks/usecases/{useCaseId}?dueDateFrom=...&dueDateTo=...` (yaklaşan deadline'lar için)

**Özellikler:**
- Progress circle/bar
- Modül listesi (özet)
- Ekip listesi (özet)
- Yaklaşan deadline'lar
- Risk göstergeleri

**Navigation:**
- Modül tıklama → Modül detay
- Ekip tıklama → Ekip listesi
- "Tümü" butonları → İlgili liste sayfası

---

### 4. 📦 Modül Detay

**Amaç:** Modülün durumunu ve use case'lerini görmesi

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Proje       Kullanıcı Yönetimi   │
├─────────────────────────────────────┤
│                                     │
│  🟢 Aktif                           │
│  Kullanıcı kayıt, giriş ve profil   │
│  yönetimi işlemleri                 │
│                                     │
│  İstatistikler                      │
│  ┌─────────────────────────────────┐│
│  │ UseCase: 5                      ││
│  │ Toplam Task: 25                 ││
│  │ Tamamlanan: 20/25 (80%) ████████││
│  └─────────────────────────────────┘│
│                                     │
│  📋 Use Case'ler                    │
│  ┌─────────────────────────────────┐│
│  │ Kullanıcı Kayıt         [12 →] ││
│  │ 🟢 Aktif                        ││
│  │ 5/6 Task (83%) ███████          ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ Kullanıcı Girişi        [8 →]  ││
│  │ 🟢 Aktif                        ││
│  │ 6/8 Task (75%) ██████           ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ Şifremi Unuttum         [5 →]  ││
│  │ 🟢 Aktif                        ││
│  │ 4/5 Task (80%) ███████          ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ Profil Güncelleme       [4 →]  ││
│  │ 🟢 Aktif                        ││
│  │ 3/4 Task (75%) ██████           ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ Email Doğrulama         [2 →]  ││
│  │ 🟢 Aktif                        ││
│  │ 2/2 Task (100%) ██████████      ││
│  └─────────────────────────────────┘│
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /modules/{id}`
- `GET /usecases/modules/{moduleId}`

**Özellikler:**
- Use case listesi
- Her use case için task sayısı ve ilerleme
- Task sayısı göstergesi (badge)

**Navigation:**
- Use case tıklama → Use case detay

---

### 5. 📋 Use Case Detay

**Amaç:** Use case'in task'larını görmesi

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Modül       Kullanıcı Kayıt      │
├─────────────────────────────────────┤
│                                     │
│  🟢 Aktif                           │
│  Yeni kullanıcıların sisteme kayıt  │
│  olması için gerekli adımlar        │
│                                     │
│  ⚠️ Önemli Notlar                   │
│  • Email doğrulaması zorunlu        │
│  • Şifre min 8 karakter             │
│  • GDPR onayı alınmalı              │
│                                     │
│  İstatistikler                      │
│  ┌─────────────────────────────────┐│
│  │ Toplam Task: 6                  ││
│  │ Tamamlanan: 5/6 (83%)           ││
│  │                                  ││
│  │ ✅ Completed: 5                 ││
│  │ 🚀 In Progress: 1               ││
│  │ 📋 Not Started: 0               ││
│  └─────────────────────────────────┘│
│                                     │
│  ✅ Task'lar                        │
│  ┌─────────────────────────────────┐│
│  │ ✨ Kayıt formu tasarımı         ││
│  │ ✅ Completed                    ││
│  │ 👤 Jane Smith                   ││
│  │ 📅 Tamamlandı: 2 gün önce       ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ ✨ Backend API geliştirme       ││
│  │ 🚀 In Progress                  ││
│  │ 👤 Bob Wilson                   ││
│  │ 📅 Bitiş: 3 gün sonra           ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ 🧪 Unit test yazma              ││
│  │ ✅ Completed                    ││
│  │ 👤 Alice Johnson                ││
│  │ 📅 Tamamlandı: 1 gün önce       ││
│  └─────────────────────────────────┘│
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /usecases/{id}`
- `GET /tasks/usecases/{useCaseId}`

**Özellikler:**
- Task listesi (durumlara göre gruplu)
- Atanan kişi
- Due date bilgisi
- Task tipi ikonu (✨ Feature, 🧪 Test, 📄 Docs, 🐛 Bug)
- Durum badge'i

**Navigation:**
- Task tıklama → Task detay

---

### 6. ✅ Task Detay

**Amaç:** Task'ın tüm detaylarını görmesi ve **durum değiştirebilmesi**

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Use Case    Backend API          │
├─────────────────────────────────────┤
│                                     │
│  ✨ Feature                         │
│  🚀 In Progress                     │
│                                     │
│  Kullanıcı kayıt endpoint'i ve      │
│  servis implementasyonu             │
│                                     │
│  📝 Önemli Notlar                   │
│  • Rate limiting uygulanmalı        │
│  • Email unique olmalı              │
│  • Password hash edilmeli           │
│                                     │
│  👤 Atanan                          │
│  Bob Wilson (Member)                │
│                                     │
│  📅 Tarihler                        │
│  • Başlangıç: 5 gün önce           │
│  • Bitiş: 3 gün sonra              │
│  • Kalan süre: 3 gün               │
│                                     │
│  🔗 İlişkiler (2)                   │
│  • 🚫 Blocks: Frontend entegrasyon  │
│  • 🔗 Relates: Email servisi        │
│                                     │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  │
│                                     │
│  Durum Değiştir                     │
│  ┌─────────────────────────────────┐│
│  │ [📋 Not Started]                ││
│  │ [🚀 In Progress]  ✓             ││
│  │ [✅ Completed]                  ││
│  │ [❌ Cancelled]                  ││
│  └─────────────────────────────────┘│
│                                     │
│  [Durumu Güncelle] 🔄               │
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /tasks/{id}`
- `GET /tasks/{taskId}/relations`
- **`PATCH /tasks/{id}/state`** (Mobilde tek yazma işlemi!)

**Özellikler:**
- ✅ **Durum değiştirme** (tek aksiyonlu işlem)
- Task detayları (description, important notes)
- Atanan kişi bilgisi
- Due date ve kalan süre
- Task ilişkileri
- Progress tracking

**Aksiyonlar:**
- **Durum değiştirme butonu** (Bottom sheet ile durum seçimi)
- İlişkili task'a navigate

---

### 7. 👥 Ekip Listesi

**Amaç:** Proje ekibinin performansını görmesi

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Proje       Ekip (8 kişi)        │
├─────────────────────────────────────┤
│                                     │
│  ┌─────────────────────────────────┐│
│  │ 👤 John Doe                     ││
│  │ 👑 Owner                        ││
│  │                                  ││
│  │ ✅ Tamamlanan: 24               ││
│  │ 🚀 Devam Eden: 3                ││
│  │ 📋 Bekleyen: 5                  ││
│  │                                  ││
│  │ Bu hafta: 8 task tamamladı      ││
│  └─────────────────────────────────┘│
│                                     │
│  ┌─────────────────────────────────┐│
│  │ 👤 Jane Smith                   ││
│  │ 🔧 Member                       ││
│  │                                  ││
│  │ ✅ Tamamlanan: 18               ││
│  │ 🚀 Devam Eden: 2                ││
│  │ 📋 Bekleyen: 3                  ││
│  │                                  ││
│  │ Bu hafta: 6 task tamamladı      ││
│  └─────────────────────────────────┘│
│                                     │
│  ┌─────────────────────────────────┐│
│  │ 👤 Bob Wilson                   ││
│  │ 🔧 Member                       ││
│  │                                  ││
│  │ ✅ Tamamlanan: 15               ││
│  │ 🚀 Devam Eden: 4                ││
│  │ 📋 Bekleyen: 6                  ││
│  │ 🔴 1 geciken task               ││
│  │                                  ││
│  │ Bu hafta: 4 task tamamladı      ││
│  └─────────────────────────────────┘│
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /projects/{projectId}/members`
- `GET /tasks/usecases/{useCaseId}?assigneeId={userId}` (her üye için task sayıları)

**Özellikler:**
- Ekip üyeleri listesi
- Rol badge'i (Owner, Member, Viewer)
- Task istatistikleri (durumlara göre)
- Bu hafta tamamlanan task sayısı
- Risk göstergeleri (geciken task varsa)

**Navigation:**
- Üye tıklama → Üyenin task'larını göster (filtrelenmiş task listesi)

---

### 8. ✅ Görevlerim

**Amaç:** Kullanıcıya atanan tüm task'ları görmesi

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Dashboard    Görevlerim (12)     │
├─────────────────────────────────────┤
│  🔍 [Ara...]    [🔽 Filtre] [⚡ →]  │
├─────────────────────────────────────┤
│                                     │
│  🚀 Devam Eden (3)                  │
│  ┌─────────────────────────────────┐│
│  │ ✨ Backend API geliştirme       ││
│  │ E-Ticaret > Kullanıcı Yön.      ││
│  │ 📅 3 gün sonra                  ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ 🧪 Unit test yazma              ││
│  │ CRM > Müşteri Yön.              ││
│  │ 📅 5 gün sonra                  ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ 📄 API dokümantasyonu           ││
│  │ Blog > İçerik Yön.              ││
│  │ 📅 7 gün sonra                  ││
│  └─────────────────────────────────┘│
│                                     │
│  📋 Bekleyen (5)                    │
│  ┌─────────────────────────────────┐│
│  │ ✨ Frontend entegrasyon         ││
│  │ E-Ticaret > Kullanıcı Yön.      ││
│  │ 🚫 Blocked by: Backend API      ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ 🐛 Login bug düzeltme           ││
│  │ CRM > Auth Modül                ││
│  │ 📅 Bitiş tarihi yok             ││
│  └─────────────────────────────────┘│
│                                     │
│  ⏰ Yaklaşan Deadline'lar           │
│  • Backend API (3 gün)             │
│  • Unit test (5 gün)               │
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /tasks/usecases/{useCaseId}?assigneeId={currentUserId}` (tüm use case'ler için)
- Alternatif: Backend'e yeni endpoint eklenebilir: `GET /users/{userId}/tasks`

**Özellikler:**
- Durumlara göre gruplandırma
- Proje ve modül bilgisi (breadcrumb)
- Due date gösterimi
- Gecikmiş task vurgulaması
- Blocked task gösterimi
- Filtreleme (durum, tarih, proje)
- Sıralama (due date, proje, durum)

**Navigation:**
- Task tıklama → Task detay

---

### 9. 🔔 Bildirimler

**Amaç:** Önemli güncellemeleri görme

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Dashboard    Bildirimler (8)     │
├─────────────────────────────────────┤
│                                     │
│  Bugün                              │
│  ┌─────────────────────────────────┐│
│  │ 🔴 [Gecikme]                    ││
│  │ "Backend API" görevi gecikti    ││
│  │ E-Ticaret > Kullanıcı Yönetimi  ││
│  │ 2 saat önce                     ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ ✅ [Tamamlandı]                 ││
│  │ Jane "Login bug"'ı tamamladı    ││
│  │ CRM > Auth Modül                ││
│  │ 5 saat önce                     ││
│  └─────────────────────────────────┘│
│                                     │
│  Dün                                │
│  ┌─────────────────────────────────┐│
│  │ 👥 [Atama]                      ││
│  │ Size yeni task atandı           ││
│  │ "API dokümantasyonu"            ││
│  │ Blog > İçerik Yönetimi          ││
│  │ 1 gün önce                      ││
│  └─────────────────────────────────┘│
│  ┌─────────────────────────────────┐│
│  │ 💬 [Yorum]                      ││
│  │ Bob task'ınıza yorum yaptı      ││
│  │ "Unit test yazma"               ││
│  │ 1 gün önce                      ││
│  └─────────────────────────────────┘│
└─────────────────────────────────────┘
```

**Bildirim Tipleri:**
- ✅ Task tamamlandı
- 🚀 Task durumu değişti
- 👥 Size task atandı
- 🔴 Task gecikti
- ⏰ Deadline yaklaşıyor (2 gün kala)
- 💬 Yorum yapıldı (opsiyonel - gelecek)
- 🔗 İlişkili task güncellendi

**Implementation:**
- Firebase Cloud Messaging (FCM)
- Backend'den push notification gönderimi
- Local notification storage

---

### 10. ⚙️ Profil & Ayarlar

**Amaç:** Kullanıcı bilgileri ve uygulama ayarları

**İçerik:**
```
┌─────────────────────────────────────┐
│  ◀ Dashboard    Profil              │
├─────────────────────────────────────┤
│                                     │
│       ┌─────────────┐               │
│       │   [👤]      │               │
│       │ John Doe    │               │
│       └─────────────┘               │
│                                     │
│  john.doe@example.com               │
│  🔧 Member • Administrator          │
│                                     │
│  Hesap                              │
│  ┌─────────────────────────────────┐│
│  │ 📧 Email Değiştir           →  ││
│  │ 🔑 Şifre Değiştir           →  ││
│  └─────────────────────────────────┘│
│                                     │
│  Bildirimler                        │
│  ┌─────────────────────────────────┐│
│  │ 🔔 Push Bildirimler     [ON]   ││
│  │ ✅ Task Tamamlandı      [ON]   ││
│  │ ⏰ Deadline Uyarısı     [ON]   ││
│  │ 🔴 Gecikme Uyarısı      [ON]   ││
│  └─────────────────────────────────┘│
│                                     │
│  Uygulama                           │
│  ┌─────────────────────────────────┐│
│  │ 🌙 Karanlık Mod         [OFF]  ││
│  │ 🌐 Dil                  TR     ││
│  │ ℹ️  Hakkında            →      ││
│  │ 📄 Gizlilik             →      ││
│  └─────────────────────────────────┘│
│                                     │
│  [🚪 Çıkış Yap]                    │
└─────────────────────────────────────┘
```

**API Endpoints:**
- `GET /auth/me` - Kullanıcı bilgileri
- `POST /auth/changePassword` - Şifre değiştirme

**Özellikler:**
- Profil bilgileri
- Şifre değiştirme
- Bildirim tercihleri
- Karanlık mod
- Çıkış yapma

---

## 🎨 UI/UX Prensipleri

### Design System
- **Material Design 3** (Android)
- **Color Scheme:** 
  - Primary: #2196F3 (Blue)
  - Success: #4CAF50 (Green)
  - Warning: #FF9800 (Orange)
  - Error: #F44336 (Red)
  - In Progress: #2196F3 (Blue)

### Typography
```kotlin
Typography(
    displayLarge = TextStyle(fontSize = 28.sp, fontWeight = FontWeight.Bold),
    titleLarge = TextStyle(fontSize = 22.sp, fontWeight = FontWeight.Bold),
    bodyLarge = TextStyle(fontSize = 16.sp),
    bodyMedium = TextStyle(fontSize = 14.sp),
    labelMedium = TextStyle(fontSize = 12.sp, fontWeight = FontWeight.Medium)
)
```

### Icons
- Task Type: ✨ Feature, 🧪 Test, 📄 Docs, 🐛 Bug
- Task State: 📋 Not Started, 🚀 In Progress, ✅ Completed, ❌ Cancelled
- Project Status: 🟢 Active, 🔴 Inactive, 🟡 Risk
- Relations: 🚫 Blocks, 🔗 Relates To, 🔧 Fixes, 📋 Duplicates

### Loading States
- Shimmer effect (Facebook style)
- Progress indicators
- Skeleton loaders

### Empty States
- İllüstrasyon + mesaj
- Call-to-action (Web'e yönlendirme)

---

## 🔧 Teknik Gereksinimler

### Tech Stack
```kotlin
// Android (Kotlin)
├── Jetpack Compose (UI)
├── Material Design 3
├── Retrofit (Networking)
├── Kotlin Coroutines + Flow
├── Hilt (Dependency Injection)
├── Room (Local Cache)
├── DataStore (Preferences)
├── Firebase Cloud Messaging (Push Notifications)
├── Coil (Image Loading)
└── JUnit + Mockito (Testing)
```

### Minimum Requirements
- **Min SDK:** 24 (Android 7.0)
- **Target SDK:** 34 (Android 14)
- **Kotlin:** 1.9+
- **Gradle:** 8.0+

### Permissions
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
```

---

## 📡 API Entegrasyonu

### Base Configuration
```kotlin
object ApiConfig {
    const val BASE_URL = "https://te4it-api.azurewebsites.net"
    const val API_VERSION = "v1"
    const val TIMEOUT = 30L // seconds
}
```

### Kullanılacak Endpoint'ler

#### Authentication
```
POST   /api/v1/auth/login
POST   /api/v1/auth/register  
POST   /api/v1/auth/refreshToken
POST   /api/v1/auth/changePassword
POST   /api/v1/auth/revokeRefreshToken
```

#### Projects (Read Only)
```
GET    /api/v1/projects
GET    /api/v1/projects/{id}
GET    /api/v1/projects/{projectId}/members
```

#### Modules (Read Only)
```
GET    /api/v1/modules/projects/{projectId}
GET    /api/v1/modules/{id}
```

#### Use Cases (Read Only)
```
GET    /api/v1/usecases/modules/{moduleId}
GET    /api/v1/usecases/{id}
```

#### Tasks (Read + State Change)
```
GET    /api/v1/tasks/usecases/{useCaseId}
GET    /api/v1/tasks/{id}
PATCH  /api/v1/tasks/{id}/state        ← TEK YAZMA İŞLEMİ!
GET    /api/v1/tasks/{taskId}/relations
```

### State Change Request
```kotlin
data class ChangeTaskStateRequest(
    val newState: TaskState
)

enum class TaskState(val value: Int) {
    NOT_STARTED(1),
    IN_PROGRESS(2),
    COMPLETED(3),
    CANCELLED(4)
}
```

---

## 💾 Local Data & Caching

### Room Database Schema

```kotlin
// Entities
@Entity(tableName = "projects")
data class ProjectEntity(
    @PrimaryKey val id: String,
    val title: String,
    val description: String?,
    val isActive: Boolean,
    val startedDate: Long,
    val cachedAt: Long
)

@Entity(tableName = "tasks")
data class TaskEntity(
    @PrimaryKey val id: String,
    val useCaseId: String,
    val title: String,
    val description: String?,
    val taskType: Int,
    val taskState: Int,
    val assigneeId: String?,
    val assigneeName: String?,
    val dueDate: Long?,
    val cachedAt: Long
)

// Caching Strategy
- Cache tüm liste verilerini (projeler, task'lar)
- Cache süresi: 5 dakika
- Offline first: Önce cache'den göster, sonra API'den güncelle
- Pull to refresh ile manuel güncelleme
```

### Offline Support
- Tüm okuma işlemleri offline çalışabilir (cache'den)
- Task durum değiştirme işlemi queue'ya alınır
- İnternet geldiğinde otomatik gönderilir
- Kullanıcıya "Offline" badge göster

---

## 🔔 Push Notifications

### FCM Integration

```kotlin
class TE4ITMessagingService : FirebaseMessagingService() {
    
    override fun onMessageReceived(message: RemoteMessage) {
        val data = message.data
        
        when (data["type"]) {
            "TASK_COMPLETED" -> showTaskCompletedNotification(data)
            "TASK_ASSIGNED" -> showTaskAssignedNotification(data)
            "DEADLINE_APPROACHING" -> showDeadlineNotification(data)
            "TASK_OVERDUE" -> showOverdueNotification(data)
        }
    }
    
    private fun showTaskCompletedNotification(data: Map<String, String>) {
        val taskTitle = data["taskTitle"]
        val userName = data["userName"]
        
        val notification = NotificationCompat.Builder(this, CHANNEL_ID)
            .setSmallIcon(R.drawable.ic_notification)
            .setContentTitle("Task Tamamlandı")
            .setContentText("$userName \"$taskTitle\" görevini tamamladı")
            .setPriority(NotificationCompat.PRIORITY_DEFAULT)
            .setAutoCancel(true)
            .build()
        
        NotificationManagerCompat.from(this).notify(notificationId, notification)
    }
}
```

### Notification Types
```json
{
  "TASK_COMPLETED": {
    "title": "Task Tamamlandı",
    "body": "{user} \"{task}\" görevini tamamladı",
    "icon": "✅"
  },
  "TASK_ASSIGNED": {
    "title": "Yeni Görev",
    "body": "Size yeni task atandı: \"{task}\"",
    "icon": "👥"
  },
  "DEADLINE_APPROACHING": {
    "title": "Deadline Yaklaşıyor",
    "body": "\"{task}\" görevi {days} gün sonra bitiyor",
    "icon": "⏰"
  },
  "TASK_OVERDUE": {
    "title": "Görev Gecikti",
    "body": "\"{task}\" görevi gecikti",
    "icon": "🔴"
  }
}
```

---

## 🎯 Performans Hedefleri

### App Metrics
- **İlk ekran yüklenme:** < 2 saniye
- **Liste scroll:** 60 FPS
- **API response time:** < 500ms (backend)
- **Offline first:** Cache'den 100ms içinde göster
- **App size:** < 50 MB

### Optimization
- LazyColumn ile liste virtualization
- Image caching (Coil)
- Network call batching
- Pagination (20 items per page)
- Background sync (WorkManager)

---

## 🧪 Testing Strategy

### Unit Tests
```kotlin
@Test
fun `login should save tokens on success`() = runTest {
    // Given
    val email = "test@example.com"
    val password = "password123"
    
    // When
    val result = authRepository.login(email, password)
    
    // Then
    assertTrue(result.isSuccess)
    verify(tokenManager).saveTokens(any(), any())
}
```

### UI Tests
```kotlin
@Test
fun testProjectListDisplaysCorrectly() {
    composeTestRule.setContent {
        ProjectListScreen()
    }
    
    composeTestRule
        .onNodeWithText("E-Ticaret Platformu")
        .assertIsDisplayed()
}
```

### Test Coverage
- Unit Tests: > 80%
- Integration Tests: Key flows
- UI Tests: Critical screens

---

## 📚 Dokümantasyon Referansları

### İlgili Dokümantasyonlar
1. **MOBILE_INTEGRATION_GUIDE.md** - API entegrasyon detayları
2. **API_QUICK_REFERENCE.md** - Tüm endpoint'lerin özeti
3. **FRONTEND_USER_STORIES.md** - User story örnekleri (referans için)

### API Dokümantasyonu
- Swagger: `https://te4it-api.azurewebsites.net/swagger`
- Base URL: `https://te4it-api.azurewebsites.net/api/v1`

---

## 🚀 Development Roadmap

### Phase 1: MVP (4 hafta)
- ✅ Authentication
- ✅ Dashboard
- ✅ Proje listesi ve detay
- ✅ Modül ve use case görünümü
- ✅ Task listesi ve detay
- ✅ Task durum değiştirme

### Phase 2: Enhanced Features (2 hafta)
- ✅ Push notifications
- ✅ Offline support
- ✅ Ekip listesi ve performans
- ✅ "Görevlerim" ekranı
- ✅ Filtreleme ve arama

### Phase 3: Polish (1 hafta)
- ✅ Karanlık mod
- ✅ Animasyonlar
- ✅ Performance optimization
- ✅ Error handling improvements

### Phase 4: Future (Opsiyonel)
- 📊 Detaylı analytics
- 📈 Grafikler ve charts
- 💬 In-app messaging
- 🔍 Advanced search

---

## 🎓 Geliştirici İçin Notlar

### Önemli Hatırlatmalar

1. **Read-Only First:** Bu mobil uygulama yönetim aracı değil, **monitoring aracıdır**
2. **Single Write Action:** Tek yazma işlemi task durum değiştirme
3. **Offline Support:** Her zaman cache-first yaklaşım
4. **Performance:** Liste scroll'u 60 FPS olmalı
5. **Security:** Token'ları DataStore'da encrypted sakla

### Anti-Patterns (Yapılmaması Gerekenler)

❌ Task/Project/Module oluşturma formu ekleme  
❌ Detaylı düzenleme ekranları  
❌ Kompleks CRUD operasyonları  
❌ Web platformunun tüm özelliklerini kopyalama  

### Best Practices

✅ Dashboard-first yaklaşım  
✅ Metrik ve ilerleme göstergeleri  
✅ Hızlı bilgi erişimi  
✅ Push notification entegrasyonu  
✅ Offline-first architecture  

---

**Son Güncelleme:** 2024-12-08  
**Versiyon:** 1.0  
**Geliştirici:** Android Team  
**Durum:** Ready for Development 🚀
