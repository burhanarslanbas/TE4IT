/**
 * API Client Interface
 * Dependency Inversion Principle için
 */
import type { ApiResponse } from '../../services/api';

export interface IApiClient {
  get<T>(endpoint: string): Promise<ApiResponse<T>>;
  post<T>(endpoint: string, data?: any): Promise<ApiResponse<T>>;
  put<T>(endpoint: string, data?: any): Promise<ApiResponse<T>>;
  patch<T>(endpoint: string, data?: any): Promise<ApiResponse<T>>;
  delete<T>(endpoint: string): Promise<ApiResponse<T>>;
  setToken(token: string): void;
  clearToken(): void;
  isAuthenticated(): boolean;
}

