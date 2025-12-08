/**
 * Task Service
 * Task ile ilgili tüm API çağrıları
 */

import { apiClient, ApiResponse } from './api';
import {
  Task,
  CreateTaskRequest,
  UpdateTaskRequest,
  ChangeTaskStateRequest,
  AssignTaskRequest,
  TaskRelation,
  CreateTaskRelationRequest,
} from '../types';

export const taskService = {
  /**
   * Task detayını getir
   */
  async getById(taskId: string): Promise<ApiResponse<Task>> {
    return apiClient.get<Task>(`/api/v1/tasks/${taskId}`);
  },

  /**
   * Task güncelle
   */
  async update(
    taskId: string,
    data: UpdateTaskRequest
  ): Promise<ApiResponse<Task>> {
    return apiClient.put<Task>(`/api/v1/tasks/${taskId}`, data);
  },

  /**
   * Task durumunu değiştir
   */
  async changeState(
    taskId: string,
    data: ChangeTaskStateRequest
  ): Promise<ApiResponse<Task>> {
    return apiClient.patch<Task>(`/api/v1/tasks/${taskId}/state`, data);
  },

  /**
   * Task'a kullanıcı ata
   */
  async assign(
    taskId: string,
    data: AssignTaskRequest
  ): Promise<ApiResponse<Task>> {
    return apiClient.post<Task>(`/api/v1/tasks/${taskId}/assign`, data);
  },

  /**
   * Task sil
   */
  async delete(taskId: string): Promise<ApiResponse<void>> {
    return apiClient.delete<void>(`/api/v1/tasks/${taskId}`);
  },

  /**
   * Task ilişkilerini getir
   */
  async getRelations(taskId: string): Promise<ApiResponse<TaskRelation[]>> {
    return apiClient.get<TaskRelation[]>(`/api/v1/tasks/${taskId}/relations`);
  },

  /**
   * Task ilişkisi oluştur
   */
  async createRelation(
    taskId: string,
    data: CreateTaskRelationRequest
  ): Promise<ApiResponse<TaskRelation>> {
    return apiClient.post<TaskRelation>(
      `/api/v1/tasks/${taskId}/relations`,
      data
    );
  },

  /**
   * Task ilişkisini sil
   */
  async deleteRelation(
    taskId: string,
    relationId: string
  ): Promise<ApiResponse<void>> {
    return apiClient.delete<void>(
      `/api/v1/tasks/${taskId}/relations/${relationId}`
    );
  },
};

