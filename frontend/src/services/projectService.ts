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
    
    const response = await apiClient.get<ProjectListResponse>(endpoint);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Projeler yüklenemedi');
  }

  /**
   * Proje detayını getir
   */
  static async getProject(projectId: string): Promise<Project> {
    const response = await apiClient.get<Project>(`/api/v1/projects/${projectId}`);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Proje bulunamadı');
  }

  /**
   * Yeni proje oluştur
   */
  static async createProject(data: CreateProjectRequest): Promise<Project> {
    const response = await apiClient.post<Project>('/api/v1/projects', data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Proje oluşturulamadı');
  }

  /**
   * Proje güncelle
   */
  static async updateProject(projectId: string, data: UpdateProjectRequest): Promise<Project> {
    const response = await apiClient.put<Project>(`/api/v1/projects/${projectId}`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Proje güncellenemedi');
  }

  /**
   * Proje durumunu değiştir (Active/Archived)
   */
  static async updateProjectStatus(projectId: string, status: 'Active' | 'Archived'): Promise<void> {
    const response = await apiClient.patch(`/api/v1/projects/${projectId}/status`, { status });
    
    if (!response.success) {
      throw new Error('Proje durumu güncellenemedi');
    }
  }

  /**
   * Proje sil
   */
  static async deleteProject(projectId: string): Promise<void> {
    const response = await apiClient.delete(`/api/v1/projects/${projectId}`);
    
    if (!response.success) {
      throw new Error('Proje silinemedi');
    }
  }
}

