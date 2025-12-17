/**
 * useTaskRelations Hook
 * Task ilişkilerini yönetmek için custom hook
 */
import { useState, useEffect, useCallback } from 'react';
import { TaskRelationService } from '../services/taskRelationService';
import type { TaskRelation, TaskRelationType } from '../types';
import { toast } from 'sonner';

interface UseTaskRelationsReturn {
  relations: TaskRelation[];
  loading: boolean;
  error: Error | null;
  refetch: () => Promise<void>;
  createRelation: (targetTaskId: string, relationType: TaskRelationType) => Promise<void>;
  deleteRelation: (relationId: string) => Promise<void>;
}

export function useTaskRelations(taskId: string | undefined): UseTaskRelationsReturn {
  const [relations, setRelations] = useState<TaskRelation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchRelations = useCallback(async () => {
    if (!taskId) {
      setRelations([]);
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      
      const data = await TaskRelationService.getTaskRelations(taskId);
      setRelations(data);
    } catch (err: any) {
      console.error('[USE TASK RELATIONS] Error loading relations:', err);
      setError(err);
      setRelations([]);
    } finally {
      setLoading(false);
    }
  }, [taskId]);

  const createRelation = useCallback(async (targetTaskId: string, relationType: TaskRelationType) => {
    if (!taskId) return;

    try {
      await TaskRelationService.createTaskRelation(taskId, targetTaskId, relationType);
      
      // İlişkileri yeniden fetch et
      await fetchRelations();
      
      toast.success('İlişki oluşturuldu');
    } catch (err: any) {
      console.error('[USE TASK RELATIONS] Error creating relation:', err);
      toast.error('İlişki oluşturulamadı', {
        description: err.message || 'Bir hata oluştu',
      });
      throw err;
    }
  }, [taskId, fetchRelations]);

  const deleteRelation = useCallback(async (relationId: string) => {
    if (!taskId) return;

    try {
      await TaskRelationService.deleteTaskRelation(taskId, relationId);
      
      // İlişkileri yeniden fetch et
      await fetchRelations();
      
      toast.success('İlişki silindi');
    } catch (err: any) {
      console.error('[USE TASK RELATIONS] Error deleting relation:', err);
      toast.error('İlişki silinemedi', {
        description: err.message || 'Bir hata oluştu',
      });
      throw err;
    }
  }, [taskId, fetchRelations]);

  useEffect(() => {
    fetchRelations();
  }, [fetchRelations]);

  return {
    relations,
    loading,
    error,
    refetch: fetchRelations,
    createRelation,
    deleteRelation,
  };
}
