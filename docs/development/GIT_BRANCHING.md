# TE4IT GitHub Kullanım Kılavuzu

Bu dokümantasyon TE4IT projesi için GitHub ortak kullanımı ve Git workflow'unu detaylı olarak açıklar.

## 🎯 **Branch Stratejisi**

### **Ana Dallar**
- **`main`** - Production-ready kod (sadece release'lerde güncellenir)
- **`develop`** - Integration branch (tüm özelliklerin birleştiği yer)

### **Feature Branch Stratejisi**
- **`feature/your-name-*`** - Her developer kendi feature branch'inde çalışır
- **Güvenlik**: Ana kod korunur, hatalı kod direkt develop'a girmez
- **Code Review**: Her değişiklik review edilir
- **Profesyonel**: Industry standard workflow

---

## 🎯 **Neden Feature Branch Yaklaşımı?**

### **✅ Avantajları**
- **Kod Güvenliği**: Ana kod korunur, hatalı kod direkt develop'a girmez
- **Code Review**: Her değişiklik review edilir, kod kalitesi artar
- **Rollback**: Hatalı kod kolayca geri alınabilir
- **Collaboration**: Takım üyeleri birbirinin kodunu görebilir
- **Learning**: GitHub'ın tüm özelliklerini öğrenirsiniz
- **Professional**: Industry standard workflow
- **Portfolio**: GitHub profiliniz daha profesyonel görünür

### **⚠️ Dezavantajları**
- **Biraz Daha Karmaşık**: Direkt push'tan daha fazla adım
- **PR Süreci**: Code review bekleme süresi
- **Branch Yönetimi**: Branch'leri temizleme gerekliliği

### **🎯 Sonuç**
Küçük takım olmamıza rağmen feature branch yaklaşımını seçiyoruz çünkü:
1. **Gelecek için hazırlık** - Takım büyüdükçe sorun çıkmaz
2. **Profesyonel gelişim** - GitHub'ı tam öğrenirsiniz
3. **Kod kalitesi** - Code review ile daha iyi kod
4. **Güvenlik** - Ana kod korunur

---

## 🚀 **İlk Kurulum (Her Developer İçin)**

### **1. Repository'yi Klonlama**
```bash
# GitHub'dan projeyi klonla
git clone https://github.com/burhanarslanbas/TE4IT.git
cd TE4IT

# Mevcut branch'leri görüntüle
git branch -a
```

### **2. Develop Branch'e Geçiş**
```bash
# Develop branch'e geç
git checkout develop

# En son değişiklikleri al
git pull origin develop
```

### **3. Kendi Feature Branch'ini Oluşturma**
```bash
# Kendi feature branch'ini oluştur
git checkout -b feature/your-name-initial-work

# Örnek: feature/ahmet-backend-auth
# Örnek: feature/mehmet-frontend-login
# Örnek: feature/ayse-mobile-ui
```

### **4. Feature Branch'i GitHub'a Push Etme**
```bash
# İlk push için upstream set et
git push -u origin feature/your-name-initial-work

# Sonraki push'lar için sadece
git push origin feature/your-name-initial-work
```

---

## 👥 **Developer Bazlı Çalışma Rehberi**

### **🏗️ Backend Developer (Ahmet)**
```bash
# 1. Günlük başlangıç
git checkout develop
git pull origin develop
git checkout feature/ahmet-backend-auth

# 2. Sadece src/ klasöründe çalış
cd src/TE4IT.API
# Kodlarını yaz...

# 3. Değişiklikleri commit et
git add src/
git commit -m "feat: add user authentication endpoint"

# 4. Feature branch'e push et
git push origin feature/ahmet-backend-auth

# 5. GitHub'da Pull Request oluştur
# 6. Code review bekle
# 7. Merge edildikten sonra branch'i temizle
```

**Çalışma Alanı:** `src/` klasörü
**Branch Pattern:** `feature/ahmet-*`
**Workflow:** Feature Branch → PR → Code Review → Merge

### **🌐 Frontend Developer (Mehmet)**
```bash
# 1. Günlük başlangıç
git checkout develop
git pull origin develop
git checkout feature/mehmet-frontend-login

# 2. Sadece frontend/ klasöründe çalış
cd frontend
# Kodlarını yaz...

# 3. Değişiklikleri commit et
git add frontend/
git commit -m "feat: implement login page UI"

# 4. Feature branch'e push et
git push origin feature/mehmet-frontend-login

# 5. GitHub'da Pull Request oluştur
# 6. Code review bekle
# 7. Merge edildikten sonra branch'i temizle
```

**Çalışma Alanı:** `frontend/` klasörü
**Branch Pattern:** `feature/mehmet-*`
**Workflow:** Feature Branch → PR → Code Review → Merge

### **📱 Mobile Developer (Ayşe)**
```bash
# 1. Günlük başlangıç
git checkout develop
git pull origin develop
git checkout feature/ayse-mobile-auth

# 2. Sadece mobile/ klasöründe çalış
cd mobile
# Kodlarını yaz...

# 3. Değişiklikleri commit et
git add mobile/
git commit -m "feat: add mobile authentication flow"

# 4. Feature branch'e push et
git push origin feature/ayse-mobile-auth

# 5. GitHub'da Pull Request oluştur
# 6. Code review bekle
# 7. Merge edildikten sonra branch'i temizle
```

**Çalışma Alanı:** `mobile/` klasörü
**Branch Pattern:** `feature/ayse-*`
**Workflow:** Feature Branch → PR → Code Review → Merge

### **🤖 AI Developer (Elif)**
```bash
# 1. Günlük başlangıç
git checkout develop
git pull origin develop
git checkout feature/elif-ai-analysis

# 2. Sadece ai-service/ klasöründe çalış
cd ai-service
# Kodlarını yaz...

# 3. Değişiklikleri commit et
git add ai-service/
git commit -m "feat: implement project analysis algorithm"

# 4. Feature branch'e push et
git push origin feature/elif-ai-analysis

# 5. GitHub'da Pull Request oluştur
# 6. Code review bekle
# 7. Merge edildikten sonra branch'i temizle
```

**Çalışma Alanı:** `ai-service/` klasörü
**Branch Pattern:** `feature/elif-*`
**Workflow:** Feature Branch → PR → Code Review → Merge

---

## 🔄 **Feature Branch Workflow**

### **Sabah Rutini (Her Developer)**
```bash
# 1. En son develop'ı al
git checkout develop
git pull origin develop

# 2. Kendi feature branch'ine geç
git checkout feature/your-name-current-work

# 3. Develop'daki son değişiklikleri kendi branch'ine al
git merge develop
```

### **Çalışma Sırasında**
```bash
# Küçük değişiklikleri commit et
git add your-folder/
git commit -m "feat: add new feature"

# Büyük değişiklikleri commit et
git add your-folder/
git commit -m "feat: complete user management module"
```

### **Akşam Rutini**
```bash
# 1. Tüm değişiklikleri commit et
git add your-folder/
git commit -m "feat: daily progress update"

# 2. Feature branch'e push et
git push origin feature/your-name-current-work

# 3. GitHub'da Pull Request oluştur (eğer feature tamamlandıysa)
```

### **Feature Tamamlandığında**
```bash
# 1. Son commit'i yap
git add .
git commit -m "feat: complete feature implementation"

# 2. Push et
git push origin feature/your-name-current-work

# 3. GitHub'da Pull Request oluştur
# 4. Code review bekle
# 5. Merge edildikten sonra branch'i temizle
```

---

## 📋 **Pull Request Süreci (Detaylı)**

### **1. Pull Request Oluşturma**
```bash
# GitHub web sitesinde:
# 1. Repository'ye git
# 2. "Compare & pull request" butonuna tıkla
# 3. Base: develop, Compare: feature/your-name-*
# 4. Title ve description yaz
# 5. "Create pull request" tıkla
```

### **2. Pull Request Template**
```markdown
## 📝 Değişiklik Özeti
- [ ] Yeni özellik eklendi
- [ ] Bug düzeltildi
- [ ] Dokümantasyon güncellendi

## 🧪 Test Edildi
- [ ] Kod çalışıyor
- [ ] Test edildi
- [ ] Dokümantasyon kontrol edildi

## 📁 Değişen Dosyalar
- `src/TE4IT.API/Controllers/AuthController.cs`
- `src/TE4IT.Application/Features/Auth/...`

## 🔗 İlgili Issue
Closes #123

## 📸 Screenshots (Eğer UI değişikliği varsa)
<!-- Ekran görüntüleri ekleyin -->
```

### **3. Code Review Süreci**
1. **Otomatik Review**: GitHub Actions çalışır
2. **Manuel Review**: Takım üyeleri review yapar
3. **Approval**: En az 1 approval gerekli
4. **Merge**: "Merge pull request" tıkla

### **4. Merge Sonrası Temizlik**
```bash
# 1. Develop'a geç
git checkout develop
git pull origin develop

# 2. Feature branch'i sil
git branch -d feature/your-name-completed-feature

# 3. Remote branch'i sil
git push origin --delete feature/your-name-completed-feature
```

---

## 🚨 **Conflict Çözümü**

### **Conflict Durumu**
```bash
# Conflict olduğunda:
git pull origin develop
# Conflict mesajları görünür
```

### **Conflict Çözümü**
```bash
# 1. Conflict'li dosyaları aç
# 2. <<<<<<< HEAD ve >>>>>>> develop arasındaki kısımları düzenle
# 3. Çözümü kaydet

# 4. Çözülen dosyaları ekle
git add resolved-file.cs

# 5. Commit et
git commit -m "resolve: merge conflict in auth controller"

# 6. Push et
git push origin feature/your-name-current-work
```

---

## 📊 **Branch Yönetimi**

### **Branch İsimlendirme Kuralları**
```bash
# Feature branches
feature/ahmet-backend-auth
feature/mehmet-frontend-login
feature/ayse-mobile-ui
feature/elif-ai-analysis

# Bug fix branches
bugfix/ahmet-auth-token-issue
bugfix/mehmet-login-validation

# Hotfix branches (acil düzeltmeler)
hotfix/ahmet-security-patch
```

### **Branch Temizleme**
```bash
# Tamamlanan branch'leri sil
git branch -d feature/completed-feature
git push origin --delete feature/completed-feature
```

---

## 🔒 **Güvenlik ve Koruma**

### **Branch Protection Rules**
- **main** branch'e direkt push yasak
- **develop** branch'e direkt push yasak
- Pull request zorunlu
- Code review zorunlu

### **Commit Mesaj Kuralları**
```bash
# Format: type(scope): description

feat(auth): add JWT token refresh
fix(api): resolve pagination issue
docs(readme): update setup instructions
style(ui): fix button alignment
refactor(db): optimize user queries
test(auth): add login tests
chore(deps): update dependencies
```

---

## 📈 **Progress Tracking**

### **GitHub Issues Kullanımı**
```markdown
# Issue Template
## 🎯 Görev
- [ ] Backend: User authentication API
- [ ] Frontend: Login page UI
- [ ] Mobile: Authentication flow
- [ ] AI: Project analysis algorithm

## 📋 Acceptance Criteria
- [ ] API endpoint çalışıyor
- [ ] UI responsive
- [ ] Test coverage > 80%
- [ ] Dokümantasyon güncel

## 🔗 İlgili PR
- #123 - Backend auth implementation
- #124 - Frontend login page
```

### **Milestone Tracking**
- **v1.0.0** - MVP Release
- **v1.1.0** - Feature Updates
- **v1.2.0** - Performance Improvements

---

## 🛠️ **GitHub Features Kullanımı**

### **1. GitHub Actions (CI/CD)**
```yaml
# .github/workflows/ci.yml
name: CI
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build
      - name: Test
        run: dotnet test
```

### **2. GitHub Projects**
- **Backend Tasks** - API development
- **Frontend Tasks** - UI development
- **Mobile Tasks** - Mobile app development
- **AI Tasks** - AI service development
- **Bug Fixes** - Bug tracking
- **Documentation** - Doc updates

### **3. GitHub Discussions**
- **General** - Genel konuşmalar
- **Q&A** - Soru-cevap
- **Ideas** - Yeni fikirler
- **Show and Tell** - Paylaşımlar

---

## 🆘 **Sık Karşılaşılan Sorunlar**

### **Q: Hangi branch'te çalışmalıyım?**
**A:** Kendi feature branch'inde (`feature/your-name-*`)

### **Q: Ne zaman merge etmeliyim?**
**A:** Özellik tamamlandığında ve test edildiğinde

### **Q: Conflict nasıl çözerim?**
**A:** `git pull origin develop` sonra conflict'leri manuel çöz

### **Q: Yanlış branch'e commit ettim, ne yapmalıyım?**
**A:** 
```bash
# Son commit'i geri al
git reset --soft HEAD~1

# Doğru branch'e geç
git checkout feature/correct-branch

# Commit'i tekrar yap
git commit -m "feat: correct commit message"
```

### **Q: Commit mesajımı nasıl düzeltebilirim?**
**A:**
```bash
# Son commit mesajını düzelt
git commit --amend -m "feat: correct commit message"

# Force push (dikkatli kullan)
git push --force-with-lease origin feature/your-branch
```

---

## 📚 **GitHub Komutları Referansı**

### **Temel Komutlar**
```bash
# Repository durumu
git status
git log --oneline
git branch -a

# Değişiklik yönetimi
git add .
git add specific-file.cs
git commit -m "message"
git push origin branch-name

# Branch yönetimi
git checkout branch-name
git checkout -b new-branch
git merge source-branch
git branch -d branch-name
```

### **Gelişmiş Komutlar**
```bash
# Stash (geçici saklama)
git stash
git stash pop

# Cherry-pick (belirli commit'i al)
git cherry-pick commit-hash

# Rebase (commit geçmişini düzenle)
git rebase -i HEAD~3

# Reset (commit'leri geri al)
git reset --soft HEAD~1
git reset --hard HEAD~1
```

---

## 🎯 **Takım İletişimi**

### **GitHub Issues**
- **Bug Reports**: Hata bildirimleri
- **Feature Requests**: Özellik istekleri
- **Questions**: Sorular
- **Enhancement**: İyileştirme önerileri

### **Pull Request Comments**
- **Code Review**: Kod inceleme
- **Suggestions**: Öneriler
- **Questions**: Sorular
- **Approval**: Onay

### **GitHub Discussions**
- **General**: Genel konuşmalar
- **Q&A**: Soru-cevap
- **Ideas**: Yeni fikirler
- **Show and Tell**: Paylaşımlar

---

## 📞 **Yardım ve Destek**

### **Acil Durumlar**
- **Slack/Discord**: Hızlı iletişim
- **GitHub Issues**: Detaylı sorun bildirimi
- **Email**: team@te4it.com

### **Öğrenme Kaynakları**
- [Git Documentation](https://git-scm.com/doc)
- [GitHub Docs](https://docs.github.com/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/)

---

## 📚 **Pratik Örnekler**

### **Örnek 1: Backend Developer Yeni Özellik Ekliyor**
```bash
# 1. Güncel develop'ı al
git checkout develop
git pull origin develop

# 2. Yeni feature branch oluştur
git checkout -b feature/ahmet-user-roles

# 3. Çalış
cd src/TE4IT.API
# UserRolesController.cs oluştur
# UserRolesService.cs oluştur

# 4. Commit et
git add src/
git commit -m "feat: add user roles management"

# 5. Push et
git push -u origin feature/ahmet-user-roles

# 6. GitHub'da PR oluştur
# 7. Code review bekle
# 8. Merge edildikten sonra temizle
git checkout develop
git pull origin develop
git branch -d feature/ahmet-user-roles
git push origin --delete feature/ahmet-user-roles
```

### **Örnek 2: Frontend Developer UI Güncelliyor**
```bash
# 1. Güncel develop'ı al
git checkout develop
git pull origin develop

# 2. Yeni feature branch oluştur
git checkout -b feature/mehmet-dashboard-ui

# 3. Çalış
cd frontend
# Dashboard.tsx güncelle
# Dashboard.css ekle

# 4. Commit et
git add frontend/
git commit -m "feat: update dashboard UI with new design"

# 5. Push et
git push -u origin feature/mehmet-dashboard-ui

# 6. GitHub'da PR oluştur
# 7. Screenshot ekle PR'ye
# 8. Code review bekle
# 9. Merge edildikten sonra temizle
```

### **Örnek 3: Bug Fix**
```bash
# 1. Güncel develop'ı al
git checkout develop
git pull origin develop

# 2. Bug fix branch oluştur
git checkout -b bugfix/ayse-login-validation

# 3. Bug'ı düzelt
cd src/TE4IT.Application
# LoginCommand.cs düzelt

# 4. Commit et
git add src/
git commit -m "fix: resolve login validation issue"

# 5. Push et
git push -u origin bugfix/ayse-login-validation

# 6. GitHub'da PR oluştur
# 7. Issue'yu kapat (#123)
# 8. Code review bekle
# 9. Merge edildikten sonra temizle
```

---

## 🎓 **GitHub Öğrenme Yolculuğu**

### **Başlangıç Seviyesi**
- ✅ Repository clone
- ✅ Branch oluşturma
- ✅ Commit ve push
- ✅ Pull Request oluşturma

### **Orta Seviye**
- 🔄 Code review yapma
- 🔄 Conflict çözümü
- 🔄 Branch yönetimi
- 🔄 Issue tracking

### **İleri Seviye**
- 🚀 GitHub Actions
- 🚀 Projects ve Milestones
- 🚀 Discussions
- 🚀 Advanced Git commands

---

## 🆘 **Sık Sorulan Sorular**

### **Q: Feature branch ne kadar süre açık kalmalı?**
**A:** Mümkün olduğunca kısa. Bir özellik tamamlandığında hemen PR oluştur ve merge et.

### **Q: Birden fazla feature aynı anda geliştirebilir miyim?**
**A:** Evet, farklı feature branch'lerde çalışabilirsiniz. Ama bir seferde bir tanesine odaklanmak daha iyidir.

### **Q: Conflict olursa ne yapmalıyım?**
**A:** 
```bash
git checkout develop
git pull origin develop
git checkout feature/your-branch
git merge develop
# Conflict'leri çöz
git add .
git commit -m "resolve: merge conflicts"
```

### **Q: PR'ım reddedilirse ne yapmalıyım?**
**A:** Feedback'leri oku, gerekli değişiklikleri yap, yeni commit ekle ve tekrar push et.

### **Q: Yanlış branch'e commit ettim, ne yapmalıyım?**
**A:**
```bash
# Son commit'i geri al
git reset --soft HEAD~1

# Doğru branch'e geç
git checkout feature/correct-branch

# Commit'i tekrar yap
git commit -m "feat: correct commit message"
```

---

## 📈 **İlerleme Takibi**

### **GitHub Features Kullanımı**
- **Issues**: Task tracking
- **Projects**: Proje yönetimi
- **Milestones**: Sprint planning
- **Discussions**: Takım iletişimi
- **Actions**: CI/CD pipeline

### **Kişisel Gelişim**
- **Commit History**: Düzenli commit yapma
- **Code Review**: Kaliteli kod yazma
- **Documentation**: Dokümantasyon güncelleme
- **Testing**: Test yazma alışkanlığı

---

**🎉 Bu kılavuzu takip ederek GitHub'ı etkili bir şekilde kullanabilir ve profesyonel geliştirici olabilirsiniz!**
