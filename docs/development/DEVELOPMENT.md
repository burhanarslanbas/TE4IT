# TE4IT Development Guidelines

Bu dokümantasyon TE4IT projesi için geliştirme standartları ve kurallarını açıklar.

## 🎯 **Proje Genel Bakış**

TE4IT, Clean Architecture + CQRS pattern kullanarak geliştirilen bir proje yönetim sistemidir.

## 🏗️ **Mevcut Mimari**

### **Backend (.NET 9)**
- **Framework**: .NET 9 Web API
- **Mimari**: Clean Architecture + CQRS
- **Veritabanı**: PostgreSQL (Task Modülü)
- **Kimlik Doğrulama**: JWT Bearer Tokens
- **Dokümantasyon**: Swagger/OpenAPI

### **Gelecek Modüller**
- **Frontend**: React + TypeScript (Coming Soon)
- **Mobile**: React Native (Coming Soon)
- **AI Service**: FastAPI + Python (Coming Soon)
- **Education Module**: MongoDB (Coming Soon)

---

## 🚀 **Geliştirme Ortamı Kurulumu**

### **Gereksinimler**
- .NET 9 SDK
- PostgreSQL 14+
- Git
- Visual Studio Code / Visual Studio

### **Kurulum Adımları**

1. **Repository'yi Klonlama**
   ```bash
   git clone https://github.com/burhanarslanbas/TE4IT.git
   cd TE4IT
   ```

2. **Backend Kurulumu**
   ```bash
   cd src/TE4IT.API
   dotnet restore
   
   # Veritabanı bağlantısını appsettings.Development.json'da güncelle
   dotnet ef database update --project ../TE4IT.Persistence
   dotnet run
   ```

3. **Swagger Dokümantasyonu**
   - `https://localhost:7001/swagger` adresinden API'yi test edebilirsiniz

---

## 📋 **Geliştirme Workflow**

### **Git Branch Stratejisi**
- **`main`** - Production-ready kod
- **`develop`** - Integration branch
- **`feature/your-name-*`** - Kişisel feature branch'leri

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

### **Pull Request Süreci**
1. Feature branch'inde çalış
2. Değişiklikleri commit et
3. GitHub'da Pull Request oluştur
4. Code review bekle
5. Merge et

---

## 🔧 **Kod Standartları**

### **Backend (.NET)**
- Clean Architecture prensiplerini takip et
- CQRS pattern kullan
- Proper error handling uygula
- Unit test yaz
- Dependency injection kullan
- REST API conventions'ları takip et

### **Kod Formatı**
- C# naming conventions
- XML documentation comments
- Async/await pattern
- Proper exception handling

### **Örnek Controller**
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request.Email, request.Password));
        return Ok(result);
    }
}
```

---

## 🧪 **Test Stratejisi**

### **Backend Testing**
```bash
cd src
dotnet test
```

### **Test Türleri**
- **Unit Tests**: Business logic testleri
- **Integration Tests**: API endpoint testleri
- **Repository Tests**: Data access testleri

### **Test Örneği**
```csharp
[Test]
public async Task Login_WithValidCredentials_ReturnsToken()
{
    // Arrange
    var command = new LoginCommand("test@example.com", "password");
    
    // Act
    var result = await _mediator.Send(command);
    
    // Assert
    Assert.IsNotNull(result.Token);
    Assert.IsTrue(result.Success);
}
```

---

## 📦 **Deployment**

### **Development**
- Docker Compose kullan
- Hot reload aktif
- Local PostgreSQL

### **Production**
- Docker containers
- Nginx reverse proxy
- SSL certificates
- Environment-specific configurations

---

## 🔐 **Güvenlik Kuralları**

### **Authentication**
- JWT tokens kısa expiration süresi
- Refresh token rotation
- Tüm input'ları validate et
- Production'da HTTPS kullan

### **API Security**
- Rate limiting
- CORS configuration
- Input validation
- SQL injection prevention
- XSS protection

### **Data Protection**
- Sensitive data'yı encrypt et
- Environment variables kullan
- Proper access controls
- Regular security audits

---

## 📚 **Dokümantasyon**

### **API Dokümantasyonu**
- Swagger/OpenAPI backend için
- Postman collections
- API versioning strategy

### **Kod Dokümantasyonu**
- XML comments .NET için
- README files her service için
- Architecture decision records

---

## 🐛 **Debugging**

### **Backend Debugging**
- Visual Studio debugger kullan
- Detailed logging aktif et
- Swagger ile API test et
- Database query analysis

### **Logging**
```csharp
_logger.LogInformation("User {UserId} logged in successfully", userId);
_logger.LogWarning("Failed login attempt for email {Email}", email);
_logger.LogError("Database connection failed: {Error}", ex.Message);
```

---

## 🚨 **Sık Karşılaşılan Sorunlar**

### **Backend Issues**
- Database connection problems
- JWT token validation errors
- CORS configuration issues
- Entity Framework migrations

### **Çözümler**
```bash
# Database migration
dotnet ef migrations add InitialCreate
dotnet ef database update

# NuGet package restore
dotnet restore

# Build issues
dotnet clean
dotnet build
```

---

## 📞 **Destek ve İletişim**

### **Takım İletişimi**
- GitHub Issues kullan
- GitHub Discussions
- Regular team meetings
- Code review sessions

### **Yardım Alma**
- Mevcut dokümantasyonu kontrol et
- GitHub issues'da ara
- Takım üyelerine sor
- Detaylı issue report oluştur

---

## 🔄 **Continuous Integration**

### **Otomatik Kontroller**
- Code formatting
- Linting
- Type checking
- Unit tests
- Security scanning

### **GitHub Actions**
```yaml
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

---

## 📈 **Performance Guidelines**

### **Backend Performance**
- Async/await kullan
- Database query optimization
- Caching strategies
- Connection pooling

### **API Performance**
- Pagination implement et
- Response compression
- Rate limiting
- Caching headers

---

## 🎯 **Best Practices**

### **Code Quality**
- SOLID principles
- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple, Stupid)
- YAGNI (You Aren't Gonna Need It)

### **Security**
- Input validation
- Output encoding
- Authentication & authorization
- Secure configuration

### **Maintainability**
- Clear naming
- Proper documentation
- Modular design
- Test coverage

---

**🎉 Bu kuralları takip ederek kaliteli ve sürdürülebilir kod yazabilirsiniz!**