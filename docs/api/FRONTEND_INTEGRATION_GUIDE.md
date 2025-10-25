# Frontend Geliştiriciler İçin TE4IT API Entegrasyon Rehberi

**Sürüm:** 2.0  
**Tarih:** Ocak 2025  
**Hedef:** React, Vue, Angular ve diğer frontend framework'leri  
**Not:** Bu dokümantasyon web platformu için hazırlanmıştır. Mobil uygulamalar için ayrı dokümantasyon mevcuttur.

---

## 📋 İçindekiler

1. [Hızlı Başlangıç](#hızlı-başlangıç)
2. [Auth Endpoint'leri Rehberi](#auth-endpointleri-rehberi)
3. [React Entegrasyonu](#react-entegrasyonu)
4. [Email Link Handling](#email-link-handling)
5. [Hata Yönetimi](#hata-yönetimi)
6. [Güvenlik En İyi Uygulamaları](#güvenlik-en-iyi-uygulamaları)
7. [Test Senaryoları](#test-senaryoları)

---

## 🚀 Hızlı Başlangıç

### 1. API Base URL'leri

```javascript
// Environment'a göre base URL seçimi
const API_BASE_URL = process.env.NODE_ENV === 'production' 
  ? 'https://te4it-api.azurewebsites.net' 
  : 'https://localhost:5001';

const API_ENDPOINTS = {
  auth: `${API_BASE_URL}/api/v1/auth`,
  projects: `${API_BASE_URL}/api/v1/projects`,
  tasks: `${API_BASE_URL}/api/v1/tasks`
};
```

### 2. Temel Auth Service

```javascript
class TE4ITAuthService {
  constructor(baseUrl = API_BASE_URL) {
    this.baseUrl = baseUrl;
    this.accessToken = localStorage.getItem('accessToken');
    this.refreshToken = localStorage.getItem('refreshToken');
  }

  // Token'ları localStorage'a kaydet
  saveTokens(accessToken, refreshToken) {
    this.accessToken = accessToken;
    this.refreshToken = refreshToken;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
  }

  // Token'ları temizle
  clearTokens() {
    this.accessToken = null;
    this.refreshToken = null;
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }

  // HTTP istekleri için header hazırla
  getAuthHeaders() {
    return {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.accessToken}`
    };
  }

  // API çağrısı yap
  async apiCall(endpoint, options = {}) {
    const url = `${this.baseUrl}${endpoint}`;
    const config = {
      ...options,
      headers: {
        ...this.getAuthHeaders(),
        ...options.headers
      }
    };

    const response = await fetch(url, config);
    
    // Token süresi dolmuşsa yenile
    if (response.status === 401 && this.refreshToken) {
      await this.refreshAccessToken();
      // Tekrar dene
      config.headers.Authorization = `Bearer ${this.accessToken}`;
      return fetch(url, config);
    }

    return response;
  }
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
- Token'ları localStorage'a kaydedin
- Dashboard sayfasına yönlendirin

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
- Başarılı olursa token'ları localStorage'a kaydedin
- Dashboard sayfasına yönlendirin

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
- `refreshToken`'ı localStorage'dan alın
- `POST /api/v1/auth/refreshToken` endpoint'ini çağırın
- Yeni token'ları localStorage'a kaydedin
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
- `refreshToken`'ı localStorage'dan alın
- `POST /api/v1/auth/revokeRefreshToken` endpoint'ini çağırın
- Authorization header'ında access token gönderin
- Token'ları localStorage'dan silin
- Login sayfasına yönlendirin

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

### **5. Şifre Sıfırlama İsteği (Forgot Password)**

#### **Ne Yapmalısınız:**
- Kullanıcıdan sadece `email` adresini isteyin
- Email formatı validasyonu yapın
- `POST /api/v1/auth/forgotPassword` endpoint'ini çağırın
- Her durumda "Email adresinize şifre sıfırlama linki gönderildi" mesajı gösterin
- Login sayfasına "Giriş Sayfasına Dön" linki ekleyin

#### **Request Format:**
```json
{
  "email": "john@example.com"
}
```

#### **Response Format:**
```json
{
  "success": true,
  "message": "Şifre sıfırlama linki email adresinize gönderildi"
}
```

#### **Hata Durumları:**
- **400 Bad Request**: Geçersiz email formatı
- **429 Too Many Requests**: Çok fazla istek

---

### **6. Şifre Sıfırlama (Reset Password)**

#### **Ne Yapmalısınız:**
- Email link'inden gelen `email` ve `token` parametrelerini alın
- Email alanını disabled yapın (kullanıcı değiştiremesin)
- Token'ı gizli tutun (kullanıcı görmesin)
- Kullanıcıdan `newPassword` ve `confirmPassword` isteyin
- Şifre validasyonu yapın (en az 8 karakter, büyük/küçük harf, rakam, özel karakter)
- `POST /api/v1/auth/resetPassword` endpoint'ini çağırın
- Başarılı olursa login sayfasına yönlendirin

#### **Request Format:**
```json
{
  "email": "john@example.com",
  "token": "reset_token_from_email",
  "newPassword": "NewSecurePass123!"
}
```

#### **Response Format:**
```json
{
  "success": true,
  "message": "Şifre başarıyla güncellendi"
}
```

#### **Hata Durumları:**
- **400 Bad Request**: Token süresi dolmuş, şifre çok zayıf
- **404 Not Found**: Email bulunamadı
- **422 Validation Error**: Şifre validasyon hataları

---

### **7. Uygulama İçi Şifre Değiştirme (Change Password)**

#### **Ne Yapmalısınız:**
- Kullanıcının giriş yapmış olması gerekir
- Authorization header'ında access token gönderin
- Kullanıcıdan `currentPassword` ve `newPassword` isteyin
- Şifre validasyonu yapın (en az 8 karakter, büyük/küçük harf, rakam, özel karakter)
- `POST /api/v1/auth/changePassword` endpoint'ini çağırın
- Başarılı olursa "Şifreniz başarıyla güncellendi" mesajı gösterin

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

## ⚛️ React Entegrasyonu

### 1. Auth Context ve Hook'lar

```javascript
// contexts/AuthContext.js
import React, { createContext, useContext, useReducer, useEffect } from 'react';

const AuthContext = createContext();

// Auth state reducer
const authReducer = (state, action) => {
  switch (action.type) {
    case 'LOGIN_SUCCESS':
      return {
        ...state,
        isAuthenticated: true,
        user: action.payload.user,
        accessToken: action.payload.accessToken,
        refreshToken: action.payload.refreshToken,
        loading: false
      };
    case 'LOGOUT':
      return {
        ...state,
        isAuthenticated: false,
        user: null,
        accessToken: null,
        refreshToken: null,
        loading: false
      };
    case 'SET_LOADING':
      return { ...state, loading: action.payload };
    default:
      return state;
  }
};

// Auth Provider Component
export const AuthProvider = ({ children }) => {
  const [state, dispatch] = useReducer(authReducer, {
    isAuthenticated: false,
    user: null,
    accessToken: localStorage.getItem('accessToken'),
    refreshToken: localStorage.getItem('refreshToken'),
    loading: true
  });

  const authService = new TE4ITAuthService();

  // Sayfa yüklendiğinde token kontrolü
  useEffect(() => {
    const initAuth = async () => {
      if (state.accessToken && state.refreshToken) {
        try {
          // Token geçerliliğini kontrol et
          const response = await authService.apiCall('/api/v1/auth/me');
          if (response.ok) {
            const userData = await response.json();
            dispatch({
              type: 'LOGIN_SUCCESS',
              payload: {
                user: userData,
                accessToken: state.accessToken,
                refreshToken: state.refreshToken
              }
            });
          } else {
            // Token geçersiz, temizle
            authService.clearTokens();
            dispatch({ type: 'LOGOUT' });
          }
        } catch (error) {
          console.error('Auth initialization failed:', error);
          authService.clearTokens();
          dispatch({ type: 'LOGOUT' });
        }
      } else {
        dispatch({ type: 'SET_LOADING', payload: false });
      }
    };

    initAuth();
  }, []);

  const login = async (email, password) => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });
      
      const response = await fetch(`${authService.baseUrl}/api/v1/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });

      if (!response.ok) {
        throw new Error('Login failed');
      }

      const data = await response.json();
      authService.saveTokens(data.accessToken, data.refreshToken);
      
      dispatch({
        type: 'LOGIN_SUCCESS',
        payload: {
          user: { email: data.email, userId: data.userId },
          accessToken: data.accessToken,
          refreshToken: data.refreshToken
        }
      });

      return { success: true, data };
    } catch (error) {
      dispatch({ type: 'SET_LOADING', payload: false });
      return { success: false, error: error.message };
    }
  };

  const register = async (userName, email, password) => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });
      
      const response = await fetch(`${authService.baseUrl}/api/v1/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userName, email, password })
      });

      if (!response.ok) {
        throw new Error('Registration failed');
      }

      const data = await response.json();
      authService.saveTokens(data.accessToken, data.refreshToken);
      
      dispatch({
        type: 'LOGIN_SUCCESS',
        payload: {
          user: { email: data.email, userId: data.userId, userName: data.userName },
          accessToken: data.accessToken,
          refreshToken: data.refreshToken
        }
      });

      return { success: true, data };
    } catch (error) {
      dispatch({ type: 'SET_LOADING', payload: false });
      return { success: false, error: error.message };
    }
  };

  const logout = async () => {
    try {
      if (state.refreshToken) {
        await authService.apiCall('/api/v1/auth/revokeRefreshToken', {
          method: 'POST',
          body: JSON.stringify({ refreshToken: state.refreshToken })
        });
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      authService.clearTokens();
      dispatch({ type: 'LOGOUT' });
    }
  };

  const forgotPassword = async (email) => {
    try {
      const response = await fetch(`${authService.baseUrl}/api/v1/auth/forgotPassword`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email })
      });

      return { success: response.ok };
    } catch (error) {
      return { success: false, error: error.message };
    }
  };

  const resetPassword = async (email, token, newPassword) => {
    try {
      const response = await fetch(`${authService.baseUrl}/api/v1/auth/resetPassword`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, token, newPassword })
      });

      const data = await response.json();
      return { success: response.ok, data };
    } catch (error) {
      return { success: false, error: error.message };
    }
  };

  const changePassword = async (currentPassword, newPassword) => {
    try {
      const response = await authService.apiCall('/api/v1/auth/changePassword', {
        method: 'POST',
        body: JSON.stringify({ currentPassword, newPassword })
      });

      const data = await response.json();
      return { success: response.ok, data };
    } catch (error) {
      return { success: false, error: error.message };
    }
  };

  const value = {
    ...state,
    login,
    register,
    logout,
    forgotPassword,
    resetPassword,
    changePassword,
    authService
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom Hook
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
```

### 2. Login Component

```javascript
// components/LoginForm.jsx
import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

const LoginForm = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [error, setError] = useState('');
  
  const { login, loading } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    const result = await login(formData.email, formData.password);
    
    if (result.success) {
      navigate('/dashboard');
    } else {
      setError(result.error);
    }
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  return (
    <form onSubmit={handleSubmit} className="login-form">
      <h2>Giriş Yap</h2>
      
      {error && <div className="error-message">{error}</div>}
      
      <div className="form-group">
        <label htmlFor="email">Email:</label>
        <input
          type="email"
          id="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          required
        />
      </div>
      
      <div className="form-group">
        <label htmlFor="password">Şifre:</label>
        <input
          type="password"
          id="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          required
        />
      </div>
      
      <button type="submit" disabled={loading}>
        {loading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
      </button>
      
      <div className="form-links">
        <a href="/forgot-password">Şifremi Unuttum</a>
        <a href="/register">Kayıt Ol</a>
      </div>
    </form>
  );
};

export default LoginForm;
```

### 3. Password Reset Component

```javascript
// components/PasswordResetForm.jsx
import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useSearchParams, useNavigate } from 'react-router-dom';

const PasswordResetForm = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { resetPassword } = useAuth();
  
  const [formData, setFormData] = useState({
    email: searchParams.get('email') || '',
    token: searchParams.get('token') || '',
    newPassword: '',
    confirmPassword: ''
  });
  
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  // URL'den token ve email geldi mi kontrol et
  const hasToken = searchParams.get('token') && searchParams.get('email');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (formData.newPassword !== formData.confirmPassword) {
      setError('Şifreler eşleşmiyor');
      return;
    }

    if (formData.newPassword.length < 8) {
      setError('Şifre en az 8 karakter olmalı');
      return;
    }

    setLoading(true);

    const result = await resetPassword(
      formData.email,
      formData.token,
      formData.newPassword
    );

    if (result.success) {
      setSuccess(true);
      setTimeout(() => {
        navigate('/login');
      }, 3000);
    } else {
      setError(result.error || 'Şifre sıfırlama başarısız');
    }

    setLoading(false);
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  if (success) {
    return (
      <div className="success-message">
        <h2>✅ Şifre Başarıyla Güncellendi!</h2>
        <p>Şifreniz başarıyla güncellendi. 3 saniye sonra giriş sayfasına yönlendirileceksiniz.</p>
      </div>
    );
  }

  if (!hasToken) {
    return (
      <div className="error-message">
        <h2>❌ Geçersiz Link</h2>
        <p>Bu şifre sıfırlama linki geçersiz veya süresi dolmuş.</p>
        <a href="/forgot-password">Yeni şifre sıfırlama isteği gönder</a>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="password-reset-form">
      <h2>Şifre Sıfırla</h2>
      
      {error && <div className="error-message">{error}</div>}
      
      <div className="form-group">
        <label htmlFor="email">Email:</label>
        <input
          type="email"
          id="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          disabled
        />
      </div>
      
      <div className="form-group">
        <label htmlFor="newPassword">Yeni Şifre:</label>
        <input
          type="password"
          id="newPassword"
          name="newPassword"
          value={formData.newPassword}
          onChange={handleChange}
          required
          minLength="8"
        />
      </div>
      
      <div className="form-group">
        <label htmlFor="confirmPassword">Şifre Tekrar:</label>
        <input
          type="password"
          id="confirmPassword"
          name="confirmPassword"
          value={formData.confirmPassword}
          onChange={handleChange}
          required
          minLength="8"
        />
      </div>
      
      <button type="submit" disabled={loading}>
        {loading ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
      </button>
    </form>
  );
};

export default PasswordResetForm;
```

### 4. Change Password Component

```javascript
// components/ChangePasswordForm.jsx
import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';

const ChangePasswordForm = ({ onSuccess }) => {
  const [formData, setFormData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });
  
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);
  
  const { changePassword } = useAuth();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (formData.newPassword !== formData.confirmPassword) {
      setError('Yeni şifreler eşleşmiyor');
      return;
    }

    if (formData.newPassword.length < 8) {
      setError('Yeni şifre en az 8 karakter olmalı');
      return;
    }

    if (formData.currentPassword === formData.newPassword) {
      setError('Yeni şifre mevcut şifre ile aynı olamaz');
      return;
    }

    setLoading(true);

    const result = await changePassword(
      formData.currentPassword,
      formData.newPassword
    );

    if (result.success) {
      setSuccess('Şifreniz başarıyla güncellendi');
      setFormData({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      });
      
      if (onSuccess) {
        onSuccess();
      }
    } else {
      setError(result.error || 'Şifre değiştirme başarısız');
    }

    setLoading(false);
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  return (
    <form onSubmit={handleSubmit} className="change-password-form">
      <h3>Şifre Değiştir</h3>
      
      {error && <div className="error-message">{error}</div>}
      {success && <div className="success-message">{success}</div>}
      
      <div className="form-group">
        <label htmlFor="currentPassword">Mevcut Şifre:</label>
        <input
          type="password"
          id="currentPassword"
          name="currentPassword"
          value={formData.currentPassword}
          onChange={handleChange}
          required
        />
      </div>
      
      <div className="form-group">
        <label htmlFor="newPassword">Yeni Şifre:</label>
        <input
          type="password"
          id="newPassword"
          name="newPassword"
          value={formData.newPassword}
          onChange={handleChange}
          required
          minLength="8"
        />
      </div>
      
      <div className="form-group">
        <label htmlFor="confirmPassword">Yeni Şifre Tekrar:</label>
        <input
          type="password"
          id="confirmPassword"
          name="confirmPassword"
          value={formData.confirmPassword}
          onChange={handleChange}
          required
          minLength="8"
        />
      </div>
      
      <button type="submit" disabled={loading}>
        {loading ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
      </button>
    </form>
  );
};

export default ChangePasswordForm;
```

---

## 🔗 Email Link Handling

### **Email Link Formatı**
Backend'den gelen email'deki link formatı:
```
https://te4it-frontend.azurestaticapps.net/reset-password?email=user@example.com&token=abc123xyz
```

### **Ne Yapmalısınız:**

#### **1. React Router Setup**
- `/reset-password` route'unu tanımlayın
- Bu route'a gelen kullanıcıları `PasswordResetForm` component'ine yönlendirin
- URL parametrelerini otomatik olarak yakalayın

#### **2. URL Parametrelerini Alma**
- `useSearchParams` hook'u ile URL parametrelerini alın
- `searchParams.get('email')` ile email'i alın
- `searchParams.get('token')` ile token'ı alın
- Bu parametreleri form state'ine otomatik doldurun

#### **3. Form Alanlarını Ayarlama**
- Email alanını `disabled` yapın (kullanıcı değiştiremesin)
- Token'ı gizli tutun (kullanıcı görmesin)
- Sadece yeni şifre alanlarını aktif bırakın

#### **4. Hata Durumları**
- Token yoksa "Geçersiz link" mesajı gösterin
- Email yoksa "Geçersiz link" mesajı gösterin
- Token süresi dolmuşsa "Link'in süresi dolmuş" mesajı gösterin

### **Örnek Implementasyon:**

```javascript
// components/PasswordResetForm.jsx
import React, { useState, useEffect } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const PasswordResetForm = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { resetPassword } = useAuth();
  
  const [formData, setFormData] = useState({
    email: searchParams.get('email') || '',
    token: searchParams.get('token') || '',
    newPassword: '',
    confirmPassword: ''
  });
  
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  // URL'den token ve email geldi mi kontrol et
  const hasToken = searchParams.get('token') && searchParams.get('email');

  useEffect(() => {
    if (!hasToken) {
      setError('Geçersiz şifre sıfırlama linki');
    }
  }, [hasToken]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (formData.newPassword !== formData.confirmPassword) {
      setError('Şifreler eşleşmiyor');
      return;
    }

    if (formData.newPassword.length < 8) {
      setError('Şifre en az 8 karakter olmalı');
      return;
    }

    setLoading(true);

    const result = await resetPassword(
      formData.email,
      formData.token,
      formData.newPassword
    );

    if (result.success) {
      setSuccess(true);
      setTimeout(() => {
        navigate('/login');
      }, 3000);
    } else {
      setError(result.error || 'Şifre sıfırlama başarısız');
    }

    setLoading(false);
  };

  if (success) {
    return (
      <div className="success-message">
        <h2>✅ Şifre Başarıyla Güncellendi!</h2>
        <p>Şifreniz başarıyla güncellendi. 3 saniye sonra giriş sayfasına yönlendirileceksiniz.</p>
      </div>
    );
  }

  if (!hasToken) {
    return (
      <div className="error-message">
        <h2>❌ Geçersiz Link</h2>
        <p>Bu şifre sıfırlama linki geçersiz veya süresi dolmuş.</p>
        <a href="/forgot-password">Yeni şifre sıfırlama isteği gönder</a>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="password-reset-form">
      <h2>Şifre Sıfırla</h2>
      
      {error && <div className="error-message">{error}</div>}
      
      <div className="form-group">
        <label htmlFor="email">Email:</label>
        <input
          type="email"
          id="email"
          name="email"
          value={formData.email}
          disabled
        />
      </div>
      
      <div className="form-group">
        <label htmlFor="newPassword">Yeni Şifre:</label>
        <input
          type="password"
          id="newPassword"
          name="newPassword"
          value={formData.newPassword}
          onChange={(e) => setFormData({...formData, newPassword: e.target.value})}
          required
          minLength="8"
        />
      </div>
      
      <div className="form-group">
        <label htmlFor="confirmPassword">Şifre Tekrar:</label>
        <input
          type="password"
          id="confirmPassword"
          name="confirmPassword"
          value={formData.confirmPassword}
          onChange={(e) => setFormData({...formData, confirmPassword: e.target.value})}
          required
          minLength="8"
        />
      </div>
      
      <button type="submit" disabled={loading}>
        {loading ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
      </button>
    </form>
  );
};

export default PasswordResetForm;
```

---

## 🛡️ Güvenlik En İyi Uygulamaları

### 1. Token Güvenliği

```javascript
// utils/tokenSecurity.js
export const secureTokenStorage = {
  // Token'ları şifreleyerek sakla
  saveTokens: (accessToken, refreshToken) => {
    try {
      // Basit encoding (production'da daha güçlü şifreleme kullan)
      const encodedAccess = btoa(accessToken);
      const encodedRefresh = btoa(refreshToken);
      
      localStorage.setItem('accessToken', encodedAccess);
      localStorage.setItem('refreshToken', encodedRefresh);
    } catch (error) {
      console.error('Token kaydetme hatası:', error);
    }
  },

  // Token'ları çöz
  getTokens: () => {
    try {
      const encodedAccess = localStorage.getItem('accessToken');
      const encodedRefresh = localStorage.getItem('refreshToken');
      
      if (encodedAccess && encodedRefresh) {
        return {
          accessToken: atob(encodedAccess),
          refreshToken: atob(encodedRefresh)
        };
      }
    } catch (error) {
      console.error('Token okuma hatası:', error);
    }
    
    return { accessToken: null, refreshToken: null };
  },

  // Token'ları temizle
  clearTokens: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }
};
```

### 2. Input Validation

```javascript
// utils/validation.js
export const validateEmail = (email) => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

export const validatePassword = (password) => {
  const minLength = password.length >= 8;
  const hasUpperCase = /[A-Z]/.test(password);
  const hasLowerCase = /[a-z]/.test(password);
  const hasNumbers = /\d/.test(password);
  const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);
  
  return {
    isValid: minLength && hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChar,
    errors: {
      minLength: !minLength ? 'En az 8 karakter olmalı' : null,
      hasUpperCase: !hasUpperCase ? 'En az 1 büyük harf olmalı' : null,
      hasLowerCase: !hasLowerCase ? 'En az 1 küçük harf olmalı' : null,
      hasNumbers: !hasNumbers ? 'En az 1 rakam olmalı' : null,
      hasSpecialChar: !hasSpecialChar ? 'En az 1 özel karakter olmalı' : null
    }
  };
};
```

---

## 🧪 Test Senaryoları

### 1. Unit Test Örneği

```javascript
// __tests__/AuthService.test.js
import { TE4ITAuthService } from '../services/AuthService';

describe('TE4ITAuthService', () => {
  let authService;

  beforeEach(() => {
    authService = new TE4ITAuthService('https://localhost:5001');
    localStorage.clear();
  });

  test('should save tokens to localStorage', () => {
    const accessToken = 'test-access-token';
    const refreshToken = 'test-refresh-token';
    
    authService.saveTokens(accessToken, refreshToken);
    
    expect(localStorage.getItem('accessToken')).toBe(accessToken);
    expect(localStorage.getItem('refreshToken')).toBe(refreshToken);
  });

  test('should clear tokens from localStorage', () => {
    authService.saveTokens('access', 'refresh');
    authService.clearTokens();
    
    expect(localStorage.getItem('accessToken')).toBeNull();
    expect(localStorage.getItem('refreshToken')).toBeNull();
  });
});
```

### 2. Integration Test Örneği

```javascript
// __tests__/LoginFlow.test.js
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from '../contexts/AuthContext';
import LoginForm from '../components/LoginForm';

const renderWithAuth = (component) => {
  return render(
    <BrowserRouter>
      <AuthProvider>
        {component}
      </AuthProvider>
    </BrowserRouter>
  );
};

test('should login successfully with valid credentials', async () => {
  // Mock fetch
  global.fetch = jest.fn(() =>
    Promise.resolve({
      ok: true,
      json: () => Promise.resolve({
        userId: '123',
        email: 'test@example.com',
        accessToken: 'mock-access-token',
        refreshToken: 'mock-refresh-token'
      })
    })
  );

  renderWithAuth(<LoginForm />);

  fireEvent.change(screen.getByLabelText(/email/i), {
    target: { value: 'test@example.com' }
  });
  
  fireEvent.change(screen.getByLabelText(/şifre/i), {
    target: { value: 'Password123!' }
  });
  
  fireEvent.click(screen.getByRole('button', { name: /giriş yap/i }));

  await waitFor(() => {
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/v1/auth/login'),
      expect.objectContaining({
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: 'test@example.com',
          password: 'Password123!'
        })
      })
    );
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

### Sorun Giderme

1. **CORS Hatası**: API base URL'ini kontrol edin
2. **Token Hatası**: Token'ların doğru kaydedildiğini kontrol edin
3. **Network Hatası**: Internet bağlantınızı kontrol edin
4. **Validation Hatası**: Form verilerinin doğru formatını kontrol edin

### İletişim

- **Email**: infoarslanbas@gmail.com
- **GitHub Issues**: https://github.com/burhanarslanbas/TE4IT/issues

---

*Bu dokümantasyon TE4IT Frontend Entegrasyon Rehberi v1.0 için hazırlanmıştır. Son güncelleme: Ocak 2025*
