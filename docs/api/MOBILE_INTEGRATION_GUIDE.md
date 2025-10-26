# Mobil Geliştiriciler İçin TE4IT API Entegrasyon Rehberi

**Sürüm:** 2.0  
**Tarih:** Ocak 2025  
**Hedef:** Android (Kotlin), iOS (Swift), React Native, Flutter  
**Not:** Bu dokümantasyon mobil platformlar için hazırlanmıştır. Web platformu için ayrı dokümantasyon mevcuttur.

---

## 📋 İçindekiler

1. [Hızlı Başlangıç](#hızlı-başlangıç)
2. [Auth Endpoint'leri Rehberi](#auth-endpointleri-rehberi)
3. [Android (Kotlin) Entegrasyonu](#android-kotlin-entegrasyonu)
4. [iOS (Swift) Entegrasyonu](#ios-swift-entegrasyonu)
5. [React Native Entegrasyonu](#react-native-entegrasyonu)
6. [Şifre Değiştirme (Sadece Mobil)](#şifre-değiştirme-sadece-mobil)
7. [Güvenlik ve Token Yönetimi](#güvenlik-ve-token-yönetimi)
8. [Test Senaryoları](#test-senaryoları)

---

## 🚀 Hızlı Başlangıç

### **Mobil Uygulamalar İçin Önemli Notlar:**
- **Şifre Sıfırlama**: Mobil uygulamalarda `forgotPassword` ve `resetPassword` kullanılmaz
- **Email Link'leri**: Sadece web platformuna yönlendirir
- **Şifre Değiştirme**: Sadece `changePassword` endpoint'i kullanılır
- **Kullanıcı Deneyimi**: Şifremi unuttum durumunda kullanıcı web'e yönlendirilir

### 1. API Base URL'leri

```kotlin
// Android - Constants.kt
object ApiConstants {
    const val BASE_URL = "https://te4it-api.azurewebsites.net"
    const val DEV_BASE_URL = "https://localhost:5001"
    
    val CURRENT_BASE_URL = if (BuildConfig.DEBUG) DEV_BASE_URL else BASE_URL
    
    const val AUTH_ENDPOINT = "/api/v1/auth"
    const val PROJECTS_ENDPOINT = "/api/v1/projects"
    const val TASKS_ENDPOINT = "/api/v1/tasks"
}
```

```swift
// iOS - Constants.swift
struct ApiConstants {
    static let baseURL = "https://te4it-api.azurewebsites.net"
    static let devBaseURL = "https://localhost:5001"
    
    static var currentBaseURL: String {
        #if DEBUG
        return devBaseURL
        #else
        return baseURL
        #endif
    }
    
    static let authEndpoint = "/api/v1/auth"
    static let projectsEndpoint = "/api/v1/projects"
    static let tasksEndpoint = "/api/v1/tasks"
}
```

---

## 🔐 Auth Endpoint'leri Rehberi

### **1. Kullanıcı Kaydı (Register)**

#### **Ne Yapmalısınız:**
- Kullanıcıdan `userName`, `email`, `password` bilgilerini isteyin
- Form validasyonu yapın (email formatı, şifre güçlülüğü)
- `POST /api/v1/auth/register` endpoint'ini çağırın
- Başarılı olursa kullanıcıyı otomatik giriş yapın
- Token'ları güvenli storage'a kaydedin (Android: DataStore, iOS: Keychain)
- Dashboard ekranına yönlendirin

#### **Request Format:**
```json
{
  "userName": "johndoe",
  "email": "john@example.com", 
  "password": "SecurePass123!"
}
```

#### **Response Format:**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "userName": "johndoe",
  "email": "john@example.com",
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-13T15:30:00Z",
  "refreshToken": "refresh_token_here",
  "refreshExpires": "2025-01-20T15:30:00Z"
}
```

#### **Hata Durumları:**
- **400 Bad Request**: Email zaten kayıtlı, şifre çok zayıf
- **422 Validation Error**: Form validasyon hataları
- **500 Internal Server Error**: Sunucu hatası

---

### **2. Kullanıcı Girişi (Login)**

#### **Ne Yapmalısınız:**
- Kullanıcıdan `email` ve `password` bilgilerini isteyin
- Form validasyonu yapın (email formatı, şifre boş değil)
- `POST /api/v1/auth/login` endpoint'ini çağırın
- Başarılı olursa token'ları güvenli storage'a kaydedin
- Dashboard ekranına yönlendirin

#### **Request Format:**
```json
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

#### **Response Format:**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "email": "john@example.com",
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-13T15:30:00Z",
  "refreshToken": "refresh_token_here",
  "refreshExpires": "2025-01-20T15:30:00Z"
}
```

#### **Hata Durumları:**
- **401 Unauthorized**: Yanlış email/şifre
- **400 Bad Request**: Geçersiz form verisi
- **429 Too Many Requests**: Çok fazla deneme

---

### **3. Token Yenileme (Refresh Token)**

#### **Ne Yapmalısınız:**
- Access token süresi dolduğunda otomatik olarak çağırın
- `refreshToken`'ı güvenli storage'dan alın
- `POST /api/v1/auth/refreshToken` endpoint'ini çağırın
- Yeni token'ları güvenli storage'a kaydedin
- Orijinal isteği yeni token ile tekrar gönderin

#### **Request Format:**
```json
{
  "refreshToken": "your_refresh_token_here"
}
```

#### **Response Format:**
```json
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-13T16:30:00Z",
  "refreshToken": "new_refresh_token_here",
  "refreshExpires": "2025-01-20T16:30:00Z"
}
```

#### **Hata Durumları:**
- **401 Unauthorized**: Refresh token geçersiz/süresi dolmuş
- **400 Bad Request**: Geçersiz refresh token

---

### **4. Çıkış Yapma (Logout)**

#### **Ne Yapmalısınız:**
- `refreshToken`'ı güvenli storage'dan alın
- `POST /api/v1/auth/revokeRefreshToken` endpoint'ini çağırın
- Authorization header'ında access token gönderin
- Token'ları güvenli storage'dan silin
- Login ekranına yönlendirin

#### **Request Format:**
```json
{
  "refreshToken": "refresh_token_to_revoke"
}
```

#### **Response Format:**
- **204 No Content**: Başarılı çıkış

#### **Hata Durumları:**
- **401 Unauthorized**: Geçersiz access token
- **400 Bad Request**: Geçersiz refresh token

---

### **5. Uygulama İçi Şifre Değiştirme (Change Password) - SADECE MOBİL**

#### **Ne Yapmalısınız:**
- Kullanıcının giriş yapmış olması gerekir
- Authorization header'ında access token gönderin
- Kullanıcıdan `currentPassword` ve `newPassword` isteyin
- Şifre validasyonu yapın (en az 8 karakter, büyük/küçük harf, rakam, özel karakter)
- `POST /api/v1/auth/changePassword` endpoint'ini çağırın
- Başarılı olursa "Şifreniz başarıyla güncellendi" toast mesajı gösterin

#### **Request Format:**
```json
{
  "currentPassword": "CurrentPass123!",
  "newPassword": "NewSecurePass456!"
}
```

#### **Response Format:**
```json
{
  "success": true,
  "message": "Şifreniz başarıyla güncellendi"
}
```

#### **Hata Durumları:**
- **401 Unauthorized**: Geçersiz access token
- **400 Bad Request**: Mevcut şifre yanlış, yeni şifre çok zayıf
- **422 Validation Error**: Şifre validasyon hataları

---

### **6. Şifremi Unuttum (Forgot Password) - MOBİLDE KULLANILMAZ**

#### **Mobil Uygulamalarda Ne Yapmalısınız:**
- **Şifremi Unuttum** butonuna tıklandığında kullanıcıyı web'e yönlendirin
- Web URL'i: `https://te4it-frontend.azurestaticapps.net/forgot-password`
- Kullanıcıya "Şifre sıfırlama işlemi için web sitesini ziyaret edin" mesajı gösterin
- Web'de şifre sıfırlama işlemi tamamlandıktan sonra mobil uygulamaya geri dönülebilir

#### **Kullanıcı Deneyimi:**
1. Kullanıcı "Şifremi Unuttum" butonuna tıklar
2. "Şifre sıfırlama için web sitesini ziyaret edin" mesajı gösterilir
3. Web link'i açılır veya kopyalanır
4. Kullanıcı web'de şifre sıfırlama işlemini tamamlar
5. Yeni şifre ile mobil uygulamaya giriş yapar

---

## 🤖 Android (Kotlin) Entegrasyonu

### 1. Dependencies (build.gradle)

```kotlin
// app/build.gradle.kts
dependencies {
    implementation("com.squareup.retrofit2:retrofit:2.9.0")
    implementation("com.squareup.retrofit2:converter-gson:2.9.0")
    implementation("com.squareup.okhttp3:logging-interceptor:4.11.0")
    implementation("androidx.security:security-crypto:1.1.0-alpha06")
    implementation("androidx.datastore:datastore-preferences:1.0.0")
}
```

### 2. Data Models

```kotlin
// models/AuthModels.kt
data class LoginRequest(
    val email: String,
    val password: String
)

data class RegisterRequest(
    val userName: String,
    val email: String,
    val password: String
)

data class LoginResponse(
    val userId: String,
    val email: String,
    val userName: String?,
    val accessToken: String,
    val expiresAt: String,
    val refreshToken: String,
    val refreshExpires: String
)

data class ForgotPasswordRequest(
    val email: String
)

data class ResetPasswordRequest(
    val email: String,
    val token: String,
    val newPassword: String
)

data class ChangePasswordRequest(
    val currentPassword: String,
    val newPassword: String
)

data class ApiResponse<T>(
    val success: Boolean,
    val message: String,
    val data: T? = null
)
```

### 3. API Service Interface

```kotlin
// network/AuthApiService.kt
interface AuthApiService {
    @POST("$AUTH_ENDPOINT/register")
    suspend fun register(@Body request: RegisterRequest): Response<LoginResponse>
    
    @POST("$AUTH_ENDPOINT/login")
    suspend fun login(@Body request: LoginRequest): Response<LoginResponse>
    
    @POST("$AUTH_ENDPOINT/refreshToken")
    suspend fun refreshToken(@Body request: RefreshTokenRequest): Response<RefreshTokenResponse>
    
    @POST("$AUTH_ENDPOINT/revokeRefreshToken")
    suspend fun revokeRefreshToken(
        @Header("Authorization") token: String,
        @Body request: RevokeRefreshTokenRequest
    ): Response<Unit>
    
    @POST("$AUTH_ENDPOINT/forgotPassword")
    suspend fun forgotPassword(@Body request: ForgotPasswordRequest): Response<ApiResponse<Unit>>
    
    @POST("$AUTH_ENDPOINT/resetPassword")
    suspend fun resetPassword(@Body request: ResetPasswordRequest): Response<ApiResponse<Unit>>
    
    @POST("$AUTH_ENDPOINT/changePassword")
    suspend fun changePassword(
        @Header("Authorization") token: String,
        @Body request: ChangePasswordRequest
    ): Response<ApiResponse<Unit>>
}
```

### 4. Token Manager

```kotlin
// security/TokenManager.kt
class TokenManager(private val context: Context) {
    private val dataStore = context.dataStore
    private val accessTokenKey = stringPreferencesKey("access_token")
    private val refreshTokenKey = stringPreferencesKey("refresh_token")
    
    suspend fun saveTokens(accessToken: String, refreshToken: String) {
        dataStore.edit { preferences ->
            preferences[accessTokenKey] = accessToken
            preferences[refreshTokenKey] = refreshToken
        }
    }
    
    suspend fun getAccessToken(): String? {
        return dataStore.data.first()[accessTokenKey]
    }
    
    suspend fun getRefreshToken(): String? {
        return dataStore.data.first()[refreshTokenKey]
    }
    
    suspend fun clearTokens() {
        dataStore.edit { preferences ->
            preferences.remove(accessTokenKey)
            preferences.remove(refreshTokenKey)
        }
    }
    
    suspend fun isLoggedIn(): Boolean {
        val accessToken = getAccessToken()
        return !accessToken.isNullOrEmpty()
    }
}
```

### 5. Auth Repository

```kotlin
// repository/AuthRepository.kt
class AuthRepository(
    private val authApiService: AuthApiService,
    private val tokenManager: TokenManager
) {
    
    suspend fun login(email: String, password: String): Result<LoginResponse> {
        return try {
            val request = LoginRequest(email, password)
            val response = authApiService.login(request)
            
            if (response.isSuccessful) {
                val loginResponse = response.body()!!
                tokenManager.saveTokens(loginResponse.accessToken, loginResponse.refreshToken)
                Result.success(loginResponse)
            } else {
                Result.failure(Exception("Login failed: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun register(userName: String, email: String, password: String): Result<LoginResponse> {
        return try {
            val request = RegisterRequest(userName, email, password)
            val response = authApiService.register(request)
            
            if (response.isSuccessful) {
                val registerResponse = response.body()!!
                tokenManager.saveTokens(registerResponse.accessToken, registerResponse.refreshToken)
                Result.success(registerResponse)
            } else {
                Result.failure(Exception("Registration failed: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun forgotPassword(email: String): Result<Unit> {
        return try {
            val request = ForgotPasswordRequest(email)
            val response = authApiService.forgotPassword(request)
            
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception("Forgot password failed: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun resetPassword(email: String, token: String, newPassword: String): Result<Unit> {
        return try {
            val request = ResetPasswordRequest(email, token, newPassword)
            val response = authApiService.resetPassword(request)
            
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception("Password reset failed: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun changePassword(currentPassword: String, newPassword: String): Result<Unit> {
        return try {
            val accessToken = tokenManager.getAccessToken()
            if (accessToken == null) {
                return Result.failure(Exception("Not authenticated"))
            }
            
            val request = ChangePasswordRequest(currentPassword, newPassword)
            val response = authApiService.changePassword("Bearer $accessToken", request)
            
            if (response.isSuccessful) {
                Result.success(Unit)
            } else {
                Result.failure(Exception("Password change failed: ${response.code()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    suspend fun logout(): Result<Unit> {
        return try {
            val refreshToken = tokenManager.getRefreshToken()
            val accessToken = tokenManager.getAccessToken()
            
            if (refreshToken != null && accessToken != null) {
                val request = RevokeRefreshTokenRequest(refreshToken)
                authApiService.revokeRefreshToken("Bearer $accessToken", request)
            }
            
            tokenManager.clearTokens()
            Result.success(Unit)
        } catch (e: Exception) {
            // Logout her durumda token'ları temizle
            tokenManager.clearTokens()
            Result.success(Unit)
        }
    }
}
```

### 6. ViewModel

```kotlin
// viewmodel/AuthViewModel.kt
class AuthViewModel(
    private val authRepository: AuthRepository
) : ViewModel() {
    
    private val _loginState = MutableStateFlow<AuthState>(AuthState.Idle)
    val loginState: StateFlow<AuthState> = _loginState.asStateFlow()
    
    private val _registerState = MutableStateFlow<AuthState>(AuthState.Idle)
    val registerState: StateFlow<AuthState> = _registerState.asStateFlow()
    
    fun login(email: String, password: String) {
        viewModelScope.launch {
            _loginState.value = AuthState.Loading
            
            authRepository.login(email, password)
                .onSuccess { loginResponse ->
                    _loginState.value = AuthState.Success(loginResponse)
                }
                .onFailure { error ->
                    _loginState.value = AuthState.Error(error.message ?: "Login failed")
                }
        }
    }
    
    fun register(userName: String, email: String, password: String) {
        viewModelScope.launch {
            _registerState.value = AuthState.Loading
            
            authRepository.register(userName, email, password)
                .onSuccess { registerResponse ->
                    _registerState.value = AuthState.Success(registerResponse)
                }
                .onFailure { error ->
                    _registerState.value = AuthState.Error(error.message ?: "Registration failed")
                }
        }
    }
    
    fun forgotPassword(email: String) {
        viewModelScope.launch {
            authRepository.forgotPassword(email)
                .onSuccess {
                    // Başarılı - kullanıcıya bilgi ver
                }
                .onFailure { error ->
                    // Hata - kullanıcıya bilgi ver
                }
        }
    }
    
    fun resetPassword(email: String, token: String, newPassword: String) {
        viewModelScope.launch {
            authRepository.resetPassword(email, token, newPassword)
                .onSuccess {
                    // Başarılı - login sayfasına yönlendir
                }
                .onFailure { error ->
                    // Hata - kullanıcıya bilgi ver
                }
        }
    }
    
    fun changePassword(currentPassword: String, newPassword: String) {
        viewModelScope.launch {
            authRepository.changePassword(currentPassword, newPassword)
                .onSuccess {
                    // Başarılı - kullanıcıya bilgi ver
                }
                .onFailure { error ->
                    // Hata - kullanıcıya bilgi ver
                }
        }
    }
}

sealed class AuthState {
    object Idle : AuthState()
    object Loading : AuthState()
    data class Success(val data: LoginResponse) : AuthState()
    data class Error(val message: String) : AuthState()
}
```

### 7. Login Activity

```kotlin
// ui/LoginActivity.kt
class LoginActivity : AppCompatActivity() {
    private lateinit var binding: ActivityLoginBinding
    private lateinit var authViewModel: AuthViewModel
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)
        
        setupViewModel()
        setupObservers()
        setupClickListeners()
    }
    
    private fun setupViewModel() {
        val authRepository = AuthRepository(
            RetrofitClient.authApiService,
            TokenManager(this)
        )
        authViewModel = AuthViewModel(authRepository)
    }
    
    private fun setupObservers() {
        authViewModel.loginState.collect { state ->
            when (state) {
                is AuthState.Loading -> {
                    binding.loginButton.isEnabled = false
                    binding.progressBar.visibility = View.VISIBLE
                }
                is AuthState.Success -> {
                    binding.progressBar.visibility = View.GONE
                    binding.loginButton.isEnabled = true
                    
                    // Dashboard'a yönlendir
                    startActivity(Intent(this, DashboardActivity::class.java))
                    finish()
                }
                is AuthState.Error -> {
                    binding.progressBar.visibility = View.GONE
                    binding.loginButton.isEnabled = true
                    Toast.makeText(this, state.message, Toast.LENGTH_LONG).show()
                }
                else -> {
                    binding.progressBar.visibility = View.GONE
                    binding.loginButton.isEnabled = true
                }
            }
        }
    }
    
    private fun setupClickListeners() {
        binding.loginButton.setOnClickListener {
            val email = binding.emailEditText.text.toString().trim()
            val password = binding.passwordEditText.text.toString().trim()
            
            if (email.isEmpty() || password.isEmpty()) {
                Toast.makeText(this, "Email ve şifre gerekli", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            
            authViewModel.login(email, password)
        }
        
        binding.forgotPasswordButton.setOnClickListener {
            startActivity(Intent(this, ForgotPasswordActivity::class.java))
        }
        
        binding.registerButton.setOnClickListener {
            startActivity(Intent(this, RegisterActivity::class.java))
        }
    }
}
```

---

## 🔐 Şifre Değiştirme (Sadece Mobil)

### **Android (Kotlin) - Change Password Activity**

#### **Ne Yapmalısınız:**
- `ActivityChangePasswordBinding` ile view binding kullanın
- `AuthViewModel` ile `changePassword` fonksiyonunu çağırın
- `StateFlow` ile loading state'ini observe edin
- Şifre validasyonu için `TextWatcher` kullanın
- Şifreler eşleşmiyorsa kırmızı `TextView` gösterin
- Başarılı olursa `Toast` gösterin ve profil ekranına dönün

#### **Örnek Implementasyon:**

```kotlin
// ui/ChangePasswordActivity.kt
class ChangePasswordActivity : AppCompatActivity() {
    private lateinit var binding: ActivityChangePasswordBinding
    private lateinit var authViewModel: AuthViewModel
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityChangePasswordBinding.inflate(layoutInflater)
        setContentView(binding.root)
        
        setupViewModel()
        setupObservers()
        setupClickListeners()
        setupValidation()
    }
    
    private fun setupViewModel() {
        val authRepository = AuthRepository(
            RetrofitClient.authApiService,
            TokenManager(this)
        )
        authViewModel = AuthViewModel(authRepository)
    }
    
    private fun setupObservers() {
        authViewModel.changePasswordState.collect { state ->
            when (state) {
                is AuthState.Loading -> {
                    binding.changePasswordButton.isEnabled = false
                    binding.progressBar.visibility = View.VISIBLE
                }
                is AuthState.Success -> {
                    binding.progressBar.visibility = View.GONE
                    binding.changePasswordButton.isEnabled = true
                    Toast.makeText(this, "Şifreniz başarıyla güncellendi", Toast.LENGTH_LONG).show()
                    finish()
                }
                is AuthState.Error -> {
                    binding.progressBar.visibility = View.GONE
                    binding.changePasswordButton.isEnabled = true
                    Toast.makeText(this, state.message, Toast.LENGTH_LONG).show()
                }
                else -> {
                    binding.progressBar.visibility = View.GONE
                    binding.changePasswordButton.isEnabled = true
                }
            }
        }
    }
    
    private fun setupClickListeners() {
        binding.changePasswordButton.setOnClickListener {
            val currentPassword = binding.currentPasswordEditText.text.toString().trim()
            val newPassword = binding.newPasswordEditText.text.toString().trim()
            val confirmPassword = binding.confirmPasswordEditText.text.toString().trim()
            
            if (currentPassword.isEmpty() || newPassword.isEmpty() || confirmPassword.isEmpty()) {
                Toast.makeText(this, "Tüm alanlar gerekli", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            
            if (newPassword != confirmPassword) {
                Toast.makeText(this, "Yeni şifreler eşleşmiyor", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            
            if (newPassword.length < 8) {
                Toast.makeText(this, "Yeni şifre en az 8 karakter olmalı", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            
            authViewModel.changePassword(currentPassword, newPassword)
        }
    }
    
    private fun setupValidation() {
        // Real-time şifre validasyonu
        binding.newPasswordEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                val password = s.toString()
                if (password.length >= 8) {
                    binding.passwordStrengthText.text = "✅ Güçlü şifre"
                    binding.passwordStrengthText.setTextColor(ContextCompat.getColor(this@ChangePasswordActivity, R.color.green))
                } else {
                    binding.passwordStrengthText.text = "❌ En az 8 karakter gerekli"
                    binding.passwordStrengthText.setTextColor(ContextCompat.getColor(this@ChangePasswordActivity, R.color.red))
                }
            }
            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}
            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {}
        })
    }
}
```

### **iOS (Swift) - Change Password View**

#### **Ne Yapmalısınız:**
- `UITextField` ile şifre alanları oluşturun
- `UIButton` ile "Şifreyi Güncelle" butonu ekleyin
- `UIActivityIndicatorView` ile loading göstergesi ekleyin
- `AuthViewModel` ile `changePassword` fonksiyonunu çağırın
- Başarılı olursa `UIAlertController` ile mesaj gösterin

#### **Örnek Implementasyon:**

```swift
// ChangePasswordViewController.swift
class ChangePasswordViewController: UIViewController {
    @IBOutlet weak var currentPasswordTextField: UITextField!
    @IBOutlet weak var newPasswordTextField: UITextField!
    @IBOutlet weak var confirmPasswordTextField: UITextField!
    @IBOutlet weak var changePasswordButton: UIButton!
    @IBOutlet weak var activityIndicator: UIActivityIndicatorView!
    
    private let authViewModel = AuthViewModel()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        setupUI()
        setupObservers()
    }
    
    private func setupUI() {
        currentPasswordTextField.isSecureTextEntry = true
        newPasswordTextField.isSecureTextEntry = true
        confirmPasswordTextField.isSecureTextEntry = true
        
        // Real-time şifre validasyonu
        newPasswordTextField.addTarget(self, action: #selector(passwordChanged), for: .editingChanged)
    }
    
    @objc private func passwordChanged() {
        let password = newPasswordTextField.text ?? ""
        if password.count >= 8 {
            // Güçlü şifre mesajı
        } else {
            // Zayıf şifre mesajı
        }
    }
    
    @IBAction func changePasswordButtonTapped(_ sender: UIButton) {
        guard let currentPassword = currentPasswordTextField.text,
              let newPassword = newPasswordTextField.text,
              let confirmPassword = confirmPasswordTextField.text else {
            showAlert(title: "Hata", message: "Tüm alanlar gerekli")
            return
        }
        
        if newPassword != confirmPassword {
            showAlert(title: "Hata", message: "Yeni şifreler eşleşmiyor")
            return
        }
        
        if newPassword.count < 8 {
            showAlert(title: "Hata", message: "Yeni şifre en az 8 karakter olmalı")
            return
        }
        
        changePasswordButton.isEnabled = false
        activityIndicator.startAnimating()
        
        authViewModel.changePassword(currentPassword: currentPassword, newPassword: newPassword)
    }
    
    private func setupObservers() {
        authViewModel.changePasswordState = { [weak self] state in
            DispatchQueue.main.async {
                self?.activityIndicator.stopAnimating()
                self?.changePasswordButton.isEnabled = true
                
                switch state {
                case .success:
                    self?.showAlert(title: "Başarılı", message: "Şifreniz başarıyla güncellendi") {
                        self?.navigationController?.popViewController(animated: true)
                    }
                case .error(let message):
                    self?.showAlert(title: "Hata", message: message)
                case .loading:
                    break
                }
            }
        }
    }
    
    private func showAlert(title: String, message: String, completion: (() -> Void)? = nil) {
        let alert = UIAlertController(title: title, message: message, preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: "Tamam", style: .default) { _ in
            completion?()
        })
        present(alert, animated: true)
    }
}
```

### **React Native - Change Password Screen**

#### **Ne Yapmalısınız:**
- `TextInput` ile şifre alanları oluşturun
- `TouchableOpacity` ile "Şifreyi Güncelle" butonu ekleyin
- `ActivityIndicator` ile loading göstergesi ekleyin
- `AuthService` ile `changePassword` fonksiyonunu çağırın
- Başarılı olursa `Alert` ile mesaj gösterin

#### **Örnek Implementasyon:**

```javascript
// screens/ChangePasswordScreen.js
import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  ActivityIndicator,
  Alert,
  StyleSheet
} from 'react-native';
import AuthService from '../services/AuthService';

const ChangePasswordScreen = ({ navigation }) => {
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChangePassword = async () => {
    if (!currentPassword || !newPassword || !confirmPassword) {
      Alert.alert('Hata', 'Tüm alanlar gerekli');
      return;
    }

    if (newPassword !== confirmPassword) {
      Alert.alert('Hata', 'Yeni şifreler eşleşmiyor');
      return;
    }

    if (newPassword.length < 8) {
      Alert.alert('Hata', 'Yeni şifre en az 8 karakter olmalı');
      return;
    }

    setLoading(true);

    try {
      const result = await AuthService.changePassword(currentPassword, newPassword);
      
      if (result.success) {
        Alert.alert('Başarılı', 'Şifreniz başarıyla güncellendi', [
          { text: 'Tamam', onPress: () => navigation.goBack() }
        ]);
      } else {
        Alert.alert('Hata', result.error || 'Şifre değiştirme başarısız');
      }
    } catch (error) {
      Alert.alert('Hata', 'Bir hata oluştu, lütfen tekrar deneyin');
    }

    setLoading(false);
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Şifre Değiştir</Text>
      
      <TextInput
        style={styles.input}
        placeholder="Mevcut Şifre"
        secureTextEntry
        value={currentPassword}
        onChangeText={setCurrentPassword}
      />
      
      <TextInput
        style={styles.input}
        placeholder="Yeni Şifre"
        secureTextEntry
        value={newPassword}
        onChangeText={setNewPassword}
      />
      
      <TextInput
        style={styles.input}
        placeholder="Yeni Şifre Tekrar"
        secureTextEntry
        value={confirmPassword}
        onChangeText={setConfirmPassword}
      />
      
      <TouchableOpacity
        style={[styles.button, loading && styles.buttonDisabled]}
        onPress={handleChangePassword}
        disabled={loading}
      >
        {loading ? (
          <ActivityIndicator color="white" />
        ) : (
          <Text style={styles.buttonText}>Şifreyi Güncelle</Text>
        )}
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 20,
    backgroundColor: '#f5f5f5'
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 30,
    textAlign: 'center'
  },
  input: {
    backgroundColor: 'white',
    padding: 15,
    borderRadius: 8,
    marginBottom: 15,
    borderWidth: 1,
    borderColor: '#ddd'
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 20
  },
  buttonDisabled: {
    backgroundColor: '#ccc'
  },
  buttonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold'
  }
});

export default ChangePasswordScreen;
```

---

## 📱 Deep Link Handling

### 1. Android Manifest

```xml
<!-- AndroidManifest.xml -->
<activity
    android:name=".ui.ResetPasswordActivity"
    android:exported="true"
    android:launchMode="singleTop">
    
    <intent-filter android:autoVerify="true">
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="te4it"
              android:host="reset-password" />
    </intent-filter>
    
    <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="https"
              android:host="te4it-frontend.azurestaticapps.net"
              android:pathPrefix="/reset-password" />
    </intent-filter>
</activity>
```

### 2. Deep Link Handler

```kotlin
// utils/DeepLinkHandler.kt
class DeepLinkHandler {
    companion object {
        fun handleResetPasswordLink(uri: Uri): ResetPasswordData? {
            return when {
                uri.scheme == "te4it" && uri.host == "reset-password" -> {
                    val email = uri.getQueryParameter("email")
                    val token = uri.getQueryParameter("token")
                    
                    if (email != null && token != null) {
                        ResetPasswordData(email, token)
                    } else null
                }
                
                uri.scheme == "https" && 
                uri.host == "te4it-frontend.azurestaticapps.net" &&
                uri.path?.startsWith("/reset-password") == true -> {
                    val email = uri.getQueryParameter("email")
                    val token = uri.getQueryParameter("token")
                    
                    if (email != null && token != null) {
                        ResetPasswordData(email, token)
                    } else null
                }
                
                else -> null
            }
        }
    }
}

data class ResetPasswordData(
    val email: String,
    val token: String
)
```

### 3. Reset Password Activity

```kotlin
// ui/ResetPasswordActivity.kt
class ResetPasswordActivity : AppCompatActivity() {
    private lateinit var binding: ActivityResetPasswordBinding
    private lateinit var authViewModel: AuthViewModel
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityResetPasswordBinding.inflate(layoutInflater)
        setContentView(binding.root)
        
        setupViewModel()
        handleIntent()
        setupClickListeners()
    }
    
    private fun handleIntent() {
        val resetData = DeepLinkHandler.handleResetPasswordLink(intent.data ?: Uri.EMPTY)
        
        if (resetData != null) {
            binding.emailEditText.setText(resetData.email)
            binding.emailEditText.isEnabled = false
            binding.tokenEditText.setText(resetData.token)
            binding.tokenEditText.isEnabled = false
        } else {
            // Geçersiz link
            Toast.makeText(this, "Geçersiz şifre sıfırlama linki", Toast.LENGTH_LONG).show()
            finish()
        }
    }
    
    private fun setupClickListeners() {
        binding.resetPasswordButton.setOnClickListener {
            val email = binding.emailEditText.text.toString().trim()
            val token = binding.tokenEditText.text.toString().trim()
            val newPassword = binding.newPasswordEditText.text.toString().trim()
            val confirmPassword = binding.confirmPasswordEditText.text.toString().trim()
            
            if (email.isEmpty() || token.isEmpty() || newPassword.isEmpty()) {
                Toast.makeText(this, "Tüm alanlar gerekli", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            
            if (newPassword != confirmPassword) {
                Toast.makeText(this, "Şifreler eşleşmiyor", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            
            if (newPassword.length < 8) {
                Toast.makeText(this, "Şifre en az 8 karakter olmalı", Toast.LENGTH_SHORT).show()
                return@setOnClickListener
            }
            
            authViewModel.resetPassword(email, token, newPassword)
        }
    }
}
```

---

## 🍎 iOS (Swift) Entegrasyonu

### 1. Data Models

```swift
// Models/AuthModels.swift
struct LoginRequest: Codable {
    let email: String
    let password: String
}

struct RegisterRequest: Codable {
    let userName: String
    let email: String
    let password: String
}

struct LoginResponse: Codable {
    let userId: String
    let email: String
    let userName: String?
    let accessToken: String
    let expiresAt: String
    let refreshToken: String
    let refreshExpires: String
}

struct ApiResponse<T: Codable>: Codable {
    let success: Bool
    let message: String
    let data: T?
}
```

### 2. API Service

```swift
// Services/AuthApiService.swift
class AuthApiService {
    private let baseURL = ApiConstants.currentBaseURL
    private let session = URLSession.shared
    
    func login(email: String, password: String) async throws -> LoginResponse {
        let request = LoginRequest(email: email, password: password)
        return try await performRequest(
            endpoint: "/api/v1/auth/login",
            method: "POST",
            body: request,
            responseType: LoginResponse.self
        )
    }
    
    func register(userName: String, email: String, password: String) async throws -> LoginResponse {
        let request = RegisterRequest(userName: userName, email: email, password: password)
        return try await performRequest(
            endpoint: "/api/v1/auth/register",
            method: "POST",
            body: request,
            responseType: LoginResponse.self
        )
    }
    
    func forgotPassword(email: String) async throws -> ApiResponse<Empty> {
        let request = ForgotPasswordRequest(email: email)
        return try await performRequest(
            endpoint: "/api/v1/auth/forgotPassword",
            method: "POST",
            body: request,
            responseType: ApiResponse<Empty>.self
        )
    }
    
    func resetPassword(email: String, token: String, newPassword: String) async throws -> ApiResponse<Empty> {
        let request = ResetPasswordRequest(email: email, token: token, newPassword: newPassword)
        return try await performRequest(
            endpoint: "/api/v1/auth/resetPassword",
            method: "POST",
            body: request,
            responseType: ApiResponse<Empty>.self
        )
    }
    
    func changePassword(currentPassword: String, newPassword: String) async throws -> ApiResponse<Empty> {
        let request = ChangePasswordRequest(currentPassword: currentPassword, newPassword: newPassword)
        return try await performRequest(
            endpoint: "/api/v1/auth/changePassword",
            method: "POST",
            body: request,
            responseType: ApiResponse<Empty>.self,
            requiresAuth: true
        )
    }
    
    private func performRequest<T: Codable, R: Codable>(
        endpoint: String,
        method: String,
        body: T,
        responseType: R.Type,
        requiresAuth: Bool = false
    ) async throws -> R {
        guard let url = URL(string: baseURL + endpoint) else {
            throw ApiError.invalidURL
        }
        
        var request = URLRequest(url: url)
        request.httpMethod = method
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        
        if requiresAuth {
            if let token = TokenManager.shared.getAccessToken() {
                request.setValue("Bearer \(token)", forHTTPHeaderField: "Authorization")
            } else {
                throw ApiError.notAuthenticated
            }
        }
        
        request.httpBody = try JSONEncoder().encode(body)
        
        let (data, response) = try await session.data(for: request)
        
        guard let httpResponse = response as? HTTPURLResponse else {
            throw ApiError.invalidResponse
        }
        
        guard httpResponse.statusCode == 200 || httpResponse.statusCode == 201 else {
            throw ApiError.httpError(httpResponse.statusCode)
        }
        
        return try JSONDecoder().decode(responseType, from: data)
    }
}

enum ApiError: Error {
    case invalidURL
    case invalidResponse
    case notAuthenticated
    case httpError(Int)
}
```

### 3. Token Manager

```swift
// Services/TokenManager.swift
class TokenManager {
    static let shared = TokenManager()
    private let keychain = Keychain(service: "com.te4it.app")
    
    private init() {}
    
    func saveTokens(accessToken: String, refreshToken: String) {
        keychain["accessToken"] = accessToken
        keychain["refreshToken"] = refreshToken
    }
    
    func getAccessToken() -> String? {
        return keychain["accessToken"]
    }
    
    func getRefreshToken() -> String? {
        return keychain["refreshToken"]
    }
    
    func clearTokens() {
        keychain["accessToken"] = nil
        keychain["refreshToken"] = nil
    }
    
    func isLoggedIn() -> Bool {
        return getAccessToken() != nil
    }
}
```

---

## ⚛️ React Native Entegrasyonu

### 1. Auth Service

```javascript
// services/AuthService.js
import AsyncStorage from '@react-native-async-storage/async-storage';
import { Linking } from 'react-native';

const API_BASE_URL = __DEV__ 
  ? 'https://localhost:5001' 
  : 'https://te4it-api.azurewebsites.net';

class AuthService {
  constructor() {
    this.baseURL = API_BASE_URL;
    this.accessToken = null;
    this.refreshToken = null;
  }

  async saveTokens(accessToken, refreshToken) {
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    await AsyncStorage.setItem('accessToken', accessToken);
    await AsyncStorage.setItem('refreshToken', refreshToken);
  }

  async loadTokens() {
    this.accessToken = await AsyncStorage.getItem('accessToken');
    this.refreshToken = await AsyncStorage.getItem('refreshToken');
  }

  async clearTokens() {
    this.accessToken = null;
    this.refreshToken = null;
    await AsyncStorage.removeItem('accessToken');
    await AsyncStorage.removeItem('refreshToken');
  }

  async apiCall(endpoint, options = {}) {
    const url = `${this.baseURL}${endpoint}`;
    const config = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      }
    };

    if (this.accessToken) {
      config.headers.Authorization = `Bearer ${this.accessToken}`;
    }

    const response = await fetch(url, config);
    
    if (response.status === 401 && this.refreshToken) {
      await this.refreshAccessToken();
      config.headers.Authorization = `Bearer ${this.accessToken}`;
      return fetch(url, config);
    }

    return response;
  }

  async login(email, password) {
    const response = await this.apiCall('/api/v1/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password })
    });

    if (!response.ok) {
      throw new Error('Login failed');
    }

    const data = await response.json();
    await this.saveTokens(data.accessToken, data.refreshToken);
    return data;
  }

  async register(userName, email, password) {
    const response = await this.apiCall('/api/v1/auth/register', {
      method: 'POST',
      body: JSON.stringify({ userName, email, password })
    });

    if (!response.ok) {
      throw new Error('Registration failed');
    }

    const data = await response.json();
    await this.saveTokens(data.accessToken, data.refreshToken);
    return data;
  }

  async forgotPassword(email) {
    const response = await this.apiCall('/api/v1/auth/forgotPassword', {
      method: 'POST',
      body: JSON.stringify({ email })
    });

    return response.ok;
  }

  async resetPassword(email, token, newPassword) {
    const response = await this.apiCall('/api/v1/auth/resetPassword', {
      method: 'POST',
      body: JSON.stringify({ email, token, newPassword })
    });

    if (!response.ok) {
      throw new Error('Password reset failed');
    }

    return await response.json();
  }

  async changePassword(currentPassword, newPassword) {
    const response = await this.apiCall('/api/v1/auth/changePassword', {
      method: 'POST',
      body: JSON.stringify({ currentPassword, newPassword })
    });

    if (!response.ok) {
      throw new Error('Password change failed');
    }

    return await response.json();
  }

  async logout() {
    if (this.refreshToken) {
      try {
        await this.apiCall('/api/v1/auth/revokeRefreshToken', {
          method: 'POST',
          body: JSON.stringify({ refreshToken: this.refreshToken })
        });
      } catch (error) {
        console.error('Logout error:', error);
      }
    }
    
    await this.clearTokens();
  }
}

export default new AuthService();
```

### 2. Deep Link Handler

```javascript
// utils/DeepLinkHandler.js
import { Linking } from 'react-native';

export const setupDeepLinkHandler = (navigation) => {
  const handleDeepLink = (url) => {
    if (url.includes('reset-password')) {
      const urlParams = new URLSearchParams(url.split('?')[1]);
      const email = urlParams.get('email');
      const token = urlParams.get('token');
      
      if (email && token) {
        navigation.navigate('ResetPassword', {
          email: decodeURIComponent(email),
          token: decodeURIComponent(token)
        });
      }
    }
  };

  // App açıkken gelen link'leri yakala
  Linking.addEventListener('url', ({ url }) => {
    handleDeepLink(url);
  });

  // App kapalıyken açılan link'i yakala
  Linking.getInitialURL().then((url) => {
    if (url) {
      handleDeepLink(url);
    }
  });
};
```

---

## 🧪 Test Senaryoları

### 1. Android Unit Test

```kotlin
// test/AuthRepositoryTest.kt
@RunWith(MockitoJUnitRunner::class)
class AuthRepositoryTest {
    
    @Mock
    private lateinit var authApiService: AuthApiService
    
    @Mock
    private lateinit var tokenManager: TokenManager
    
    private lateinit var authRepository: AuthRepository
    
    @Before
    fun setup() {
        authRepository = AuthRepository(authApiService, tokenManager)
    }
    
    @Test
    fun `login should save tokens on success`() = runTest {
        // Given
        val email = "test@example.com"
        val password = "password123"
        val loginResponse = LoginResponse(
            userId = "123",
            email = email,
            userName = "testuser",
            accessToken = "access_token",
            expiresAt = "2025-01-20T10:00:00Z",
            refreshToken = "refresh_token",
            refreshExpires = "2025-01-27T10:00:00Z"
        )
        
        whenever(authApiService.login(any())).thenReturn(
            Response.success(loginResponse)
        )
        
        // When
        val result = authRepository.login(email, password)
        
        // Then
        assertTrue(result.isSuccess)
        verify(tokenManager).saveTokens("access_token", "refresh_token")
    }
    
    @Test
    fun `login should return failure on API error`() = runTest {
        // Given
        val email = "test@example.com"
        val password = "wrongpassword"
        
        whenever(authApiService.login(any())).thenReturn(
            Response.error(401, "Unauthorized".toResponseBody())
        )
        
        // When
        val result = authRepository.login(email, password)
        
        // Then
        assertTrue(result.isFailure)
        verify(tokenManager, never()).saveTokens(any(), any())
    }
}
```

### 2. React Native Test

```javascript
// __tests__/AuthService.test.js
import AuthService from '../services/AuthService';
import AsyncStorage from '@react-native-async-storage/async-storage';

// Mock AsyncStorage
jest.mock('@react-native-async-storage/async-storage', () => ({
  setItem: jest.fn(),
  getItem: jest.fn(),
  removeItem: jest.fn(),
}));

// Mock fetch
global.fetch = jest.fn();

describe('AuthService', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test('should login successfully', async () => {
    const mockResponse = {
      ok: true,
      json: () => Promise.resolve({
        userId: '123',
        email: 'test@example.com',
        accessToken: 'access_token',
        refreshToken: 'refresh_token'
      })
    };

    fetch.mockResolvedValueOnce(mockResponse);

    const result = await AuthService.login('test@example.com', 'password123');

    expect(result.email).toBe('test@example.com');
    expect(AsyncStorage.setItem).toHaveBeenCalledWith('accessToken', 'access_token');
    expect(AsyncStorage.setItem).toHaveBeenCalledWith('refreshToken', 'refresh_token');
  });

  test('should handle login failure', async () => {
    const mockResponse = {
      ok: false,
      status: 401
    };

    fetch.mockResolvedValueOnce(mockResponse);

    await expect(AuthService.login('test@example.com', 'wrongpassword'))
      .rejects.toThrow('Login failed');
  });
});
```

---

## 📞 Destek ve Kaynaklar

### Hızlı Başvuru

- **Production API**: `https://te4it-api.azurewebsites.net`
- **Development API**: `https://localhost:5001`
- **Swagger UI**: `https://te4it-api.azurewebsites.net/swagger`
- **GitHub**: https://github.com/burhanarslanbas/TE4IT

### Platform Spesifik Notlar

#### Android
- **Min SDK**: 21 (Android 5.0)
- **Target SDK**: 34 (Android 14)
- **Permissions**: INTERNET, ACCESS_NETWORK_STATE
- **Deep Links**: te4it://reset-password

#### iOS
- **Min iOS**: 13.0
- **Target iOS**: 17.0
- **URL Schemes**: te4it
- **Associated Domains**: te4it-frontend.azurestaticapps.net

#### React Native
- **Min RN**: 0.70
- **Platforms**: Android, iOS
- **Dependencies**: AsyncStorage, Linking

### Sorun Giderme

1. **Network Hatası**: API base URL'ini kontrol edin
2. **Token Hatası**: Token'ların doğru kaydedildiğini kontrol edin
3. **Deep Link Hatası**: Manifest/Info.plist ayarlarını kontrol edin
4. **CORS Hatası**: Development ortamında HTTPS kullanın

### İletişim

- **Email**: infoarslanbas@gmail.com
- **GitHub Issues**: https://github.com/burhanarslanbas/TE4IT/issues

---

*Bu dokümantasyon TE4IT Mobil Entegrasyon Rehberi v1.0 için hazırlanmıştır. Son güncelleme: Ocak 2025*
