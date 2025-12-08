/**
 * Module Service
 * Module ile ilgili tüm API çağrıları
 */

import { apiClient, ApiResponse } from './api';
import {
  Module,
  CreateModuleRequest,
  UpdateModuleRequest,
  PaginationParams,
  PaginationResponse,
  ModuleFilters,
  ChangeStatusRequest,
  UseCase,
  UseCaseFilters,
  CreateUseCaseRequest,
} from '../types';

export const moduleService = {
  /**
   * Modül detayını getir
   */
  async getById(moduleId: string): Promise<ApiResponse<Module>> {
    return apiClient.get<Module>(`/api/v1/modules/${moduleId}`);
  },

  /**
   * Modül güncelle
   */
  async update(
    moduleId: string,
    data: UpdateModuleRequest
  ): Promise<ApiResponse<Module>> {
    return apiClient.put<Module>(`/api/v1/modules/${moduleId}`, data);
  },

  /**
   * Modül durumunu değiştir (Active/Archived)
   */
  async changeStatus(
    moduleId: string,
    data: ChangeStatusRequest
  ): Promise<ApiResponse<Module>> {
    return apiClient.patch<Module>(
      `/api/v1/modules/${moduleId}/status`,
      data
    );
  },

  /**
   * Modül sil
   */
  async delete(moduleId: string): Promise<ApiResponse<void>> {
    return apiClient.delete<void>(`/api/v1/modules/${moduleId}`);
  },

  /**
   * Modül use case'lerini getir
   */
  async getUseCases(
    moduleId: string,
    params: PaginationParams & UseCaseFilters
  ): Promise<ApiResponse<PaginationResponse<UseCase>>> {
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

    return apiClient.get<PaginationResponse<UseCase>>(
      `/api/v1/modules/${moduleId}/usecases?${queryParams.toString()}`
    );
  },

  /**
   * Modül için use case oluştur
   */
  async createUseCase(
    moduleId: string,
    data: CreateUseCaseRequest
  ): Promise<ApiResponse<UseCase>> {
    return apiClient.post<UseCase>(
      `/api/v1/modules/${moduleId}/usecases`,
      data
    );
  },
};

