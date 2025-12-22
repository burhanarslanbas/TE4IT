/**
 * Video Player Component
 * Video içerik oynatıcı - react-player kullanır
 */

import { useState, useEffect, useRef } from 'react';
import ReactPlayer from 'react-player';
import { Button } from '../ui/button';
import { CheckCircle2, Loader2 } from 'lucide-react';
import { EducationService } from '../../services/educationService';
import { toast } from 'sonner';
import type { CourseContent } from '../../types/education';

interface VideoPlayerProps {
  content: CourseContent;
  courseId: string;
  onComplete?: () => void;
}

export function VideoPlayer({ content, courseId, onComplete }: VideoPlayerProps) {
  const [isCompleted, setIsCompleted] = useState(false);
  const [isCompleting, setIsCompleting] = useState(false);
  const [watchedPercentage, setWatchedPercentage] = useState(0);
  const [hasStarted, setHasStarted] = useState(false);
  const progressUpdateInterval = useRef<NodeJS.Timeout | null>(null);

  // Video URL'i embed URL'e çevir (eğer embedUrl yoksa)
  const videoUrl = content.embedUrl || content.linkUrl || '';

  // Video ilerlemesini takip et (throttled)
  useEffect(() => {
    if (!hasStarted || isCompleted) return;

    // Periyodik olarak ilerleme güncelle (her 10 saniyede bir)
    progressUpdateInterval.current = setInterval(async () => {
      if (watchedPercentage > 0 && watchedPercentage < 100) {
        try {
          await EducationService.updateVideoProgress(content.id, courseId, {
            watchedPercentage: Math.round(watchedPercentage),
            timeSpentSeconds: Math.round(watchedPercentage * 0.1), // Tahmini süre
            isCompleted: false,
          });
        } catch (error) {
          console.warn('Video progress update failed:', error);
        }
      }
    }, 10000); // 10 saniyede bir

    return () => {
      if (progressUpdateInterval.current) {
        clearInterval(progressUpdateInterval.current);
      }
    };
  }, [hasStarted, watchedPercentage, isCompleted, content.id]);

  // %80+ izlendiğinde otomatik tamamla
  useEffect(() => {
    if (watchedPercentage >= 80 && !isCompleted && hasStarted) {
      handleAutoComplete();
    }
  }, [watchedPercentage, isCompleted, hasStarted]);

  const handleAutoComplete = async () => {
    if (isCompleted || isCompleting) return;

    try {
      setIsCompleting(true);
      await EducationService.completeContent(content.id, courseId, {
        timeSpentMinutes: Math.round(watchedPercentage * 0.1), // Tahmini süre
      });
      setIsCompleted(true);
      toast.success('Video tamamlandı!', {
        description: 'İçerik başarıyla tamamlandı olarak işaretlendi.',
        duration: 3000,
      });
      onComplete?.();
    } catch (error: any) {
      toast.error('Hata', {
        description: error.message || 'Video tamamlanamadı',
      });
    } finally {
      setIsCompleting(false);
    }
  };

  const handleManualComplete = async () => {
    if (isCompleted) return;

    try {
      setIsCompleting(true);
      await EducationService.completeContent(content.id, courseId, {
        timeSpentMinutes: Math.round(watchedPercentage * 0.1),
      });
      setIsCompleted(true);
      toast.success('Video tamamlandı!', {
        description: 'İçerik başarıyla tamamlandı olarak işaretlendi.',
        duration: 3000,
      });
      onComplete?.();
    } catch (error: any) {
      toast.error('Hata', {
        description: error.message || 'Video tamamlanamadı',
      });
    } finally {
      setIsCompleting(false);
    }
  };

  return (
    <div className="space-y-4">
      <div className="bg-[#161B22]/50 border border-[#30363D] rounded-xl overflow-hidden">
        {/* Video Player Container */}
        <div className="relative w-full" style={{ paddingTop: '56.25%' }}> {/* 16:9 aspect ratio */}
          <div className="absolute inset-0 bg-[#0D1117]">
            {videoUrl ? (
              <ReactPlayer
                url={videoUrl}
                width="100%"
                height="100%"
                controls
                playing={false}
                onStart={() => setHasStarted(true)}
                onProgress={({ played }) => {
                  const percentage = played * 100;
                  setWatchedPercentage(percentage);
                }}
                onEnded={() => {
                  setWatchedPercentage(100);
                  if (!isCompleted) {
                    handleAutoComplete();
                  }
                }}
                config={{
                  youtube: {
                    playerVars: {
                      modestbranding: 1,
                      rel: 0,
                    },
                  },
                  vimeo: {
                    playerOptions: {
                      responsive: true,
                    },
                  },
                }}
              />
            ) : (
              <div className="flex items-center justify-center h-full text-center text-[#9CA3AF]">
                <div>
                  <Loader2 className="w-8 h-8 animate-spin mx-auto mb-2" />
                  <p>Video yükleniyor...</p>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Video Info */}
        <div className="p-6">
          <h2 className="text-xl font-semibold text-[#E5E7EB] mb-2">
            {content.title}
          </h2>
          {content.platform && (
            <p className="text-sm text-[#9CA3AF] mb-4">
              Platform: {content.platform}
            </p>
          )}

          {/* Progress Indicator */}
          {hasStarted && (
            <div className="mb-4">
              <div className="flex items-center justify-between mb-2">
                <span className="text-sm text-[#9CA3AF]">İzlenme Durumu</span>
                <span className="text-sm text-[#2DD4BF]">
                  %{Math.round(watchedPercentage)}
                </span>
              </div>
              <div className="w-full h-2 bg-[#30363D] rounded-full overflow-hidden">
                <div
                  className="h-full bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] rounded-full transition-all duration-300"
                  style={{ width: `${watchedPercentage}%` }}
                />
              </div>
            </div>
          )}

          {/* Complete Button */}
          <div className="flex items-center justify-between">
            {isCompleted ? (
              <div className="flex items-center gap-2 text-[#10B981]">
                <CheckCircle2 className="w-5 h-5" />
                <span className="font-medium">Tamamlandı</span>
              </div>
            ) : (
              <Button
                onClick={handleManualComplete}
                disabled={isCompleting}
                className="bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] hover:from-[#7C3AED] hover:to-[#14B8A6] text-white"
              >
                {isCompleting ? (
                  <>
                    <Loader2 className="w-4 h-4 animate-spin mr-2" />
                    Tamamlanıyor...
                  </>
                ) : (
                  'Tamamlandı Olarak İşaretle'
                )}
              </Button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

