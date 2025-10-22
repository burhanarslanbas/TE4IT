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
        // Token'ı kaydet - Backend'den gelen accessToken'ı kullan
        apiClient.setToken(response.data.accessToken);
        
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
        // Token'ı kaydet - Backend'den gelen accessToken'ı kullan
        apiClient.setToken(response.data.accessToken);
        
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
   * Kullanıcı çıkışı
   */
  static async logout(): Promise<void> {
    try {
      // Backend'e logout isteği gönder (opsiyonel)
      await apiClient.post('/api/v1/auth/logout');
    } catch (error) {
      // Logout hatası önemli değil, token'ı temizle
      console.warn('Logout API hatası:', error);
    } finally {
      // Her durumda token'ı temizle
      apiClient.clearToken();
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
   * Şifre sıfırlama isteği gönder
   * @param email - Kullanıcı email'i
   */
  static async requestPasswordReset(email: string): Promise<void> {
    try {
      const response = await apiClient.post('/api/v1/auth/forgot-password', {
        email,
      });

      if (!response.success) {
        throw new ApiError('Şifre sıfırlama isteği başarısız');
      }
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Şifre sıfırlama isteği gönderilirken hata oluştu');
    }
  }

  /**
   * Şifre sıfırlama
   * @param token - Reset token
   * @param newPassword - Yeni şifre
   */
  static async resetPassword(token: string, newPassword: string): Promise<void> {
    try {
      const response = await apiClient.post('/api/v1/auth/reset-password', {
        token,
        password: newPassword,
      });

      if (!response.success) {
        throw new ApiError('Şifre sıfırlama başarısız');
      }
    } catch (error) {
      if (error instanceof ApiError) {
        throw error;
      }
      throw new ApiError('Şifre sıfırlanırken hata oluştu');
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
