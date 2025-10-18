# Development Documentation

Bu klasör TE4IT projesi için geliştirme dokümantasyonunu içerir.

## 📁 **Dosya Yapısı**

### **GIT_BRANCHING.md**
- GitHub ortak kullanım kılavuzu
- Branch stratejisi ve workflow
- Developer bazlı çalışma rehberi
- Conflict çözümü ve best practices

### **DEVELOPMENT.md**
- Geliştirme standartları ve kuralları
- Kod formatı ve best practices
- Test stratejisi
- Debugging ve troubleshooting

---

## 🎯 **Hızlı Başlangıç**

### **1. GitHub Kullanımı**
```bash
# Repository'yi klonla
git clone https://github.com/burhanarslanbas/TE4IT.git
cd TE4IT

# Develop branch'e geç
git checkout develop
git pull origin develop

# Kendi feature branch'ini oluştur
git checkout -b feature/your-name-initial-work
```

### **2. Backend Geliştirme**
```bash
cd src/TE4IT.API
dotnet restore
dotnet run
```

### **3. API Test**
- Swagger: `https://localhost:7001/swagger`
- Postman collection kullan

---

## 👥 **Developer Rolleri**

### **Backend Developer**
- **Çalışma Alanı**: `src/` klasörü
- **Branch Pattern**: `feature/ahmet-*`
- **Teknolojiler**: .NET 9, PostgreSQL, CQRS

### **Frontend Developer** (Coming Soon)
- **Çalışma Alanı**: `frontend/` klasörü
- **Branch Pattern**: `feature/mehmet-*`
- **Teknolojiler**: React, TypeScript, Tailwind

### **Mobile Developer** (Coming Soon)
- **Çalışma Alanı**: `mobile/` klasörü
- **Branch Pattern**: `feature/ayse-*`
- **Teknolojiler**: React Native, TypeScript

### **AI Developer** (Coming Soon)
- **Çalışma Alanı**: `ai-service/` klasörü
- **Branch Pattern**: `feature/elif-*`
- **Teknolojiler**: FastAPI, Python, ML

---

## 🔄 **Workflow Özeti**

### **Günlük Rutin**
1. **Sabah**: `git checkout develop && git pull origin develop`
2. **Çalışma**: Kendi feature branch'inde çalış
3. **Commit**: Düzenli commit yap
4. **Akşam**: `git push origin feature/your-branch`

### **Pull Request Süreci**
1. Feature tamamlandığında PR oluştur
2. Code review bekle
3. Approval sonrası merge et
4. Branch'i temizle

---

## 📚 **Önemli Linkler**

- **GitHub Repository**: https://github.com/burhanarslanbas/TE4IT
- **API Documentation**: https://localhost:7001/swagger
- **Project Management**: GitHub Projects
- **Issue Tracking**: GitHub Issues

---

## 🆘 **Yardım**

### **Sık Sorulan Sorular**
- **Q: Hangi branch'te çalışmalıyım?**
  A: Kendi feature branch'inde (`feature/your-name-*`)

- **Q: Conflict nasıl çözerim?**
  A: `git pull origin develop` sonra manuel çöz

- **Q: API'yi nasıl test ederim?**
  A: Swagger UI kullan veya Postman collection

### **İletişim**
- GitHub Issues
- GitHub Discussions
- Team meetings
- Code review sessions

---

**🎉 Bu dokümantasyonu takip ederek projeye katkıda bulunabilirsiniz!**