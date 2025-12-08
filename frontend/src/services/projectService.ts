/**
 * Project Service
 * Project ile ilgili tüm API çağrıları
 */

import { apiClient, ApiResponse } from './api';
import {
  Project,
  CreateProjectRequest,
  UpdateProjectRequest,
  PaginationParams,
  PaginationResponse,
  ProjectFilters,
  ChangeStatusRequest,
  Module,
  ModuleFilters,
  CreateModuleRequest,
} from '../types';

export const projectService = {
  /**
   * Proje listesini getir
   */
  async list(
    params: PaginationParams & ProjectFilters
  ): Promise<ApiResponse<PaginationResponse<Project>>> {
    const queryParams = new URLSearchParams({
      page: params.page.toString(),
      pageSize: params.pageSize.toString(),
    });

    if (params.isActive !== null && params.isActive !== undefined) {
      queryParams.append('isActive', params.isActive.toString());
    }

    if (params.search) {
      queryParams.append('search', params.search);
    }

    return apiClient.get<PaginationResponse<Project>>(
      `/api/v1/projects?${queryParams.toString()}`
    );
  },

  /**
   * Proje detayını getir
   */
  async getById(projectId: string): Promise<ApiResponse<Project>> {
    return apiClient.get<Project>(`/api/v1/projects/${projectId}`);
  },

  /**
   * Yeni proje oluştur
   */
  async create(
    data: CreateProjectRequest
  ): Promise<ApiResponse<Project>> {
    return apiClient.post<Project>('/api/v1/projects', data);
  },

  /**
   * Proje güncelle
   */
  async update(
    projectId: string,
    data: UpdateProjectRequest
  ): Promise<ApiResponse<Project>> {
    return apiClient.put<Project>(`/api/v1/projects/${projectId}`, data);
  },

  /**
   * Proje durumunu değiştir (Active/Archived)
   */
  async changeStatus(
    projectId: string,
    data: ChangeStatusRequest
  ): Promise<ApiResponse<Project>> {
    return apiClient.patch<Project>(
      `/api/v1/projects/${projectId}/status`,
      data
    );
  },

  /**
   * Proje sil
   */
  async delete(projectId: string): Promise<ApiResponse<void>> {
    return apiClient.delete<void>(`/api/v1/projects/${projectId}`);
  },

  /**
   * Proje modüllerini getir
   */
  async getModules(
    projectId: string,
    params: PaginationParams & ModuleFilters
  ): Promise<ApiResponse<PaginationResponse<Module>>> {
    const queryParams = new URLSearchParams({
      page: params.page.toString(),
      pageSize: params.pageSize.toString(),
    });

    if (params.isActive !== null && params.isActive !== undefined) {
      queryParams.append('status', params.isActive ? 'Active' : 'Archived');
    }

    if (params.search) {
      queryParams.append('search', params.search);
    }

    return apiClient.get<PaginationResponse<Module>>(
      `/api/v1/projects/${projectId}/modules?${queryParams.toString()}`
    );
  },

  /**
   * Proje için modül oluştur
   */
  async createModule(
    projectId: string,
    data: CreateModuleRequest
  ): Promise<ApiResponse<Module>> {
    return apiClient.post<Module>(
      `/api/v1/projects/${projectId}/modules`,
      data
    );
  },
};

