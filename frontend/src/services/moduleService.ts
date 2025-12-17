/**
 * Module API Servisleri
 */
import { apiClient, ApiResponse } from './api';
import type {
  Module,
  CreateModuleRequest,
  UpdateModuleRequest,
  ModuleListResponse,
  ModuleFilters,
} from '../types';

// Backend response type (isActive boolean olarak geliyor)
interface BackendModule {
  id: string;
  projectId: string;
  title: string;
  description?: string;
  isActive: boolean;
  startedDate: string;
  useCaseCount?: number;
  createdDate?: string;
  updatedDate?: string;
}

interface BackendModuleListResponse {
  items: BackendModule[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Backend Module response'unu frontend Module tipine çevir
 */
function mapBackendModuleToFrontend(backendModule: BackendModule): Module {
  return {
    id: backendModule.id,
    projectId: backendModule.projectId,
    title: backendModule.title,
    description: backendModule.description,
    status: backendModule.isActive ? 'Active' : 'Archived',
    useCaseCount: backendModule.useCaseCount,
    createdAt: backendModule.createdDate || backendModule.startedDate,
    updatedAt: backendModule.updatedDate || backendModule.startedDate,
  };
}

export class ModuleService {
  /**
   * Proje modüllerini getir
   */
  static async getModules(projectId: string, filters?: ModuleFilters): Promise<ModuleListResponse> {
    const params = new URLSearchParams();
    
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.status && filters.status !== 'All') {
      params.append('status', filters.status);
    }
    if (filters?.search) params.append('search', filters.search);

    const queryString = params.toString();
    const endpoint = `/api/v1/Modules/projects/${projectId}${queryString ? `?${queryString}` : ''}`;
    
    const response = await apiClient.get<BackendModuleListResponse>(endpoint);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return {
        items: response.data.items.map(mapBackendModuleToFrontend),
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
      };
    }
    
    throw new Error('Modüller yüklenemedi');
  }

  /**
   * Modül detayını getir
   */
  static async getModule(moduleId: string): Promise<Module> {
    const response = await apiClient.get<BackendModule>(`/api/v1/Modules/${moduleId}`);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return mapBackendModuleToFrontend(response.data);
    }
    
    throw new Error('Modül bulunamadı');
  }

  /**
   * Yeni modül oluştur
   */
  static async createModule(projectId: string, data: CreateModuleRequest): Promise<Module> {
    const response = await apiClient.post<BackendModule>(`/api/v1/Modules/projects/${projectId}`, data);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return mapBackendModuleToFrontend(response.data);
    }
    
    throw new Error('Modül oluşturulamadı');
  }

  /**
   * Modül güncelle
   */
  static async updateModule(moduleId: string, data: UpdateModuleRequest): Promise<Module> {
    const response = await apiClient.put<BackendModule>(`/api/v1/Modules/${moduleId}`, data);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return mapBackendModuleToFrontend(response.data);
    }
    
    throw new Error('Modül güncellenemedi');
  }

  /**
   * Modül durumunu değiştir (Active/Archived)
   * Backend { isActive: boolean } bekliyor
   */
  static async updateModuleStatus(moduleId: string, isActive: boolean): Promise<BackendModule> {
    const response = await apiClient.patch<BackendModule>(
      `/api/v1/Modules/${moduleId}/status`, 
      { isActive }
    );
    
    if (!response.success || !response.data) {
      throw new Error('Modül durumu güncellenemedi');
    }
    
    // Response'u döndür - caller refetch için kullanabilir
    return response.data;
  }

  /**
   * Modül sil
   * @throws {Error} 409 - Modül içinde use case'ler var
   * @throws {Error} 404 - Modül bulunamadı
   * @throws {Error} 403 - Yetki yok
   */
  static async deleteModule(moduleId: string): Promise<void> {
    try {
      console.log('[DELETE MODULE] Starting delete for module:', moduleId);
      const response = await apiClient.delete(`/api/v1/Modules/${moduleId}`);
      
      if (!response.success) {
        throw new Error(response.message || 'Modül silinemedi');
      }
      
      console.log('[DELETE MODULE] Successfully deleted module:', moduleId);
    } catch (error: any) {
      console.error('[DELETE MODULE] Error:', error);
      
      // 409 Conflict - Alt öğeler var
      if (error.statusCode === 409 || error.status === 409) {
        throw new Error('Bu modülü silebilmek için önce use case\'leri silmeniz gerekiyor.');
      }
      
      // 404 Not Found - Zaten silinmiş
      if (error.statusCode === 404 || error.status === 404) {
        throw new Error('Modül bulunamadı. Zaten silinmiş olabilir.');
      }
      
      // 403 Forbidden - Yetki yok
      if (error.statusCode === 403 || error.status === 403) {
        throw new Error('Bu modülü silmek için yetkiniz bulunmamaktadır.');
      }
      
      // Backend'den gelen hata mesajını koru
      if (error.message) {
        throw error;
      }
      
      throw new Error('Modül silinemedi');
    }
  }
}

