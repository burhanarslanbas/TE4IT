/**
 * Task Relation API Servisleri
 */
import { apiClient } from './api';
import type { TaskRelation, CreateTaskRelationRequest, TaskRelationType } from '../types';

export class TaskRelationService {
  /**
   * Task ilişkilerini getir
   */
  static async getTaskRelations(taskId: string): Promise<TaskRelation[]> {
    try {
      console.log('[TASK RELATIONS] Fetching relations for task:', taskId);
      const response = await apiClient.get<TaskRelation[]>(`/api/v1/Tasks/${taskId}/relations`);
      
      if (response.success && response.data) {
        console.log('[TASK RELATIONS] Relations loaded:', response.data.length);
        return response.data;
      }
      
      return [];
    } catch (error: any) {
      console.error('[TASK RELATIONS] Error fetching relations:', error);
      // 404 ise boş array dön (ilişki yok)
      if (error.statusCode === 404 || error.status === 404) {
        return [];
      }
      throw error;
    }
  }

  /**
   * Task ilişkisi oluştur
   */
  static async createTaskRelation(
    taskId: string,
    targetTaskId: string,
    relationType: TaskRelationType
  ): Promise<void> {
    try {
      const payload: CreateTaskRelationRequest = {
        targetTaskId,
        relationType,
      };

      console.log('[TASK RELATIONS] Creating relation:', { taskId, payload });
      const response = await apiClient.post(`/api/v1/Tasks/${taskId}/relations`, payload);
      
      if (!response.success) {
        throw new Error(response.message || 'İlişki oluşturulamadı');
      }
      
      console.log('[TASK RELATIONS] Relation created successfully');
    } catch (error: any) {
      console.error('[TASK RELATIONS] Error creating relation:', error);
      throw error;
    }
  }

  /**
   * Task ilişkisini sil
   */
  static async deleteTaskRelation(taskId: string, relationId: string): Promise<void> {
    try {
      console.log('[TASK RELATIONS] Deleting relation:', { taskId, relationId });
      const response = await apiClient.delete(`/api/v1/Tasks/${taskId}/relations/${relationId}`);
      
      if (!response.success) {
        throw new Error(response.message || 'İlişki silinemedi');
      }
      
      console.log('[TASK RELATIONS] Relation deleted successfully');
    } catch (error: any) {
      console.error('[TASK RELATIONS] Error deleting relation:', error);
      throw error;
    }
  }
}
