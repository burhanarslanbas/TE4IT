/**
 * UseCase Service
 * UseCase ile ilgili tüm API çağrıları
 */

import { apiClient, ApiResponse } from './api';
import {
  UseCase,
  CreateUseCaseRequest,
  UpdateUseCaseRequest,
  PaginationParams,
  PaginationResponse,
  UseCaseFilters,
  ChangeStatusRequest,
  Task,
  TaskFilters,
  CreateTaskRequest,
} from '../types';

// Backend response type (isActive boolean olarak geliyor)
interface BackendUseCase {
  id: string;
  moduleId: string;
  title: string;
  description?: string;
  importantNotes?: string;
  isActive: boolean;
  startedDate: string;
  taskCount?: number;
  createdDate?: string;
  updatedDate?: string;
}

interface BackendUseCaseListResponse {
  items: BackendUseCase[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Backend UseCase response'unu frontend UseCase tipine çevir
 */
function mapBackendUseCaseToFrontend(backendUseCase: BackendUseCase): UseCase {
  return {
    id: backendUseCase.id,
    moduleId: backendUseCase.moduleId,
    title: backendUseCase.title,
    description: backendUseCase.description,
    importantNotes: backendUseCase.importantNotes,
    status: backendUseCase.isActive ? 'Active' : 'Archived',
    taskCount: backendUseCase.taskCount,
    createdAt: backendUseCase.createdDate || backendUseCase.startedDate,
    updatedAt: backendUseCase.updatedDate || backendUseCase.startedDate,
  };
}

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
    const endpoint = `/api/v1/UseCases/modules/${moduleId}${queryString ? `?${queryString}` : ''}`;
    
    const response = await apiClient.get<BackendUseCaseListResponse>(endpoint);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return {
        items: response.data.items.map(mapBackendUseCaseToFrontend),
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
      };
    }
    
    throw new Error('Use case\'ler yüklenemedi');
  }

  /**
   * Use case detayını getir
   */
  static async getUseCase(useCaseId: string): Promise<UseCase> {
    const response = await apiClient.get<BackendUseCase>(`/api/v1/UseCases/${useCaseId}`);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return mapBackendUseCaseToFrontend(response.data);
    }
    
    throw new Error('Use case bulunamadı');
  }

  /**
   * Yeni use case oluştur
   */
  static async createUseCase(moduleId: string, data: CreateUseCaseRequest): Promise<UseCase> {
    try {
      const response = await apiClient.post<BackendUseCase>(`/api/v1/UseCases/modules/${moduleId}`, data);
      
      if (response.success && response.data) {
        // Backend response'unu frontend tipine çevir
        return mapBackendUseCaseToFrontend(response.data);
      }
      
      throw new Error(response.message || 'Use case oluşturulamadı');
    } catch (error: any) {
      console.error('CreateUseCase Error:', error);
      throw error;
    }
  }

  /**
   * Use case güncelle
   */
  static async updateUseCase(useCaseId: string, data: UpdateUseCaseRequest): Promise<UseCase> {
    const response = await apiClient.put<BackendUseCase>(`/api/v1/UseCases/${useCaseId}`, data);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return mapBackendUseCaseToFrontend(response.data);
    }
    
    throw new Error('Use case güncellenemedi');
  }

  /**
   * Use case durumunu değiştir (Active/Archived)
   * Backend { isActive: boolean } bekliyor
   */
  static async updateUseCaseStatus(useCaseId: string, isActive: boolean): Promise<BackendUseCase> {
    const response = await apiClient.patch<BackendUseCase>(
      `/api/v1/UseCases/${useCaseId}/status`, 
      { isActive }
    );
    
    if (!response.success || !response.data) {
      throw new Error('Use case durumu güncellenemedi');
    }
    
    // Response'u döndür - caller refetch için kullanabilir
    return response.data;
  }

  /**
   * Use case sil
   * @throws {Error} 409 - Use case içinde task'lar var
   * @throws {Error} 404 - Use case bulunamadı
   * @throws {Error} 403 - Yetki yok
   */
  static async deleteUseCase(useCaseId: string): Promise<void> {
    try {
      console.log('[DELETE USECASE] Starting delete for use case:', useCaseId);
      const response = await apiClient.delete(`/api/v1/UseCases/${useCaseId}`);
      
      if (!response.success) {
        throw new Error(response.message || 'Use case silinemedi');
      }
      
      console.log('[DELETE USECASE] Successfully deleted use case:', useCaseId);
    } catch (error: any) {
      console.error('[DELETE USECASE] Error:', error);
      
      // 409 Conflict - Alt öğeler var
      if (error.statusCode === 409 || error.status === 409) {
        throw new Error('Bu use case\'i silebilmek için önce task\'ları silmeniz gerekiyor.');
      }
      
      // 404 Not Found - Zaten silinmiş
      if (error.statusCode === 404 || error.status === 404) {
        throw new Error('Use case bulunamadı. Zaten silinmiş olabilir.');
      }
      
      // 403 Forbidden - Yetki yok
      if (error.statusCode === 403 || error.status === 403) {
        throw new Error('Bu use case\'i silmek için yetkiniz bulunmamaktadır.');
      }
      
      // Backend'den gelen hata mesajını koru
      if (error.message) {
        throw error;
      }
      
      throw new Error('Use case silinemedi');
    }
  }
}

