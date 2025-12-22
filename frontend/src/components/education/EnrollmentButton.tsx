/**
 * Enrollment Button Component
 * Kursa kayıt olma / Devam et butonu
 */

import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '../ui/button';
import { EducationService } from '../../services/educationService';
import { toast } from 'sonner';
import { Loader2, BookOpen, Play } from 'lucide-react';
import { ApiError } from '../../services/api';

interface EnrollmentButtonProps {
  courseId: string;
  isEnrolled: boolean;
  hasStarted?: boolean;
  onEnrollmentChange?: () => void;
}

export function EnrollmentButton({
  courseId,
  isEnrolled,
  hasStarted = false,
  onEnrollmentChange,
}: EnrollmentButtonProps) {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);

  const handleEnroll = async () => {
    try {
      setIsLoading(true);
      await EducationService.enrollInCourse(courseId);
      toast.success('Kursa başarıyla kayıt oldunuz!', {
        description: 'Şimdi kursa başlayabilirsiniz.',
        duration: 3000,
      });
      onEnrollmentChange?.();
      
      // Global event dispatch - CoursesListPage'de dinlenecek
      window.dispatchEvent(new CustomEvent('course-enrollment-changed', {
        detail: { courseId }
      }));
      
      // Kurs detay sayfasına yönlendir
      navigate(`/education/courses/${courseId}`);
    } catch (error: any) {
      // 400 hatası - zaten kayıtlı olabilir, sessizce handle et
      if (error instanceof ApiError && error.status === 400) {
        // Zaten kayıtlı olabilir, kurs detay sayfasına yönlendir
        toast.info('Zaten bu kursa kayıtlısınız', {
          description: 'Kursa yönlendiriliyorsunuz...',
          duration: 2000,
        });
        onEnrollmentChange?.();
        
        // Global event dispatch
        window.dispatchEvent(new CustomEvent('course-enrollment-changed', {
          detail: { courseId }
        }));
        
        // Kurs detay sayfasına yönlendir
        setTimeout(() => {
          navigate(`/education/courses/${courseId}`);
        }, 500);
        return;
      }
      
      toast.error('Kayıt işlemi başarısız', {
        description: error.message || 'Bir hata oluştu. Lütfen tekrar deneyin.',
        duration: 3000,
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleContinue = () => {
    navigate(`/education/courses/${courseId}/roadmap`);
  };

  if (isEnrolled) {
    return (
      <Button
        onClick={handleContinue}
        className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white"
        disabled={isLoading}
      >
        {isLoading ? (
          <>
            <Loader2 className="w-4 h-4 animate-spin" />
            Yükleniyor...
          </>
        ) : (
          <>
            <Play className="w-4 h-4" />
            {hasStarted ? 'Devam Et' : 'Kursa Başla'}
          </>
        )}
      </Button>
    );
  }

  return (
    <Button
      onClick={handleEnroll}
      className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white"
      disabled={isLoading}
    >
      {isLoading ? (
        <>
          <Loader2 className="w-4 h-4 animate-spin" />
          Kayıt olunuyor...
        </>
      ) : (
        <>
          <BookOpen className="w-4 h-4" />
          Kayıt Ol
        </>
      )}
    </Button>
  );
}

