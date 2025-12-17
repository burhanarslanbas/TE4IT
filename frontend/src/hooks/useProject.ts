/**
 * useProject Hook
 * Project yönetimi için custom hook
 */
import { useState, useEffect, useCallback } from 'react';
import { ProjectService } from '../services/projectService';
import type { Project, ProjectFilters } from '../types';
import { ApiError } from '../core/errors/ApiError';
import { toast } from 'sonner';

interface UseProjectReturn {
  project: Project | null;
  loading: boolean;
  error: Error | null;
  refetch: () => Promise<void>;
}

export function useProject(projectId: string | undefined): UseProjectReturn {
  const [project, setProject] = useState<Project | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const loadProject = useCallback(async () => {
    if (!projectId) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const data = await ProjectService.getProject(projectId);
      setProject(data);
    } catch (err) {
      const apiError = err instanceof ApiError ? err : new Error('Proje yüklenemedi');
      setError(apiError);
      
      if (apiError instanceof ApiError && apiError.isAuthenticationError()) {
        toast.error('Oturum süreniz dolmuş', {
          description: 'Lütfen tekrar giriş yapın.',
        });
      } else {
        toast.error('Proje yüklenemedi', {
          description: apiError.message || 'Bir hata oluştu',
        });
      }
    } finally {
      setLoading(false);
    }
  }, [projectId]);

  useEffect(() => {
    loadProject();
  }, [loadProject]);

  return {
    project,
    loading,
    error,
    refetch: loadProject,
  };
}

