/**
 * useEnrollment Hook
 * Kurs kayıt işlemleri için custom hook
 */

import { useState } from 'react';
import { EducationService } from '../services/educationService';
import { toast } from 'sonner';
import type { EnrollmentResponse } from '../types/education';
import { ApiError } from '../services/api';

export function useEnrollment() {
  const [loading, setLoading] = useState(false);

  const enroll = async (courseId: string): Promise<EnrollmentResponse | null> => {
    try {
      setLoading(true);
      const response = await EducationService.enrollInCourse(courseId);
      toast.success('Kursa başarıyla kayıt oldunuz!', {
        description: 'Şimdi kursa başlayabilirsiniz.',
        duration: 3000,
      });
      return response;
    } catch (error) {
      console.error('Error enrolling in course:', error);
      if (error instanceof ApiError) {
        toast.error('Kayıt işlemi başarısız', {
          description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
          duration: 3000,
        });
      } else {
        toast.error('Kayıt işlemi başarısız', {
          description: 'Bir hata oluştu. Lütfen tekrar deneyin.',
          duration: 3000,
        });
      }
      return null;
    } finally {
      setLoading(false);
    }
  };

  return { enroll, loading };
}

