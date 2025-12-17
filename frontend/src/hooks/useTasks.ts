/**
 * useTasks Hook
 * Use case task'larını yönetmek için custom hook
 */
import { useState, useEffect, useCallback } from 'react';
import { TaskService, TaskFilters } from '../services/taskService';
import type { Task } from '../types';

interface UseTasksReturn {
  tasks: Task[];
  loading: boolean;
  error: Error | null;
  totalCount: number;
  totalPages: number;
  refetch: () => Promise<void>;
}

export function useTasks(useCaseId: string | undefined, filters?: TaskFilters): UseTasksReturn {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  const fetchTasks = useCallback(async () => {
    if (!useCaseId) {
      setTasks([]);
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      console.log('[USE TASKS] Fetching tasks for use case:', useCaseId);
      
      const response = await TaskService.getTasks(useCaseId, filters);
      
      console.log('[USE TASKS] Tasks loaded:', response.items.length);
      setTasks(response.items);
      setTotalCount(response.totalCount);
      setTotalPages(response.totalPages);
    } catch (err: any) {
      console.error('[USE TASKS] Error loading tasks:', err);
      setError(err);
      setTasks([]);
    } finally {
      setLoading(false);
    }
  }, [useCaseId, filters?.page, filters?.pageSize, filters?.state, filters?.type, filters?.search]);

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  return {
    tasks,
    loading,
    error,
    totalCount,
    totalPages,
    refetch: fetchTasks,
  };
}
