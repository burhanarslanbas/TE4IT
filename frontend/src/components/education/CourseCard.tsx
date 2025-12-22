/**
 * Course Card Component
 * Kurs kartı bileşeni - görseldeki tasarıma uygun
 */

import { motion } from 'motion/react';
import { useNavigate } from 'react-router-dom';
import { CheckCircle2, Video, FileText, ClipboardCheck, Lock, Clock } from 'lucide-react';
import { Card, CardContent } from '../ui/card';
import { Badge } from '../ui/badge';
import type { CourseListItem, CourseStatus } from '../../types/education';
import { ContentType } from '../../types/education';

interface CourseCardProps {
  course: CourseListItem;
  status?: CourseStatus;
  progressPercentage?: number;
  enrollment?: {
    enrolledAt: string;
    startedAt?: string;
    completedAt?: string;
  };
  onClick?: () => void;
}

export function CourseCard({
  course,
  status = 'not-enrolled',
  progressPercentage = 0,
  enrollment,
  onClick,
}: CourseCardProps) {
  const navigate = useNavigate();

  const handleClick = () => {
    if (onClick) {
      onClick();
    } else {
      navigate(`/education/courses/${course.id}`);
    }
  };

  // Durum ikonunu belirle
  const getStatusIcon = () => {
    switch (status) {
      case 'completed':
        return <CheckCircle2 className="w-6 h-6 text-[#10B981]" />;
      case 'in-progress':
        return <Video className="w-6 h-6 text-[#2DD4BF]" />;
      case 'enrolled':
        return <ClipboardCheck className="w-6 h-6 text-[#6B7280]" />;
      default:
        return <Lock className="w-6 h-6 text-[#6B7280]" />;
    }
  };

  // Durum rengini belirle
  const getStatusColor = () => {
    switch (status) {
      case 'completed':
        return 'bg-[#10B981]/10 border-[#10B981]/30';
      case 'in-progress':
        return 'bg-[#2DD4BF]/10 border-[#2DD4BF]/30';
      default:
        return 'bg-[#161B22]/50 border-[#30363D]';
    }
  };

  // İçerik tipi badge'ini belirle (örnek - gerçekte roadmap'ten gelecek)
  const getContentTypeBadge = () => {
    // Bu bilgi backend'den gelecek, şimdilik varsayılan
    return (
      <Badge className="bg-[#8B5CF6]/20 text-[#C4B5FD] border-[#8B5CF6]/30">
        <Video className="w-3 h-3 mr-1" />
        Video
      </Badge>
    );
  };

  // Süreyi formatla
  const formatDuration = (minutes: number) => {
    if (minutes < 60) {
      return `${minutes} dk`;
    }
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return mins > 0 ? `${hours} saat ${mins} dk` : `${hours} saat`;
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      whileHover={{ scale: 1.02 }}
      whileTap={{ scale: 0.98 }}
    >
      <Card
        className={`cursor-pointer transition-all duration-300 hover:border-[#8B5CF6]/50 hover:shadow-lg hover:shadow-[#8B5CF6]/20 ${getStatusColor()}`}
        onClick={handleClick}
      >
        <CardContent className="p-6">
          <div className="flex items-start gap-4">
            {/* İkon */}
            <div className="flex-shrink-0">
              <div className="w-12 h-12 rounded-lg bg-[#161B22] border border-[#30363D] flex items-center justify-center">
                {getStatusIcon()}
              </div>
            </div>

            {/* İçerik */}
            <div className="flex-1 min-w-0">
              <div className="flex items-start justify-between gap-2 mb-2">
                <h3 className="text-lg font-semibold text-[#E5E7EB] line-clamp-2">
                  {course.title}
                </h3>
              </div>

              <div className="flex items-center gap-3 mb-3">
                <div className="flex items-center gap-1 text-sm text-[#9CA3AF]">
                  <Clock className="w-4 h-4" />
                  <span>{formatDuration(course.estimatedDurationMinutes)}</span>
                </div>
                {getContentTypeBadge()}
              </div>

              {/* İlerleme çubuğu (eğer kayıtlıysa) */}
              {status !== 'not-enrolled' && progressPercentage > 0 && (
                <div className="mt-3">
                  <div className="w-full h-1.5 bg-[#30363D] rounded-full overflow-hidden">
                    <motion.div
                      className="h-full bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] rounded-full"
                      initial={{ width: 0 }}
                      animate={{ width: `${progressPercentage}%` }}
                      transition={{ duration: 0.5 }}
                    />
                  </div>
                  <p className="text-xs text-[#9CA3AF] mt-1">
                    %{Math.round(progressPercentage)} tamamlandı
                  </p>
                </div>
              )}
            </div>
          </div>
        </CardContent>
      </Card>
    </motion.div>
  );
}

