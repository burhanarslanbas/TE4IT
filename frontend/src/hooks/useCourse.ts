/**
 * useCourse Hook
 * Kurs detayı için custom hook
 */

import { useState, useEffect } from 'react';
import { EducationService } from '../services/educationService';
import type { CourseDetailResponse } from '../types/education';
import { ApiError } from '../services/api';

export function useCourse(courseId: string | undefined) {
  const [course, setCourse] = useState<CourseDetailResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!courseId) {
      setLoading(false);
      return;
    }

    const loadCourse = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await EducationService.getCourseById(courseId);
        setCourse(data);
      } catch (err) {
        console.error('Error loading course:', err);
        if (err instanceof ApiError) {
          setError(err.message || 'Kurs yüklenemedi');
        } else {
          setError('Kurs yüklenemedi');
        }
      } finally {
        setLoading(false);
      }
    };

    loadCourse();
  }, [courseId]);

  const refetch = async () => {
    if (!courseId) return;

    try {
      setLoading(true);
      setError(null);
      const data = await EducationService.getCourseById(courseId);
      setCourse(data);
    } catch (err) {
      console.error('Error refetching course:', err);
      if (err instanceof ApiError) {
        setError(err.message || 'Kurs yüklenemedi');
      } else {
        setError('Kurs yüklenemedi');
      }
    } finally {
      setLoading(false);
    }
  };

  return { course, loading, error, refetch };
}

