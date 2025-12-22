/**
 * Progress Bar Component
 * İlerleme çubuğu bileşeni - görseldeki tasarıma uygun
 */

import { motion } from 'motion/react';

interface ProgressBarProps {
  percentage: number; // 0-100
  label?: string;
  showPercentage?: boolean;
  className?: string;
}

export function ProgressBar({
  percentage,
  label,
  showPercentage = true,
  className = '',
}: ProgressBarProps) {
  const clampedPercentage = Math.min(100, Math.max(0, percentage));

  return (
    <div className={`w-full ${className}`}>
      {label && (
        <div className="flex items-center justify-between mb-2">
          <span className="text-sm font-medium text-[#E5E7EB]">{label}</span>
          {showPercentage && (
            <span className="text-sm text-[#9CA3AF]">{Math.round(clampedPercentage)}% Tamamlandı</span>
          )}
        </div>
      )}
      <div className="w-full h-2 bg-[#30363D] rounded-full overflow-hidden">
        <motion.div
          className="h-full bg-gradient-to-r from-[#8B5CF6] to-[#2DD4BF] rounded-full"
          initial={{ width: 0 }}
          animate={{ width: `${clampedPercentage}%` }}
          transition={{ duration: 0.5, ease: 'easeOut' }}
        />
      </div>
    </div>
  );
}

