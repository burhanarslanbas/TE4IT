/**
 * useModules Hook
 * Module yönetimi için custom hook
 */
import { useState, useEffect, useCallback } from 'react';
import { ModuleService } from '../services/moduleService';
import type { Module, ModuleFilters, ModuleListResponse } from '../types';
import { ApiError } from '../core/errors/ApiError';
import { toast } from 'sonner';

interface UseModulesReturn {
  modules: Module[];
  loading: boolean;
  error: Error | null;
  totalPages: number;
  totalCount: number;
  refetch: () => Promise<void>;
}

export function useModules(
  projectId: string | undefined,
  filters?: ModuleFilters
): UseModulesReturn {
  const [modules, setModules] = useState<Module[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const loadModules = useCallback(async () => {
    if (!projectId) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const response: ModuleListResponse = await ModuleService.getModules(projectId, filters);
      setModules(response.items);
      setTotalPages(response.totalPages);
      setTotalCount(response.totalCount);
    } catch (err) {
      const apiError = err instanceof ApiError ? err : new Error('Modüller yüklenemedi');
      setError(apiError);
      
      if (apiError instanceof ApiError && apiError.isAuthenticationError()) {
        toast.error('Oturum süreniz dolmuş', {
          description: 'Lütfen tekrar giriş yapın.',
        });
      } else {
        toast.error('Modüller yüklenemedi', {
          description: apiError.message || 'Bir hata oluştu',
        });
      }
    } finally {
      setLoading(false);
    }
  }, [projectId, filters]);

  useEffect(() => {
    loadModules();
  }, [loadModules]);

  return {
    modules,
    loading,
    error,
    totalPages,
    totalCount,
    refetch: loadModules,
  };
}

