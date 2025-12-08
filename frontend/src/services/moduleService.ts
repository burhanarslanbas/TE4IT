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
    const endpoint = `/api/v1/projects/${projectId}/modules${queryString ? `?${queryString}` : ''}`;
    
    const response = await apiClient.get<ModuleListResponse>(endpoint);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Modüller yüklenemedi');
  }

  /**
   * Modül detayını getir
   */
  static async getModule(moduleId: string): Promise<Module> {
    const response = await apiClient.get<Module>(`/api/v1/modules/${moduleId}`);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Modül bulunamadı');
  }

  /**
   * Yeni modül oluştur
   */
  static async createModule(projectId: string, data: CreateModuleRequest): Promise<Module> {
    const response = await apiClient.post<Module>(`/api/v1/projects/${projectId}/modules`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Modül oluşturulamadı');
  }

  /**
   * Modül güncelle
   */
  static async updateModule(moduleId: string, data: UpdateModuleRequest): Promise<Module> {
    const response = await apiClient.put<Module>(`/api/v1/modules/${moduleId}`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Modül güncellenemedi');
  }

  /**
   * Modül durumunu değiştir (Active/Archived)
   */
  static async updateModuleStatus(moduleId: string, status: 'Active' | 'Archived'): Promise<void> {
    const response = await apiClient.patch(`/api/v1/modules/${moduleId}/status`, { status });
    
    if (!response.success) {
      throw new Error('Modül durumu güncellenemedi');
    }
  }

  /**
   * Modül sil
   */
  static async deleteModule(moduleId: string): Promise<void> {
    const response = await apiClient.delete(`/api/v1/modules/${moduleId}`);
    
    if (!response.success) {
      throw new Error('Modül silinemedi');
    }
  }
}

