/**
 * UseCase API Servisleri
 */
import { apiClient, ApiResponse } from './api';
import type {
  UseCase,
  CreateUseCaseRequest,
  UpdateUseCaseRequest,
  UseCaseListResponse,
  UseCaseFilters,
} from '../types';

export class UseCaseService {
  /**
   * Modül use case'lerini getir
   */
  static async getUseCases(moduleId: string, filters?: UseCaseFilters): Promise<UseCaseListResponse> {
    const params = new URLSearchParams();
    
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.status && filters.status !== 'All') {
      params.append('status', filters.status);
    }
    if (filters?.search) params.append('search', filters.search);

    const queryString = params.toString();
    const endpoint = `/api/v1/modules/${moduleId}/usecases${queryString ? `?${queryString}` : ''}`;
    
    const response = await apiClient.get<UseCaseListResponse>(endpoint);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Use case\'ler yüklenemedi');
  }

  /**
   * Use case detayını getir
   */
  static async getUseCase(useCaseId: string): Promise<UseCase> {
    const response = await apiClient.get<UseCase>(`/api/v1/usecases/${useCaseId}`);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Use case bulunamadı');
  }

  /**
   * Yeni use case oluştur
   */
  static async createUseCase(moduleId: string, data: CreateUseCaseRequest): Promise<UseCase> {
    const response = await apiClient.post<UseCase>(`/api/v1/modules/${moduleId}/usecases`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Use case oluşturulamadı');
  }

  /**
   * Use case güncelle
   */
  static async updateUseCase(useCaseId: string, data: UpdateUseCaseRequest): Promise<UseCase> {
    const response = await apiClient.put<UseCase>(`/api/v1/usecases/${useCaseId}`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Use case güncellenemedi');
  }

  /**
   * Use case durumunu değiştir (Active/Archived)
   */
  static async updateUseCaseStatus(useCaseId: string, status: 'Active' | 'Archived'): Promise<void> {
    const response = await apiClient.patch(`/api/v1/usecases/${useCaseId}/status`, { status });
    
    if (!response.success) {
      throw new Error('Use case durumu güncellenemedi');
    }
  }

  /**
   * Use case sil
   */
  static async deleteUseCase(useCaseId: string): Promise<void> {
    const response = await apiClient.delete(`/api/v1/usecases/${useCaseId}`);
    
    if (!response.success) {
      throw new Error('Use case silinemedi');
    }
  }
}

