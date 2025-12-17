/**
 * Project API Servisleri
 */
import { apiClient, ApiResponse } from './api';
import type {
  Project,
  CreateProjectRequest,
  UpdateProjectRequest,
  ProjectListResponse,
  ProjectFilters,
} from '../types';

// Backend response type (IsActive boolean olarak geliyor)
interface BackendProject {
  id: string;
  title: string;
  description?: string;
  isActive: boolean;
  startedDate: string;
  createdDate?: string;
  updatedDate?: string;
}

interface BackendProjectListResponse {
  items: BackendProject[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Backend Project response'unu frontend Project tipine çevir
 */
function mapBackendProjectToFrontend(backendProject: BackendProject): Project {
  return {
    id: backendProject.id,
    title: backendProject.title,
    description: backendProject.description,
    status: backendProject.isActive ? 'Active' : 'Archived',
    startedDate: backendProject.startedDate,
    createdAt: backendProject.createdDate || backendProject.startedDate,
    updatedAt: backendProject.updatedDate || backendProject.startedDate,
  };
}

export class ProjectService {
  /**
   * Proje listesini getir
   */
  static async getProjects(filters?: ProjectFilters): Promise<ProjectListResponse> {
    const params = new URLSearchParams();
    
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.isActive !== undefined) params.append('isActive', filters.isActive.toString());
    if (filters?.search) params.append('search', filters.search);

    const queryString = params.toString();
    const endpoint = `/api/v1/projects${queryString ? `?${queryString}` : ''}`;
    
    const response = await apiClient.get<BackendProjectListResponse>(endpoint);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return {
        items: response.data.items.map(mapBackendProjectToFrontend),
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
      };
    }
    
    throw new Error('Projeler yüklenemedi');
  }

  /**
   * Proje detayını getir
   */
  static async getProject(projectId: string): Promise<Project> {
    const response = await apiClient.get<BackendProject>(`/api/v1/projects/${projectId}`);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return mapBackendProjectToFrontend(response.data);
    }
    
    throw new Error('Proje bulunamadı');
  }

  /**
   * Yeni proje oluştur
   */
  static async createProject(data: CreateProjectRequest): Promise<Project> {
    try {
      const response = await apiClient.post<BackendProject>('/api/v1/projects', data);
      
      if (response.success && response.data) {
        // Backend response'unu frontend tipine çevir
        return mapBackendProjectToFrontend(response.data);
      }
      
      // Response başarılı ama data yoksa
      throw new Error(response.message || 'Proje oluşturulamadı');
    } catch (error: any) {
      // ApiError ise mesajını kullan
      if (error instanceof Error) {
        throw error;
      }
      // Diğer hatalar için genel mesaj
      throw new Error(error?.message || 'Proje oluşturulamadı. Lütfen tekrar deneyin.');
    }
  }

  /**
   * Proje güncelle
   */
  static async updateProject(projectId: string, data: UpdateProjectRequest): Promise<Project> {
    const response = await apiClient.put<BackendProject>(`/api/v1/projects/${projectId}`, data);
    
    if (response.success && response.data) {
      // Backend response'unu frontend tipine çevir
      return mapBackendProjectToFrontend(response.data);
    }
    
    throw new Error('Proje güncellenemedi');
  }

  /**
   * Proje durumunu değiştir (Active/Archived)
   * Backend IsActive (boolean) bekliyor, bu yüzden status string'ini boolean'a çeviriyoruz
   */
  static async updateProjectStatus(projectId: string, status: 'Active' | 'Archived'): Promise<void> {
    // Backend IsActive (boolean) bekliyor, status string'ini boolean'a çevir
    const isActive = status === 'Active';
    const response = await apiClient.patch(`/api/v1/projects/${projectId}/status`, { IsActive: isActive });
    
    if (!response.success) {
      throw new Error('Proje durumu güncellenemedi');
    }
  }

  /**
   * Proje sil
   * @throws {Error} 409 - Proje içinde modüller var
   * @throws {Error} 404 - Proje bulunamadı
   * @throws {Error} 403 - Yetki yok
   */
  static async deleteProject(projectId: string): Promise<void> {
    try {
      console.log('[DELETE PROJECT] Starting delete for project:', projectId);
      const response = await apiClient.delete(`/api/v1/projects/${projectId}`);
      
      if (!response.success) {
        throw new Error(response.message || 'Proje silinemedi');
      }
      
      console.log('[DELETE PROJECT] Successfully deleted project:', projectId);
    } catch (error: any) {
      console.error('[DELETE PROJECT] Error:', error);
      
      // 409 Conflict - Alt öğeler var
      if (error.statusCode === 409 || error.status === 409) {
        throw new Error('Bu projeyi silebilmek için önce modülleri silmeniz gerekiyor.');
      }
      
      // 404 Not Found - Zaten silinmiş
      if (error.statusCode === 404 || error.status === 404) {
        throw new Error('Proje bulunamadı. Zaten silinmiş olabilir.');
      }
      
      // 403 Forbidden - Yetki yok
      if (error.statusCode === 403 || error.status === 403) {
        throw new Error('Bu projeyi silmek için yetkiniz bulunmamaktadır.');
      }
      
      // Backend'den gelen hata mesajını koru
      if (error.message) {
        throw error;
      }
      
      throw new Error('Proje silinemedi');
    }
  }
}

