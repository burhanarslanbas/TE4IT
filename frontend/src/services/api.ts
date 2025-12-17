/**
 * TE4IT API Servisleri
 * Backend API ile iletişim için temel konfigürasyon ve yardımcı fonksiyonlar
 * Güvenlik iyileştirmeleri ile
 */

// Import API configuration
import { API_BASE_URL } from '../config/config';
import type { IApiClient } from '../core/interfaces/IApiClient';
import { ApiError as CoreApiError } from '../core/errors/ApiError';
import {
  getToken,
  saveToken,
  getRefreshToken,
  saveRefreshToken,
  clearTokens,
  isTokenExpired,
  isTokenValid,
} from '../utils/tokenManager';
import { AuthService } from './auth';

// API Response tipleri
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

// Re-export ApiError for backward compatibility
export { ApiError } from '../core/errors/ApiError';

/**
 * API istekleri için temel fetch wrapper
 * Tüm API çağrıları için ortak hata yönetimi ve token ekleme
 * Güvenlik iyileştirmeleri: Token expiration kontrolü, otomatik yenileme, 401/403 handling
 */
export class ApiClient implements IApiClient {
  private baseURL: string;
  private isRefreshing = false;
  private refreshPromise: Promise<string> | null = null;
  private onUnauthorizedCallback?: () => void;

  constructor(baseURL: string = API_BASE_URL) {
    this.baseURL = baseURL;
  }

  /**
   * Unauthorized callback'i ayarla (logout için)
   */
  setUnauthorizedCallback(callback: () => void): void {
    this.onUnauthorizedCallback = callback;
  }

  /**
   * JWT token'ı kaydet (sessionStorage kullan)
   */
  setToken(token: string, expiresAt?: string): void {
    saveToken(token, expiresAt);
  }

  /**
   * JWT token'ı temizle
   */
  clearToken(): void {
    clearTokens();
  }

  /**
   * Token'ın geçerli olup olmadığını kontrol et
   */
  isAuthenticated(): boolean {
    return isTokenValid();
  }

  /**
   * Token'ı yenile (otomatik)
   */
  private async refreshToken(): Promise<string> {
    // Eğer zaten refresh işlemi devam ediyorsa, o promise'i döndür
    if (this.isRefreshing && this.refreshPromise) {
      return this.refreshPromise;
    }

    this.isRefreshing = true;
    this.refreshPromise = (async () => {
      try {
        const refreshTokenValue = getRefreshToken();
        if (!refreshTokenValue) {
          throw new CoreApiError('Refresh token bulunamadı', 401);
        }

        const response = await AuthService.refreshAccessToken(refreshTokenValue);
        
        // Yeni token'ları kaydet
        this.setToken(response.accessToken, response.expiresAt);
        saveRefreshToken(response.refreshToken);

        return response.accessToken;
      } catch (error) {
        // Refresh başarısız, logout yap
        this.clearToken();
        if (this.onUnauthorizedCallback) {
          this.onUnauthorizedCallback();
        }
        throw error;
      } finally {
        this.isRefreshing = false;
        this.refreshPromise = null;
      }
    })();

    return this.refreshPromise;
  }

  /**
   * URL'yi düzgün birleştir (çift slash sorununu önler)
   */
  private joinUrl(base: string, path: string): string {
    // Base URL'in sonundaki slash'ı temizle
    const cleanBase = base.replace(/\/+$/, '');
    // Path'in başındaki slash'ı temizle ve gerekirse ekle
    const cleanPath = path.replace(/^\/+/, '');
    return `${cleanBase}/${cleanPath}`;
  }

  /**
   * API isteği gönder
   * Güvenlik: Token expiration kontrolü, otomatik yenileme, 401/403 handling
   */
  async request<T>(
    endpoint: string,
    options: RequestInit = {},
    retryOn401: boolean = true
  ): Promise<ApiResponse<T>> {
    try {
      // Token'ı kontrol et ve gerekirse yenile
      let token = getToken();
      
      if (token && isTokenExpired(token)) {
        // Token süresi dolmuş, yenile
        try {
          token = await this.refreshToken();
        } catch (refreshError) {
          // Refresh başarısız, logout yapıldı
          throw new CoreApiError('Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.', 401);
        }
      }

      // URL'yi tamamla (çift slash sorununu önle)
      const url = this.joinUrl(this.baseURL, endpoint);

      // Headers'ı hazırla
      const headers: HeadersInit = {
        'Content-Type': 'application/json',
        ...options.headers,
      };

      // Eğer token varsa Authorization header'ına ekle
      if (token) {
        headers.Authorization = `Bearer ${token}`;
      }

      // Fetch isteği gönder
      const response = await fetch(url, {
        ...options,
        headers,
      });

      // Response'u parse et
      let data: any;
      const contentType = response.headers.get('content-type');
      
      if (contentType && contentType.includes('application/json')) {
        try {
          data = await response.json();
        } catch (parseError) {
          // JSON parse hatası
          throw new CoreApiError('Sunucudan geçersiz yanıt alındı', response.status);
        }
      } else {
        // JSON değilse text olarak oku
        const text = await response.text();
        data = { message: text || 'Bir hata oluştu' };
      }

      // HTTP status kodunu kontrol et
      if (!response.ok) {
        // Backend bazen { message: { message: string, errors: [...] } } gibi dönebiliyor.
        const rawMessage = data?.message;
        const message =
          typeof rawMessage === 'string'
            ? rawMessage
            : typeof rawMessage?.message === 'string'
              ? rawMessage.message
              : 'Bir hata oluştu';

        const rawErrors = data?.errors ?? rawMessage?.errors;
        let errors: string[] | undefined;
        if (Array.isArray(rawErrors)) {
          if (rawErrors.length === 0) {
            errors = undefined;
          } else if (typeof rawErrors[0] === 'string') {
            errors = rawErrors as string[];
          } else if (typeof rawErrors[0] === 'object' && rawErrors[0] !== null) {
            errors = (rawErrors as any[]).map((e) => {
              if (typeof e?.message === 'string') {
                return typeof e?.field === 'string' ? `${e.field}: ${e.message}` : e.message;
              }
              try {
                return JSON.stringify(e);
              } catch {
                return 'Bir hata oluştu';
              }
            });
          }
        }

        const apiError = new CoreApiError(message, response.status, errors);

        // 401 Unauthorized - Token geçersiz veya süresi dolmuş
        if (response.status === 401 && retryOn401) {
          try {
            // Token'ı yenile ve isteği tekrar dene
            const newToken = await this.refreshToken();
            
            // Yeni token ile isteği tekrar gönder (sadece bir kez)
            return this.request<T>(endpoint, options, false);
          } catch (refreshError) {
            // Refresh başarısız, logout yap
            this.clearToken();
            if (this.onUnauthorizedCallback) {
              this.onUnauthorizedCallback();
            }
            throw new CoreApiError('Oturum süreniz dolmuş. Lütfen tekrar giriş yapın.', 401);
          }
        }

        // 403 Forbidden - Yetki yok
        if (response.status === 403) {
          throw new CoreApiError(
            'Bu işlem için yetkiniz bulunmamaktadır.',
            403,
            data.errors,
            'FORBIDDEN'
          );
        }

        throw apiError;
      }

      return {
        success: true,
        data: data,
        message: data.message,
      };
    } catch (error) {
      // Network hatası veya parse hatası
      if (error instanceof CoreApiError) {
        throw error;
      }

      // Network hatası
      if (error instanceof TypeError && error.message.includes('fetch')) {
        throw new CoreApiError(
          'Bağlantı hatası. Lütfen internet bağlantınızı kontrol edin.',
          0,
          undefined,
          'NETWORK_ERROR'
        );
      }

      throw new CoreApiError(
        'Beklenmeyen bir hata oluştu',
        0,
        undefined,
        'UNKNOWN_ERROR'
      );
    }
  }

  /**
   * GET isteği gönder
   */
  async get<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, { method: 'GET' });
  }

  /**
   * POST isteği gönder
   */
  async post<T>(endpoint: string, data?: any): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PUT isteği gönder
   */
  async put<T>(endpoint: string, data?: any): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * PATCH isteği gönder
   */
  async patch<T>(endpoint: string, data?: any): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      method: 'PATCH',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  /**
   * DELETE isteği gönder
   */
  async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, { method: 'DELETE' });
  }
}

// Global API client instance
export const apiClient = new ApiClient();
