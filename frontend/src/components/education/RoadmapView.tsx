/**
 * Roadmap View Component
 * Roadmap görüntüleme bileşeni
 */

import { StepCard } from './StepCard';
import type { CourseRoadmap, StepProgress } from '../../types/education';

interface RoadmapViewProps {
  roadmap: CourseRoadmap;
  stepProgresses?: StepProgress[];
  onStepClick?: (stepId: string) => void;
  courseId?: string; // İçeriklere tıklama için gerekli
  onContentClick?: (stepId: string, contentId: string) => void; // İçerik tıklama handler
}

export function RoadmapView({
  roadmap,
  stepProgresses = [],
  onStepClick,
  courseId,
  onContentClick,
}: RoadmapViewProps) {
  // Step progress map oluştur
  const progressMap = new Map(
    stepProgresses.map(sp => [sp.stepId, sp])
  );

  // Adımın kilitli olup olmadığını kontrol et
  const isStepLocked = (stepOrder: number): boolean => {
    // İlk adım her zaman açık
    if (stepOrder === 1) return false;

    // Önceki tüm zorunlu adımlar tamamlanmış olmalı
    for (let i = 1; i < stepOrder; i++) {
      const prevStep = roadmap.steps.find(s => s.order === i);
      if (prevStep?.isRequired) {
        const prevProgress = progressMap.get(prevStep.id);
        if (!prevProgress?.isCompleted) {
          return true;
        }
      }
    }

    return false;
  };

  return (
    <div className="space-y-4">
      {roadmap.steps.map((step) => {
        const progress = progressMap.get(step.id);
        const isCompleted = progress?.isCompleted || false;
        const isLocked = isStepLocked(step.order);
        
        // Adım ilerleme yüzdesi hesapla
        const completedContents = progress?.contents.filter(c => c.isCompleted).length || 0;
        const totalContents = step.contents.length;
        const stepProgressPercentage = totalContents > 0 
          ? (completedContents / totalContents) * 100 
          : 0;

        return (
          <StepCard
            key={step.id}
            step={step}
            isCompleted={isCompleted}
            isLocked={isLocked}
            progressPercentage={stepProgressPercentage}
            onClick={() => onStepClick?.(step.id)}
            courseId={courseId}
            onContentClick={onContentClick}
          />
        );
      })}
    </div>
  );
}

