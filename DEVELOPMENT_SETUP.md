# 🔐 Development Environment Setup

## User Secrets Kullanımı

Bu proje .NET User Secrets kullanarak hassas bilgileri güvenli bir şekilde saklar.

### User Secrets Kurulumu

```bash
cd src/TE4IT.API
dotnet user-secrets init
```

### Development Bilgilerini Ekleme

```bash
# Veritabanı bağlantısı
dotnet user-secrets set "ConnectionStrings:Pgsql" "YOUR_CONNECTION_STRING"

# JWT Signing Key
dotnet user-secrets set "Jwt:SigningKey" "YOUR_JWT_SIGNING_KEY"

# Email konfigürasyonu
dotnet user-secrets set "Email:Username" "YOUR_EMAIL_USERNAME"
dotnet user-secrets set "Email:Password" "YOUR_EMAIL_PASSWORD"
```

### Mevcut Secrets'ları Görüntüleme

```bash
dotnet user-secrets list
```

### Secrets'ları Silme

```bash
dotnet user-secrets clear
```

## Alternatif Yöntemler

### 1. Environment Variables
```bash
# Windows PowerShell
$env:CONNECTION_STRING="your_connection_string"
$env:JWT_SIGNING_KEY="your_jwt_key"

# Windows CMD
set CONNECTION_STRING=your_connection_string
set JWT_SIGNING_KEY=your_jwt_key
```

### 2. appsettings.Development.json (Önerilmez)
Sadece test amaçlı kullanın, asla production'a commit etmeyin.

## Güvenlik Notları

✅ **Yapılacaklar:**
- User Secrets kullanın
- .gitignore'da secrets dosyalarını belirtin
- Production'da environment variables kullanın

❌ **Yapılmayacaklar:**
- Hassas bilgileri kod içinde hardcode etmeyin
- appsettings.json'a gerçek şifreler yazmayın
- Secrets'ları Git'e commit etmeyin

## Railway Deployment

Production deployment için Railway dashboard'da environment variables ayarlayın:

1. Railway dashboard'a gidin
2. Projenizi seçin
3. "Variables" sekmesine gidin
4. Gerekli environment variables'ları ekleyin

Detaylı bilgi için `RAILWAY_DEPLOYMENT_GUIDE.md` dosyasına bakın.
