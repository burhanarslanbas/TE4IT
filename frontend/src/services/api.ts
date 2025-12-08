/**
 * TE4IT API Servisleri
 * Backend API ile iletişim için temel konfigürasyon ve yardımcı fonksiyonlar
 */

// Import API configuration
import { API_BASE_URL } from '../config/config';

// API Response tipleri
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

// API Error tipi
export interface ApiError {
  message: string;
  status?: number;
  errors?: string[];
}

/**
 * API istekleri için temel fetch wrapper
 * Tüm API çağrıları için ortak hata yönetimi ve token ekleme
 */
export class ApiClient {
  private baseURL: string;
  private token: string | null = null;

  constructor(baseURL: string = API_BASE_URL) {
    this.baseURL = baseURL;
    // localStorage'dan token'ı yükle
    this.loadToken();
  }

  /**
   * localStorage'dan JWT token'ı yükle
   */
  private loadToken(): void {
    this.token = localStorage.getItem('te4it_token');
  }

  /**
   * JWT token'ı kaydet
   */
  setToken(token: string): void {
    this.token = token;
    localStorage.setItem('te4it_token', token);
  }

  /**
   * JWT token'ı temizle
   */
  clearToken(): void {
    this.token = null;
    localStorage.removeItem('te4it_token');
  }

  /**
   * Token'ın geçerli olup olmadığını kontrol et
   */
  isAuthenticated(): boolean {
    return !!this.token;
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
   */
  async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<ApiResponse<T>> {
    try {
      // Token'ı her request'te yeniden yükle (localStorage güncel olabilir)
      this.loadToken();
      
      // URL'yi tamamla (çift slash sorununu önle)
      const url = this.joinUrl(this.baseURL, endpoint);

      // Headers'ı hazırla
      const headers: HeadersInit = {
        'Content-Type': 'application/json',
        ...options.headers,
      };

      // Eğer token varsa Authorization header'ına ekle
      if (this.token) {
        headers.Authorization = `Bearer ${this.token}`;
      }

      // Fetch isteği gönder
      const response = await fetch(url, {
        ...options,
        headers,
      });

      // Response'u parse et
      const data = await response.json();

      // HTTP status kodunu kontrol et
      if (!response.ok) {
        throw new ApiError(
          data.message || 'Bir hata oluştu',
          response.status,
          data.errors
        );
      }

      return {
        success: true,
        data: data,
        message: data.message,
      };
    } catch (error) {
      // Network hatası veya parse hatası
      if (error instanceof ApiError) {
        throw error;
      }

      throw new ApiError(
        'Bağlantı hatası. Lütfen internet bağlantınızı kontrol edin.',
        0
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

// Custom Error class
export class ApiError extends Error {
  public status?: number;
  public errors?: string[];

  constructor(message: string, status?: number, errors?: string[]) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.errors = errors;
  }
}
