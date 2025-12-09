# Mobile Development

Bu klasör Android mobile uygulaması için ayrılmıştır.

## 📋 Görevler

- [ ] Android projesi oluştur (Kotlin/Java)
- [ ] UI/UX tasarımı (Material Design)
- [ ] Backend API entegrasyonu
- [ ] Authentication sistemi
- [ ] Local storage
- [ ] Testing setup
- [ ] Build & deployment
- [ ] Play Store hazırlığı

## 🚀 Başlangıç

### 1. Android Studio Kurulumu
- Android Studio indir ve kur
- Android SDK kurulumu
- Emulator veya fiziksel cihaz hazırla

### 2. Proje Oluşturma
```bash
# Android Studio'da yeni proje oluştur
# Template: Empty Activity
# Language: Kotlin
# Minimum SDK: API 21 (Android 5.0)
```

### 3. Geliştirme Ortamı
- Android Studio IDE
- Kotlin/Java
- Gradle build system

## 🔗 Backend API Entegrasyonu

Backend API şu adreste çalışıyor:
- **URL**: https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

### Authentication
```kotlin
// API base URL
val API_BASE_URL = "https://localhost:5001"

// Login örneği (Retrofit ile)
interface AuthService {
    @POST("api/v1/auth/login")
    suspend fun login(@Body loginRequest: LoginRequest): Response<LoginResponse>
}

// Token Storage (SharedPreferences ile)
class TokenManager(private val context: Context) {
    private val prefs = context.getSharedPreferences("auth", Context.MODE_PRIVATE)
    
    fun saveToken(token: String) {
        prefs.edit().putString("auth_token", token).apply()
    }
    
    fun getToken(): String? {
        return prefs.getString("auth_token", null)
    }
}
```

## 📚 Dokümantasyon

Detaylı API dokümantasyonu için:
- [Ana README](../README.md)
- [Geliştirme Rehberi](../docs/DEVELOPMENT.md)
- [AI Entegrasyonu](../docs/AI_INTEGRATION.md)

## 🛠️ Önerilen Teknolojiler

- **Language**: Kotlin (önerilen) veya Java
- **Architecture**: MVVM veya Clean Architecture
- **UI Framework**: Jetpack Compose veya XML Layouts
- **Networking**: Retrofit + OkHttp
- **Image Loading**: Glide veya Coil
- **Database**: Room (SQLite)
- **Dependency Injection**: Hilt veya Dagger
- **Navigation**: Navigation Component
- **Testing**: JUnit + Mockito + Espresso

## 📱 Platform Özellikleri

### Android
- Minimum SDK: 21 (Android 5.0)
- Target SDK: 34 (Android 14)
- Material Design 3
- Support: Phone ve Tablet

## 🎨 UI/UX Önerileri

- Material Design 3 guidelines
- Native Android look & feel
- Touch-friendly interface
- Accessibility desteği
- Loading states ve error handling
- Offline support

## 🔧 Development Setup

1. Android Studio kurulumu
2. Android SDK kurulumu
3. Emulator veya fiziksel cihaz
4. Git repository clone

## 📦 Build & Deployment

### Debug Build
```bash
# Android Studio'da Run/Debug
# veya command line
./gradlew assembleDebug
```

### Release Build
```bash
# Release APK
./gradlew assembleRelease

# Play Store hazırlığı
./gradlew bundleRelease
```

### Play Store
- Signed APK/AAB oluştur
- Play Console'a yükle
- Store listing hazırla

İyi kodlamalar! 🚀
