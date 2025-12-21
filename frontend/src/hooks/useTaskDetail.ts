/**
 * useTaskDetail Hook
 * Task detayını yönetmek için custom hook
 */
import { useState, useEffect, useCallback } from 'react';
import { TaskService } from '../services/taskService';
import type { Task, TaskState } from '../types';
import { toast } from 'sonner';

interface UseTaskDetailReturn {
  task: Task | null;
  loading: boolean;
  error: Error | null;
  refetch: () => Promise<void>;
  updateState: (taskId: string, state: TaskState) => Promise<void>;
  assignTask: (taskId: string, assigneeId: string) => Promise<void>;
}

export function useTaskDetail(taskId: string | undefined): UseTaskDetailReturn {
  const [task, setTask] = useState<Task | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchTask = useCallback(async () => {
    if (!taskId) {
      setTask(null);
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      console.log('[USE TASK DETAIL] Fetching task:', taskId);
      
      const taskData = await TaskService.getTask(taskId);
      
      console.log('[USE TASK DETAIL] Task loaded:', taskData);
      setTask(taskData);
    } catch (err: any) {
      console.error('[USE TASK DETAIL] Error loading task:', err);
      setError(err);
      setTask(null);
      toast.error('Task yüklenemedi', {
        description: err.message || 'Bir hata oluştu',
      });
    } finally {
      setLoading(false);
    }
  }, [taskId]);

  const updateState = useCallback(async (taskId: string, state: TaskState) => {
    try {
      console.log('[USE TASK DETAIL] Updating state:', { taskId, state });
      await TaskService.updateTaskState(taskId, state);
      
      // Task'ı yeniden fetch et
      await fetchTask();
      
      toast.success('Task durumu güncellendi');
    } catch (err: any) {
      console.error('[USE TASK DETAIL] Error updating state:', err);
      toast.error('Task durumu güncellenemedi', {
        description: err.message || 'Bir hata oluştu',
      });
      throw err;
    }
  }, [fetchTask]);

  const assignTask = useCallback(async (taskId: string, assigneeId: string) => {
    try {
      console.log('[USE TASK DETAIL] Assigning task:', { taskId, assigneeId });
      await TaskService.assignTask(taskId, assigneeId);
      
      // Task'ı yeniden fetch et
      await fetchTask();
      
      toast.success('Task atandı');
    } catch (err: any) {
      console.error('[USE TASK DETAIL] Error assigning task:', err);
      toast.error('Task ataması yapılamadı', {
        description: err.message || 'Bir hata oluştu',
      });
      throw err;
    }
  }, [fetchTask]);

  useEffect(() => {
    fetchTask();
  }, [fetchTask]);

  return {
    task,
    loading,
    error,
    refetch: fetchTask,
    updateState,
    assignTask,
  };
}

