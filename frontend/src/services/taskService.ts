/**
 * Task API Servisleri
 */
import { apiClient, ApiResponse } from './api';
import type {
  Task,
  CreateTaskRequest,
  UpdateTaskRequest,
  TaskListResponse,
  TaskFilters,
  TaskState,
  TaskRelation,
  CreateTaskRelationRequest,
} from '../types';

export class TaskService {
  /**
   * Use case task'larını getir
   */
  static async getTasks(useCaseId: string, filters?: TaskFilters): Promise<TaskListResponse> {
    const params = new URLSearchParams();
    
    if (filters?.page) params.append('page', filters.page.toString());
    if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
    if (filters?.state && filters.state !== 'All') {
      params.append('state', filters.state);
    }
    if (filters?.type && filters.type !== 'All') {
      params.append('type', filters.type);
    }
    if (filters?.assigneeId) params.append('assignee', filters.assigneeId);
    if (filters?.dueDate && filters.dueDate !== 'All') {
      params.append('dueDate', filters.dueDate);
    }
    if (filters?.search) params.append('search', filters.search);

    const queryString = params.toString();
    const endpoint = `/api/v1/usecases/${useCaseId}/tasks${queryString ? `?${queryString}` : ''}`;
    
    const response = await apiClient.get<TaskListResponse>(endpoint);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Task\'lar yüklenemedi');
  }

  /**
   * Task detayını getir
   */
  static async getTask(taskId: string): Promise<Task> {
    const response = await apiClient.get<Task>(`/api/v1/tasks/${taskId}`);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Task bulunamadı');
  }

  /**
   * Yeni task oluştur
   */
  static async createTask(useCaseId: string, data: CreateTaskRequest): Promise<Task> {
    const response = await apiClient.post<Task>(`/api/v1/usecases/${useCaseId}/tasks`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Task oluşturulamadı');
  }

  /**
   * Task güncelle
   */
  static async updateTask(taskId: string, data: UpdateTaskRequest): Promise<Task> {
    const response = await apiClient.put<Task>(`/api/v1/tasks/${taskId}`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Task güncellenemedi');
  }

  /**
   * Task durumunu değiştir
   */
  static async updateTaskState(taskId: string, state: TaskState): Promise<void> {
    const response = await apiClient.patch(`/api/v1/tasks/${taskId}/state`, { state });
    
    if (!response.success) {
      throw new Error('Task durumu güncellenemedi');
    }
  }

  /**
   * Task'a kullanıcı ata
   */
  static async assignTask(taskId: string, assigneeId: string): Promise<void> {
    const response = await apiClient.post(`/api/v1/tasks/${taskId}/assign`, { assigneeId });
    
    if (!response.success) {
      throw new Error('Task atanamadı');
    }
  }

  /**
   * Task sil
   */
  static async deleteTask(taskId: string): Promise<void> {
    const response = await apiClient.delete(`/api/v1/tasks/${taskId}`);
    
    if (!response.success) {
      throw new Error('Task silinemedi');
    }
  }

  /**
   * Task ilişkilerini getir
   */
  static async getTaskRelations(taskId: string): Promise<TaskRelation[]> {
    const response = await apiClient.get<TaskRelation[]>(`/api/v1/tasks/${taskId}/relations`);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Task ilişkileri yüklenemedi');
  }

  /**
   * Task ilişkisi oluştur
   */
  static async createTaskRelation(taskId: string, data: CreateTaskRelationRequest): Promise<TaskRelation> {
    const response = await apiClient.post<TaskRelation>(`/api/v1/tasks/${taskId}/relations`, data);
    
    if (response.success && response.data) {
      return response.data;
    }
    
    throw new Error('Task ilişkisi oluşturulamadı');
  }

  /**
   * Task ilişkisini sil
   */
  static async deleteTaskRelation(taskId: string, relationId: string): Promise<void> {
    const response = await apiClient.delete(`/api/v1/tasks/${taskId}/relations/${relationId}`);
    
    if (!response.success) {
      throw new Error('Task ilişkisi silinemedi');
    }
  }
}

