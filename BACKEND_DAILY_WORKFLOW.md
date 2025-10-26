# 🚀 Backend Günlük Geliştirme Planı

**Proje:** TE4IT - Task Management Platform  
**Developer:** Burhan Arslanbaş  
**Workflow:** `backend` → `develop` → `main`  
**Tarih:** $(Get-Date -Format "dd.MM.yyyy")

---

## 📋 Günlük Geliştirme Workflow'u

### **🔄 Git Branch Stratejisi**
```
backend (Geliştirme) → develop (Test) → main (Production)
```

**Açıklama:**
- **`backend`**: Aktif geliştirme branch'i
- **`develop`**: Test ve entegrasyon branch'i  
- **`main`**: Production branch'i (Azure'a otomatik deploy)

---

## 🛠️ Günlük Komutlar

### **1. 🔧 Geliştirme Başlangıcı**

```bash
# Backend branch'ine geç
git checkout backend

# Son değişiklikleri çek
git pull origin backend

# Mevcut durumu kontrol et
git status
```

### **2. 💻 Kod Geliştirme**

```bash
# Değişikliklerinizi yapın
# ... kod yazma ...

# Değişiklikleri kontrol et
git diff

# Tüm değişiklikleri ekle
git add .

# Commit mesajı ile kaydet
git commit -m "feat: Yeni özellik açıklaması"

# Backend branch'ini push et
git push origin backend
```

### **3. 🧪 Develop'a Merge**

```bash
# Develop branch'ine geç
git checkout develop

# Backend'den develop'a merge et
git merge backend

# Develop'ı push et
git push origin develop
```

### **4. 🚀 Main'e Merge**

```bash
# Main branch'ine geç
git checkout main

# Develop'dan main'e merge et
git merge develop

# Main'i push et (Azure'a otomatik deploy)
git push origin main
```

---

## 📝 Commit Mesaj Formatları

### **🎯 Önerilen Format**
```bash
git commit -m "type: Kısa açıklama"

# Örnekler:
git commit -m "feat: Add user profile API endpoint"
git commit -m "fix: Resolve authentication token issue"
git commit -m "docs: Update API documentation"
git commit -m "refactor: Improve email service structure"
```

### **📋 Commit Types**
- **`feat`**: Yeni özellik
- **`fix`**: Bug düzeltmesi
- **`docs`**: Dokümantasyon
- **`refactor`**: Kod yeniden düzenleme
- **`test`**: Test ekleme/düzeltme
- **`chore`**: Genel bakım

---

## 🔍 Günlük Kontrol Listesi

### **✅ Geliştirme Öncesi**
- [ ] `backend` branch'inde olduğunuzdan emin olun
- [ ] Son değişiklikleri pull edin
- [ ] Çalışacağınız konuyu belirleyin

### **✅ Geliştirme Sırasında**
- [ ] Kod değişikliklerini yapın
- [ ] Değişiklikleri test edin
- [ ] Commit mesajını yazın
- [ ] Backend'e push edin

### **✅ Geliştirme Sonrası**
- [ ] Develop'a merge edin
- [ ] Main'e merge edin
- [ ] Azure deployment'ı kontrol edin

---

## 🚨 Hata Durumları

### **❌ Merge Conflict**
```bash
# Conflict'i çöz
git status
# Dosyaları düzenle
git add .
git commit -m "resolve: Merge conflict resolved"
```

### **❌ Push Hatası**
```bash
# Force push (dikkatli kullanın!)
git push origin backend --force

# Veya önce pull edin
git pull origin backend
git push origin backend
```

### **❌ Yanlış Branch**
```bash
# Hangi branch'te olduğunuzu kontrol edin
git branch

# Doğru branch'e geçin
git checkout backend
```

---

## 📊 Günlük Raporlama

### **📈 Günlük İlerleme**
```bash
# Bugün yapılan commit'leri görün
git log --oneline --since="1 day ago"

# Branch durumunu kontrol edin
git branch -v

# Remote ile senkronizasyonu kontrol edin
git status
```

### **📋 Haftalık Özet**
```bash
# Bu hafta yapılan tüm değişiklikleri görün
git log --oneline --since="1 week ago"

# Branch'ler arası farkları kontrol edin
git log backend..develop
git log develop..main
```

---

## 🎯 Proje Özel Notlar

### **🔧 Backend Teknolojileri**
- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL (Supabase)
- **Authentication**: JWT + Identity
- **Email**: SMTP (Gmail)
- **Deployment**: Azure App Service

### **📁 Önemli Dosyalar**
```
src/TE4IT.API/Controllers/     # API Controllers
src/TE4IT.Application/         # Business Logic
src/TE4IT.Infrastructure/     # External Services
src/TE4IT.Domain/             # Domain Models
```

### **🌐 Environment Variables**
```bash
# Development
ASPNETCORE_ENVIRONMENT=Development
FrontendUrl=http://localhost:3000

# Production (Azure)
ASPNETCORE_ENVIRONMENT=Production
FRONTEND_URL=https://te4it-frontend.up.railway.app
```

---

## 🚀 Hızlı Başlangıç Komutları

### **⚡ Tek Satırda Tüm Workflow**
```bash
# Backend'de çalış ve push et
git checkout backend && git add . && git commit -m "feat: Değişiklik" && git push origin backend

# Develop'a merge et
git checkout develop && git merge backend && git push origin develop

# Main'e merge et
git checkout main && git merge develop && git push origin main
```

### **🔄 Hızlı Sync**
```bash
# Tüm branch'leri güncelle
git checkout backend && git pull origin backend
git checkout develop && git pull origin develop  
git checkout main && git pull origin main
```

---

## 📞 Acil Durumlar

### **🚨 Production Hatası**
```bash
# Hemen main'e geç
git checkout main

# Son stable commit'e dön
git log --oneline -5
git reset --hard <stable-commit-hash>

# Force push (dikkatli!)
git push origin main --force
```

### **🔧 Hotfix**
```bash
# Main'den hotfix branch oluştur
git checkout main
git checkout -b hotfix/urgent-fix

# Düzeltmeyi yap ve merge et
git add . && git commit -m "hotfix: Critical bug fix"
git checkout main && git merge hotfix/urgent-fix
git push origin main
```

---

## 📚 Faydalı Kaynaklar

### **🔗 Proje Dokümantasyonu**
- `AZURE_DEPLOYMENT_GUIDE.md` - Azure deployment rehberi
- `CHANGELOG_ENVIRONMENT_CONFIG.md` - Environment değişiklikleri
- `docs/` - Proje dokümantasyonu

### **🌐 External Links**
- [Azure Portal](https://portal.azure.com)
- [GitHub Repository](https://github.com/burhanarslanbas/TE4IT)
- [Swagger API Docs](https://te4it-api.azurewebsites.net/swagger)

---

## 📝 Notlar

### **💡 Geliştirme İpuçları**
- Her commit'ten önce kodu test edin
- Commit mesajlarını açıklayıcı yazın
- Büyük değişiklikleri küçük parçalara bölün
- Environment variable'ları doğru ayarlayın

### **⚠️ Dikkat Edilecekler**
- `main` branch'e direkt commit yapmayın
- Production environment variable'larını değiştirmeyin
- Database migration'ları dikkatli yapın
- Email template'lerini test edin

---

**📅 Son Güncelleme:** $(Get-Date -Format "dd.MM.yyyy HH:mm")  
**👨‍💻 Developer:** Burhan Arslanbaş  
**🚀 Status:** Aktif Geliştirme

---

*Bu doküman günlük geliştirme sürecinizi kolaylaştırmak için hazırlanmıştır. Sorularınız için iletişime geçebilirsiniz.*
