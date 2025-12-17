# Development Documentation

Bu klasör TE4IT projesi için geliştirme dokümantasyonunu içerir.

## 📁 Dosya Yapısı

### 🎯 Frontend Developer Dokümantasyonu

#### **FRONTEND_USER_STORIES.md** ⭐ YENİ!
- **Kullanıcı hikayeleri bazlı akışlar**
- Proje yönetimi (oluşturma, düzenleme, silme)
- Email ile üye davet sistemi (tam akış)
- Proje üyeleri yönetimi
- Her akış için:
  - ✅ Başarı kriterleri
  - 🔄 Adım adım implementasyon
  - 💻 Hazır kod örnekleri (React/TypeScript)
  - 🎨 UI mockup'ları
  - ⚠️ Edge case'ler

#### **TASK_MANAGEMENT_FLOWS.md** ⭐ YENİ!
- **Modül, UseCase ve Task yönetimi**
- Modül oluşturma ve düzenleme
- UseCase yönetimi
- Task oluşturma (opsiyonel atama)
- Kanban board implementasyonu (Drag & Drop)
- Task durum yönetimi
- Task ilişkileri (dependencies)
- Task filtreleme ve arama
- Performance optimization pattern'leri

#### **COMMON_PATTERNS.md** ⭐ YENİ!
- **Tekrar eden pattern'ler ve best practices**
- Authentication & JWT token yönetimi
- Centralized error handling
- Type-safe API communication
- React Query integration
- Pagination implementasyonu
- Loading states & skeleton loaders
- Optimistic updates
- ✅ DO / ❌ DON'T listesi

#### **API_QUICK_REFERENCE.md** ⭐ YENİ!
- **Tüm endpoint'lerin özet listesi**
- HTTP methods ve URL'ler
- Query parameters
- Enum değerleri
- Request/Response örnekleri
- HTTP status codes
- Quick start kod örnekleri

### 📚 Backend Dokümantasyonu

#### **GIT_BRANCHING.md**
- GitHub ortak kullanım kılavuzu
- Branch stratejisi ve workflow
- Developer bazlı çalışma rehberi
- Conflict çözümü ve best practices

#### **DEVELOPMENT.md**
- Geliştirme standartları ve kuralları
- Kod formatı ve best practices
- Test stratejisi
- Debugging ve troubleshooting

#### **BACKEND_DAILY_WORKFLOW.md**
- Günlük backend development rutini
- Commit ve push stratejisi

#### **BACKEND_TASK_MANAGEMENT_IMPLEMENTATION.md**
- Task management feature implementasyonu
- Backend mimari kararları

---

## 🎯 Hızlı Başlangıç

### Frontend Developer İçin

```bash
# 1. Repository'yi klonla
git clone https://github.com/burhanarslanbas/TE4IT.git
cd TE4IT

# 2. Dokümantasyonu oku
# - FRONTEND_USER_STORIES.md (Başlangıç için ideal!)
# - TASK_MANAGEMENT_FLOWS.md (Task yönetimi için)
# - COMMON_PATTERNS.md (Best practices)
# - API_QUICK_REFERENCE.md (Hızlı referans)

# 3. Frontend kurulumu (Coming soon)
cd frontend
npm install
npm run dev
```

### Backend Developer İçin

```bash
# 1. Repository'yi klonla
git clone https://github.com/burhanarslanbas/TE4IT.git
cd TE4IT

# 2. Develop branch'e geç
git checkout develop
git pull origin develop

# 3. Backend'i çalıştır
cd src/TE4IT.API
dotnet restore
dotnet run

# 4. Swagger'a git
# https://localhost:7001/swagger
```

---

## 👥 Developer Rolleri

### **Frontend Developer**
- **Çalışma Alanı**: `frontend/` klasörü
- **Dokümantasyon**: 
  - `FRONTEND_USER_STORIES.md` (başla buradan!)
  - `TASK_MANAGEMENT_FLOWS.md`
  - `COMMON_PATTERNS.md`
  - `API_QUICK_REFERENCE.md`
- **Branch Pattern**: `feature/mehmet-*`
- **Teknolojiler**: React, TypeScript, Vite, React Query, Zustand

### **Backend Developer**
- **Çalışma Alanı**: `src/` klasörü
- **Dokümantasyon**:
  - `BACKEND_DAILY_WORKFLOW.md`
  - `DEVELOPMENT.md`
- **Branch Pattern**: `feature/ahmet-*`
- **Teknolojiler**: .NET 9, PostgreSQL, CQRS, MediatR

### **Mobile Developer** (Coming Soon)
- **Çalışma Alanı**: `mobile/` klasörü
- **Branch Pattern**: `feature/ayse-*`
- **Teknolojiler**: Kotlin (Android)

### **AI Developer** (Coming Soon)
- **Çalışma Alanı**: `ai-service/` klasörü
- **Branch Pattern**: `feature/elif-*`
- **Teknolojiler**: FastAPI, Python, ML

---

## 🔄 Workflow Özeti

### Günlük Rutin
1. **Sabah**: `git checkout develop && git pull origin develop`
2. **Çalışma**: Kendi feature branch'inde çalış
3. **Commit**: Düzenli commit yap
4. **Akşam**: `git push origin feature/your-branch`

### Pull Request Süreci
1. Feature tamamlandığında PR oluştur
2. Code review bekle
3. Approval sonrası merge et
4. Branch'i temizle

---

## 📚 Dokümantasyon Yol Haritası

### Frontend Developer İçin Önerilen Sıra:

1. **İlk Gün**: `FRONTEND_USER_STORIES.md`
   - Proje oluşturma akışını oku
   - Email davet sistemini anla
   - Kod örneklerini incele

2. **İkinci Gün**: `TASK_MANAGEMENT_FLOWS.md`
   - Modül/UseCase/Task hiyerarşisini öğren
   - Kanban board implementasyonunu incele
   - Task ilişkilerini anla

3. **Üçüncü Gün**: `COMMON_PATTERNS.md`
   - Authentication pattern'ini implement et
   - Error handling'i kur
   - React Query'yi entegre et

4. **Her Zaman**: `API_QUICK_REFERENCE.md`
   - Endpoint'leri bul
   - Hızlı örnek kodları kullan

---

## 🔍 Ne Arıyorsun?

### "Kullanıcı davet etme nasıl yapılır?"
→ `FRONTEND_USER_STORIES.md` → Bölüm 6

### "Task oluştururken atama opsiyonel mi?"
→ `TASK_MANAGEMENT_FLOWS.md` → Bölüm 8

### "Error handling nasıl yapılır?"
→ `COMMON_PATTERNS.md` → Bölüm 2

### "Hangi endpoint'i kullanmalıyım?"
→ `API_QUICK_REFERENCE.md`

### "Token nasıl yönetilir?"
→ `COMMON_PATTERNS.md` → Bölüm 1

---

## 🆘 Yardım

### Sık Sorulan Sorular

**Q: Hangi branch'te çalışmalıyım?**  
  A: Kendi feature branch'inde (`feature/your-name-*`)

**Q: Frontend dokümantasyonu nerede?**  
A: `FRONTEND_USER_STORIES.md` ile başla!

**Q: API endpoint'lerini nerede bulabilirim?**  
A: `API_QUICK_REFERENCE.md` veya Swagger (https://localhost:7001/swagger)

**Q: Kod örnekleri var mı?**  
A: Evet! Her user story dokümantasyonunda hazır kod örnekleri var (copy-paste edilebilir)

**Q: Email davet sistemi nasıl çalışıyor?**  
A: `FRONTEND_USER_STORIES.md` → Bölüm 6-7 (detaylı akış + sequence diagram)

### İletişim
- GitHub Issues
- GitHub Discussions
- Team meetings
- Code review sessions

---

## 📊 Dokümantasyon İstatistikleri

- **Toplam User Story:** 13+
- **Toplam Kod Örneği:** 50+
- **Toplam Endpoint:** 34
- **Toplam Pattern:** 10+
- **UI Mockup:** 15+

---

## 🎉 Yeni Özellikler (v2.0)

### ✨ User Story Bazlı Dokümantasyon
- Frontend developer'lar için özel hazırlandı
- Gerçek kullanım senaryoları
- Adım adım implementasyon
- Hazır kod örnekleri

### 🎨 UI Mockup'lar
- Her akış için görsel tasarım
- ASCII art ile UI gösterimi

### 🔄 Sequence Diagram'lar
- Email davet akışı
- Authentication akışı

### 📝 Best Practices
- DO/DON'T listeleri
- Common pitfalls
- Performance tips

---

**🚀 Bu dokümantasyonu takip ederek TE4IT projesine hızlıca katkıda bulunabilirsiniz!**

**💡 İpucu:** Frontend developer iseniz `FRONTEND_USER_STORIES.md` ile başlayın!
