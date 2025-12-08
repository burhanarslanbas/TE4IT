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

export const useCaseService = {
  /**
   * Use case detayını getir
   */
  async getById(useCaseId: string): Promise<ApiResponse<UseCase>> {
    return apiClient.get<UseCase>(`/api/v1/usecases/${useCaseId}`);
  },

  /**
   * Use case güncelle
   */
  async update(
    useCaseId: string,
    data: UpdateUseCaseRequest
  ): Promise<ApiResponse<UseCase>> {
    return apiClient.put<UseCase>(`/api/v1/usecases/${useCaseId}`, data);
  },

  /**
   * Use case durumunu değiştir (Active/Archived)
   */
  async changeStatus(
    useCaseId: string,
    data: ChangeStatusRequest
  ): Promise<ApiResponse<UseCase>> {
    return apiClient.patch<UseCase>(
      `/api/v1/usecases/${useCaseId}/status`,
      data
    );
  },

  /**
   * Use case sil
   */
  async delete(useCaseId: string): Promise<ApiResponse<void>> {
    return apiClient.delete<void>(`/api/v1/usecases/${useCaseId}`);
  },

  /**
   * Use case task'larını getir
   */
  async getTasks(
    useCaseId: string,
    params: PaginationParams & TaskFilters
  ): Promise<ApiResponse<PaginationResponse<Task>>> {
    const queryParams = new URLSearchParams({
      page: params.page.toString(),
      pageSize: params.pageSize.toString(),
    });

    if (params.state !== null && params.state !== undefined) {
      queryParams.append('state', params.state);
    }

    if (params.type !== null && params.type !== undefined) {
      queryParams.append('type', params.type);
    }

    if (params.assigneeId) {
      queryParams.append('assignee', params.assigneeId);
    }

    if (params.dueDate) {
      queryParams.append('dueDate', params.dueDate);
    }

    if (params.search) {
      queryParams.append('search', params.search);
    }

    return apiClient.get<PaginationResponse<Task>>(
      `/api/v1/usecases/${useCaseId}/tasks?${queryParams.toString()}`
    );
  },

  /**
   * Use case için task oluştur
   */
  async createTask(
    useCaseId: string,
    data: CreateTaskRequest
  ): Promise<ApiResponse<Task>> {
    return apiClient.post<Task>(
      `/api/v1/usecases/${useCaseId}/tasks`,
      data
    );
  },
};

