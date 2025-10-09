# Git Branching Strategy

Bu dokümantasyon TE4IT projesi için Git dal stratejisini açıklar.

## 🌳 Dal Yapısı

### Ana Dallar
- **`main`** - Production-ready kod
- **`develop`** - Integration branch (tüm özelliklerin birleştiği yer)

### Feature Dalları
- **`feature/backend-*`** - Backend özellikleri
- **`feature/frontend-*`** - Frontend özellikleri  
- **`feature/mobile-*`** - Mobile özellikleri
- **`feature/ai-*`** - AI service özellikleri

### Diğer Dallar
- **`bugfix/*`** - Bug düzeltmeleri
- **`hotfix/*`** - Kritik production düzeltmeleri
- **`release/*`** - Release hazırlığı

## 🚀 Başlangıç Kurulumu

### 1. Repository Clone
```bash
git clone https://github.com/kullanici-adi/TE4IT.git
cd TE4IT
```

### 2. Develop Branch Oluştur
```bash
git checkout -b develop
git push -u origin develop
```

## 👥 Takım Çalışması

### Her Developer İçin:
1. **Kendi feature branch'ini oluştur**
2. **Sadece kendi klasöründe çalış**
3. **Düzenli commit yap**
4. **Pull request oluştur**

### Branch İsimlendirme:
```bash
# Backend developer
feature/backend-auth-improvements
feature/backend-project-crud

# Frontend developer  
feature/frontend-login-page
feature/frontend-dashboard

# Mobile developer
feature/mobile-auth-flow
feature/mobile-project-list

# AI developer
feature/ai-project-analysis
feature/ai-task-estimation
```

## 🔄 Workflow

### 1. Yeni Özellik Geliştirme
```bash
# Develop'dan yeni branch oluştur
git checkout develop
git pull origin develop
git checkout -b feature/backend-new-feature

# Çalış...
git add .
git commit -m "feat: add new authentication feature"

# Push et
git push -u origin feature/backend-new-feature
```

### 2. Pull Request Oluşturma
1. GitHub'da Pull Request oluştur
2. `develop` branch'e merge et
3. Code review yap
4. Merge et

### 3. Release Hazırlığı
```bash
# Release branch oluştur
git checkout develop
git checkout -b release/v1.0.0

# Release hazırlığı
# Version bump, changelog, etc.

# Main'e merge et
git checkout main
git merge release/v1.0.0
git tag v1.0.0
git push origin main --tags

# Develop'a geri merge et
git checkout develop
git merge release/v1.0.0
git push origin develop
```

## 📁 Klasör Bazlı Çalışma

### Backend Developer
- Sadece `src/` klasöründe çalış
- `feature/backend-*` branch'leri kullan

### Frontend Developer  
- Sadece `frontend/` klasöründe çalış
- `feature/frontend-*` branch'leri kullan

### Mobile Developer
- Sadece `mobile/` klasöründe çalış
- `feature/mobile-*` branch'leri kullan

### AI Developer
- Sadece `ai-service/` klasöründe çalış
- `feature/ai-*` branch'leri kullan

## 🔒 Koruma Kuralları

### Branch Protection
- `main` branch'e direkt push yasak
- `develop` branch'e direkt push yasak
- Pull request zorunlu
- Code review zorunlu

### Commit Mesajları
```
feat: yeni özellik
fix: bug düzeltmesi
docs: dokümantasyon
style: kod formatı
refactor: kod yeniden düzenleme
test: test ekleme
chore: build, config değişiklikleri
```

## 🚨 Conflict Resolution

### Merge Conflict Çözümü
1. `git pull origin develop` ile güncel develop'ı al
2. Conflict'leri çöz
3. `git add .` ile çözümleri ekle
4. `git commit` ile commit et
5. Push et

## 📊 Branch Status

### Aktif Branches
- `main` - Production
- `develop` - Integration
- `feature/backend-auth` - Backend authentication
- `feature/frontend-login` - Frontend login page
- `feature/mobile-auth` - Mobile authentication
- `feature/ai-analysis` - AI project analysis

### Tamamlanan Branches
- `feature/backend-setup` ✅
- `feature/backend-role-seeding` ✅

## 🎯 Takım Rolleri

### Repository Admin
- Branch protection kuralları
- Merge permissions
- Release management

### Code Reviewers
- Her developer kendi alanında reviewer
- Cross-review önerilen

### Release Manager
- Version management
- Release notes
- Tagging

## 📈 Progress Tracking

### GitHub Projects
- Backend tasks
- Frontend tasks  
- Mobile tasks
- AI tasks
- Bug fixes
- Documentation

### Milestones
- v1.0.0 - MVP Release
- v1.1.0 - Feature Updates
- v1.2.0 - Performance Improvements

## 🔧 Git Hooks

### Pre-commit Hooks
- Code formatting
- Linting
- Test running

### Pre-push Hooks
- Build verification
- Test suite

## 📚 Kaynaklar

- [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/)
- [GitHub Flow](https://docs.github.com/en/get-started/quickstart/github-flow)
- [Conventional Commits](https://www.conventionalcommits.org/)

## 🆘 Yardım

### Sık Sorulan Sorular
- **Q: Hangi branch'te çalışmalıyım?**
  A: Kendi feature branch'inde (`feature/your-name-*`)

- **Q: Ne zaman merge etmeliyim?**
  A: Özellik tamamlandığında ve test edildiğinde

- **Q: Conflict nasıl çözerim?**
  A: `git pull origin develop` sonra conflict'leri manuel çöz

### İletişim
- GitHub Issues kullan
- Slack/Discord kanalları
- Haftalık sync meetings
