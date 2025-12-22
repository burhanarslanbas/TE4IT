/**
 * useCourseProgress Hook
 * Kurs ilerlemesi için custom hook
 */

import { useState, useEffect } from 'react';
import { EducationService } from '../services/educationService';
import type { CourseProgressResponse } from '../types/education';
import { ApiError } from '../services/api';

export function useCourseProgress(courseId: string | undefined) {
  const [progress, setProgress] = useState<CourseProgressResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!courseId) {
      setLoading(false);
      return;
    }

    const loadProgress = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await EducationService.getUserProgress(courseId);
        setProgress(data);
      } catch (err) {
        console.error('Error loading progress:', err);
        // Progress yoksa hata verme, sadece null döndür
        if (err instanceof ApiError && err.statusCode !== 404) {
          setError(err.message || 'İlerleme yüklenemedi');
        }
        setProgress(null);
      } finally {
        setLoading(false);
      }
    };

    loadProgress();
  }, [courseId]);

  const refetch = async () => {
    if (!courseId) return;

    try {
      setLoading(true);
      setError(null);
      const data = await EducationService.getUserProgress(courseId);
      setProgress(data);
    } catch (err) {
      console.error('Error refetching progress:', err);
      if (err instanceof ApiError && err.statusCode !== 404) {
        setError(err.message || 'İlerleme yüklenemedi');
      }
      setProgress(null);
    } finally {
      setLoading(false);
    }
  };

  return { progress, loading, error, refetch };
}

