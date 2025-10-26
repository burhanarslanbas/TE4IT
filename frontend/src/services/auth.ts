/**
 * TE4IT Authentication Servisleri
 * Login, Register ve kullanıcı yönetimi için API servisleri
 */

import { apiClient, ApiResponse, ApiError } from './api';

// Re-export ApiError for convenience
export { ApiError };

// Kullanıcı tipleri
export interface User {
  id: string;
  userName: string;
  email: string;
  firstName?: string;
  lastName?: string;
  role?: string;
  createdAt: string;
  updatedAt: string;
}

// Login request tipi
export interface LoginRequest {
  email: string;
  password: string;
}

// Register request tipi
export interface RegisterRequest {
  userName: string;
  email: string;
  password: string;
  confirmPassword?: string; // Frontend validation için
}

// Login response tipi - Backend'den gelen format
export interface LoginResponse {
  userId: string;
  email: string;
  accessToken: string;
  expiresAt: string;
  refreshToken: string;
  refreshExpires: string;
}

// Register response tipi - Backend'den gelen format
export interface RegisterResponse {
  userId: string;
  email: string;
  accessToken: string;
  expiresAt: string;
  refreshToken: string;
  refreshExpires: string;
}

// Refresh token request tipi
export interface RefreshTokenRequest {
  refreshToken: string;
}

// Refresh token response tipi
export interface RefreshTokenResponse {
  accessToken: string;
  expiresAt: string;
  refreshToken: string;
  refreshExpires: string;
}

// Change password request tipi
export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// Change password response tipi
export interface ChangePasswordResponse {
  success: boolean;
  message: string;
}

// Forgot password response tipi
export interface ForgotPasswordResponse {
  success: boolean;
  message: string;
}

// Reset password response tipi
export interface ResetPasswordResponse {
  success: boolean;
  message: string;
}

/**
 * Authentication Servisleri
 */
export class AuthService {
  /**
   * Kullanıcı girişi yap
   * @param credentials - Email ve şifre
   * @returns Login response
   */
  static async login(credentials: LoginRequest): Promise<LoginResponse> {
    try {
      const response = await apiClient.post<LoginResponse>(
        '/api/v1/auth/login',
        {
          email: credentials.email,
          password: credentials.password,
        }
      );

      if (response.success && response.data) {
        // Token'ları kaydet
        apiClient.setToken(response.data.accessToken);
        localStorage.setItem('refreshToken', response.data.refreshToken);
        
        return response.data;
      }

      throw new ApiError('Giriş başarısız');
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Giriş sırasında bir hata oluştu');
    }
  }

  /**
   * Yeni kullanıcı kaydı
   * @param userData - Kullanıcı bilgileri
   * @returns Register response
   */
  static async register(userData: RegisterRequest): Promise<RegisterResponse> {
    try {
      // Confirm password'u backend'e gönderme
      const { confirmPassword, ...dataToSend } = userData;

      const response = await apiClient.post<RegisterResponse>(
        '/api/v1/auth/register',
        dataToSend
      );

      if (response.success && response.data) {
        // Token'ları kaydet
        apiClient.setToken(response.data.accessToken);
        localStorage.setItem('refreshToken', response.data.refreshToken);
        
        return response.data;
      }

      throw new ApiError('Kayıt başarısız');
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Kayıt sırasında bir hata oluştu');
    }
  }

  /**
   * Access token'ı yenile
   * @param refreshToken - Refresh token
   * @returns Refresh token response
   */
  static async refreshAccessToken(refreshToken?: string): Promise<RefreshTokenResponse> {
    try {
      // Eğer refreshToken verilmemişse localStorage'dan al
      const tokenToUse = refreshToken || localStorage.getItem('refreshToken');
      
      if (!tokenToUse) {
        throw new ApiError('Refresh token bulunamadı');
      }

      const response = await apiClient.post<RefreshTokenResponse>(
        '/api/v1/auth/refreshToken',
        { refreshToken: tokenToUse }
      );

      if (response.success && response.data) {
        // Yeni token'ları kaydet
        apiClient.setToken(response.data.accessToken);
        localStorage.setItem('refreshToken', response.data.refreshToken);
        
        return response.data;
      }

      throw new ApiError('Token yenileme başarısız');
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Token yenilenirken hata oluştu');
    }
  }

  /**
   * Kullanıcı çıkışı
   * Token'ları her durumda temizler - Backend isteği opsiyonel
   */
  static async logout(): Promise<void> {
    try {
      // Token'ları temizle
      apiClient.clearToken();
      localStorage.removeItem('refreshToken');
    } catch (error) {
      // Logout hatası önemli değil, token'ı temizle
      console.warn('Logout hatası:', error);
    } finally {
      // Her durumda token'ları temizle (double check)
      apiClient.clearToken();
      localStorage.removeItem('refreshToken');
    }
  }

  /**
   * Mevcut kullanıcı bilgilerini getir
   * @returns Kullanıcı bilgileri
   */
  static async getCurrentUser(): Promise<User> {
    try {
      const response = await apiClient.get<User>('/api/v1/auth/me');

      if (response.success && response.data) {
        return response.data;
      }

      throw new ApiError('Kullanıcı bilgileri alınamadı');
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Kullanıcı bilgileri alınırken hata oluştu');
    }
  }

  /**
   * Token'ın geçerli olup olmadığını kontrol et
   * @returns Token geçerli mi?
   */
  static isAuthenticated(): boolean {
    return apiClient.isAuthenticated();
  }

  /**
   * Şifre sıfırlama isteği gönder (Forgot Password)
   * @param email - Kullanıcı email'i
   * @returns Forgot password response
   */
  static async requestPasswordReset(email: string): Promise<ForgotPasswordResponse> {
    try {
      const response = await apiClient.post<ForgotPasswordResponse>(
        '/api/v1/auth/forgotPassword',
        { email }
      );

      if (response.success && response.data) {
        return response.data;
      }

      throw new ApiError('Şifre sıfırlama isteği başarısız');
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Şifre sıfırlama isteği gönderilirken hata oluştu');
    }
  }

  /**
   * Şifre sıfırlama (Reset Password)
   * @param email - Kullanıcı email'i
   * @param token - Reset token
   * @param newPassword - Yeni şifre
   * @returns Reset password response
   */
  static async resetPassword(email: string, token: string, newPassword: string): Promise<ResetPasswordResponse> {
    try {
      const response = await apiClient.post<ResetPasswordResponse>(
        '/api/v1/auth/resetPassword',
        {
          email,
          token,
          newPassword
        }
      );

      if (response.success && response.data) {
        return response.data;
      }

      throw new ApiError('Şifre sıfırlama başarısız');
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Şifre sıfırlanırken hata oluştu');
    }
  }

  /**
   * Şifre değiştirme (Change Password)
   * @param currentPassword - Mevcut şifre
   * @param newPassword - Yeni şifre
   * @returns Change password response
   */
  static async changePassword(currentPassword: string, newPassword: string): Promise<ChangePasswordResponse> {
    try {
      const response = await apiClient.post<ChangePasswordResponse>(
        '/api/v1/auth/changePassword',
        {
          currentPassword,
          newPassword
        }
      );

      if (response.success && response.data) {
        return response.data;
      }

      throw new ApiError('Şifre değiştirme başarısız');
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Şifre değiştirilirken hata oluştu');
    }
  }
}

/**
 * Form validation yardımcı fonksiyonları
 */
export class ValidationHelper {
  /**
   * Email formatını kontrol et
   */
  static isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  /**
   * Şifre gücünü kontrol et
   */
  static validatePassword(password: string): {
    isValid: boolean;
    errors: string[];
  } {
    const errors: string[] = [];

    if (password.length < 8) {
      errors.push('Şifre en az 8 karakter olmalıdır');
    }

    if (!/\d/.test(password)) {
      errors.push('Şifre en az bir rakam içermelidir');
    }

    if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
      errors.push('Şifre en az bir özel karakter içermelidir');
    }

    return {
      isValid: errors.length === 0,
      errors,
    };
  }

  /**
   * Kullanıcı adı formatını kontrol et
   */
  static isValidUsername(username: string): boolean {
    // En az 3 karakter, sadece harf, rakam ve alt çizgi
    const usernameRegex = /^[a-zA-Z0-9_]{3,}$/;
    return usernameRegex.test(username);
  }
}

/**
 * JWT Token Decode Helper
 * Token'dan kullanıcı bilgilerini decode eder
 */
export class TokenHelper {
  /**
   * JWT token'dan payload'ı decode et
   * @param token - JWT token string
   * @returns Decoded payload veya null
   */
  static decodeToken(token: string): any | null {
    try {
      // JWT token formatı: header.payload.signature
      const parts = token.split('.');
      
      if (parts.length !== 3) {
        console.error('Geçersiz JWT token formatı');
        return null;
      }

      // Payload kısmını decode et (base64)
      const payload = parts[1];
      const decodedPayload = atob(payload);
      
      // JSON olarak parse et
      return JSON.parse(decodedPayload);
    } catch (error) {
      console.error('Token decode hatası:', error);
      return null;
    }
  }

  /**
   * Token'dan kullanıcı bilgilerini getir
   * @returns Kullanıcı bilgileri veya null
   */
  static getCurrentUser(): User | null {
    try {
      const token = localStorage.getItem('te4it_token');
      
      if (!token) {
        return null;
      }

      const decoded = this.decodeToken(token);
      
      if (!decoded) {
        return null;
      }

      // Token'dan kullanıcı bilgilerini map et
      // Tüm olası field'ları kontrol et
      return {
        id: decoded.userId || decoded.sub || decoded.id || '',
        userName: decoded.userName || decoded.username || decoded.preferred_username || decoded.unique_name || '',
        email: decoded.email || decoded.emailAddress || decoded.eMail || '',
        firstName: decoded.firstName || decoded.given_name || decoded.first_name || '',
        lastName: decoded.lastName || decoded.family_name || decoded.last_name || '',
        role: decoded.role || decoded.roles || '',
        createdAt: decoded.createdAt || decoded.iat || '',
        updatedAt: decoded.updatedAt || decoded.exp || '',
      };
    } catch (error) {
      console.error('Kullanıcı bilgisi alınırken hata:', error);
      return null;
    }
  }

  /**
   * Token'ın expire tarihini kontrol et
   * @returns Expire date veya null
   */
  static getTokenExpirationDate(): Date | null {
    try {
      const token = localStorage.getItem('te4it_token');
      
      if (!token) {
        return null;
      }

      const decoded = this.decodeToken(token);
      
      if (!decoded || !decoded.exp) {
        return null;
      }

      // exp Unix timestamp (seconds)
      return new Date(decoded.exp * 1000);
    } catch (error) {
      console.error('Token expiration kontrolü hatası:', error);
      return null;
    }
  }

  /**
   * Token'ın süresi dolmuş mu?
   * @returns Token geçerli mi?
   */
  static isTokenExpired(): boolean {
    try {
      const expirationDate = this.getTokenExpirationDate();
      
      if (!expirationDate) {
        return true;
      }

      return expirationDate < new Date();
    } catch (error) {
      console.error('Token expiration kontrolü hatası:', error);
      return true;
    }
  }
}
