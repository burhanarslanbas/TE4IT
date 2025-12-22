/**
 * Step Card Component
 * Roadmap adım kartı bileşeni
 */

import { motion } from 'motion/react';
import { CheckCircle2, Lock, Clock, FileText, Video, ExternalLink, Play, ArrowRight } from 'lucide-react';
import { Card, CardContent } from '../ui/card';
import { Badge } from '../ui/badge';
import { ProgressBar } from './ProgressBar';
import type { RoadmapStep } from '../../types/education';
import { ContentType } from '../../types/education';

interface StepCardProps {
  step: RoadmapStep;
  isCompleted: boolean;
  isLocked: boolean;
  progressPercentage?: number;
  onClick?: () => void;
  courseId?: string; // İçeriklere tıklama için gerekli
  onContentClick?: (stepId: string, contentId: string) => void; // İçerik tıklama handler
}

export function StepCard({
  step,
  isCompleted,
  isLocked,
  progressPercentage = 0,
  onClick,
  courseId,
  onContentClick,
}: StepCardProps) {
  const getStatusIcon = () => {
    if (isCompleted) {
      return <CheckCircle2 className="w-6 h-6 text-[#10B981]" />;
    }
    if (isLocked) {
      return <Lock className="w-6 h-6 text-[#6B7280]" />;
    }
    return <Clock className="w-6 h-6 text-[#2DD4BF]" />;
  };

  const getStatusColor = () => {
    if (isCompleted) {
      return 'bg-[#10B981]/10 border-[#10B981]/50 shadow-lg shadow-[#10B981]/10';
    }
    if (isLocked) {
      return 'bg-[#161B22]/30 border-[#30363D] opacity-60';
    }
    return 'bg-[#161B22]/50 border-[#30363D] hover:border-[#8B5CF6]/50 hover:shadow-lg hover:shadow-[#8B5CF6]/10';
  };

  const getStatusBadge = () => {
    if (isCompleted) {
      return (
        <Badge className="bg-[#10B981]/20 text-[#10B981] border-[#10B981]/50">
          <CheckCircle2 className="w-3 h-3 mr-1" />
          Tamamlandı
        </Badge>
      );
    }
    if (isLocked) {
      return (
        <Badge className="bg-[#6B7280]/20 text-[#6B7280] border-[#6B7280]/50">
          <Lock className="w-3 h-3 mr-1" />
          Kilitli
        </Badge>
      );
    }
    if (progressPercentage > 0) {
      return (
        <Badge className="bg-[#2DD4BF]/20 text-[#2DD4BF] border-[#2DD4BF]/50">
          <Clock className="w-3 h-3 mr-1" />
          Devam Ediyor
        </Badge>
      );
    }
    return (
      <Badge className="bg-[#8B5CF6]/20 text-[#8B5CF6] border-[#8B5CF6]/50">
        <Play className="w-3 h-3 mr-1" />
        Başla
      </Badge>
    );
  };

  const getContentTypeIcon = (type: ContentType) => {
    switch (type) {
      case ContentType.VideoLink:
        return <Video className="w-4 h-4" />;
      case ContentType.DocumentLink:
        return <FileText className="w-4 h-4" />;
      case ContentType.ExternalLink:
        return <ExternalLink className="w-4 h-4" />;
      default:
        return <FileText className="w-4 h-4" />;
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      whileHover={!isLocked ? { scale: 1.01, y: -2 } : {}}
      whileTap={!isLocked ? { scale: 0.99 } : {}}
      transition={{ duration: 0.2 }}
    >
      <Card
        className={`transition-all duration-300 ${getStatusColor()} ${!isLocked && onClick ? 'cursor-pointer' : 'cursor-not-allowed'}`}
        onClick={(e) => {
          // #region agent log
          fetch('http://127.0.0.1:7242/ingest/c56ce959-0007-47da-a7df-a89074f8ff7a',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({location:'StepCard.tsx:107',message:'StepCard onClick',data:{stepId:step.id,isLocked,hasOnClick:!!onClick},timestamp:Date.now(),sessionId:'debug-session',runId:'run1',hypothesisId:'F'})}).catch(()=>{});
          // #endregion
          
          if (!isLocked && onClick) {
            onClick();
          }
        }}
      >
        <CardContent className="p-6">
          <div className="flex items-start gap-4">
            {/* Step Number & Status Icon */}
            <div className="flex-shrink-0">
              <div className={`w-14 h-14 rounded-xl flex items-center justify-center ${
                isCompleted 
                  ? 'bg-gradient-to-br from-[#10B981] to-[#059669] shadow-lg shadow-[#10B981]/30' 
                  : isLocked
                  ? 'bg-[#161B22] border-2 border-[#30363D]'
                  : 'bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] shadow-lg shadow-[#8B5CF6]/30'
              }`}>
                {isCompleted ? (
                  <CheckCircle2 className="w-7 h-7 text-white" />
                ) : isLocked ? (
                  <Lock className="w-6 h-6 text-[#6B7280]" />
                ) : (
                  <span className="text-white font-bold text-lg">{step.order}</span>
                )}
              </div>
            </div>

            {/* Content */}
            <div className="flex-1 min-w-0">
              <div className="flex items-start justify-between gap-3 mb-2">
                <div className="flex-1">
                  <h3 className={`text-xl font-bold mb-2 ${
                    isLocked ? 'text-[#6B7280]' : 'text-[#E5E7EB]'
                  }`}>
                    {step.title}
                  </h3>
                  {getStatusBadge()}
                </div>
              </div>

              {step.description && (
                <p className={`text-sm mb-4 ${isLocked ? 'text-[#6B7280]' : 'text-[#9CA3AF]'}`}>
                  {step.description}
                </p>
              )}

              {/* Contents List */}
              <div className="space-y-2 mb-4">
                {step.contents.slice(0, 3).map((content) => (
                  <div
                    key={content.id}
                    onClick={(e) => {
                      e.stopPropagation(); // Step click'i engelle
                      if (!isLocked && onContentClick && courseId) {
                        onContentClick(step.id, content.id);
                      }
                    }}
                    className={`flex items-center gap-2 text-sm ${
                      isLocked 
                        ? 'text-[#6B7280] cursor-not-allowed' 
                        : onContentClick && courseId
                        ? 'text-[#9CA3AF] hover:text-[#E5E7EB] cursor-pointer hover:bg-[#161B22]/50 rounded px-2 py-1 transition-colors'
                        : 'text-[#9CA3AF]'
                    }`}
                  >
                    {getContentTypeIcon(content.type)}
                    <span className="truncate">{content.title}</span>
                    {content.isRequired && (
                      <Badge className="ml-auto bg-[#8B5CF6]/20 text-[#8B5CF6] border-[#8B5CF6]/50 text-xs">
                        Zorunlu
                      </Badge>
                    )}
                  </div>
                ))}
                {step.contents.length > 3 && (
                  <p className="text-xs text-[#6B7280]">
                    +{step.contents.length - 3} içerik daha
                  </p>
                )}
              </div>

              {/* Progress Bar */}
              {progressPercentage > 0 && !isCompleted && (
                <div className="mb-4">
                  <ProgressBar
                    percentage={progressPercentage}
                    label={`${Math.round(progressPercentage)}% tamamlandı`}
                    showPercentage={false}
                  />
                </div>
              )}

              {/* Footer Info */}
              <div className="flex items-center justify-between pt-3 border-t border-[#30363D]">
                <div className="flex items-center gap-4 text-sm">
                  <span className={`${isLocked ? 'text-[#6B7280]' : 'text-[#9CA3AF]'}`}>
                    {step.contents.length} İçerik
                  </span>
                  <span className={`${isLocked ? 'text-[#6B7280]' : 'text-[#9CA3AF]'}`}>
                    {step.estimatedDurationMinutes} dk
                  </span>
                </div>
                {!isLocked && onClick && (
                  <motion.div
                    whileHover={{ x: 5 }}
                    className="flex items-center text-[#8B5CF6] text-sm font-medium"
                  >
                    {isCompleted ? 'Görüntüle' : 'Başla'}
                    <ArrowRight className="w-4 h-4 ml-1" />
                  </motion.div>
                )}
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </motion.div>
  );
}

