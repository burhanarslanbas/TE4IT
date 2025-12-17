/**
 * Task State Actions Component
 * Task durumunu değiştirmek için action butonları
 */
import { useState } from 'react';
import { Button } from '../ui/button';
import { Play, CheckCircle, XCircle, RotateCcw, Loader2 } from 'lucide-react';
import type { Task, TaskState } from '../../types';
import { motion } from 'motion/react';

interface TaskStateActionsProps {
  task: Task;
  onStateChange: (taskId: string, state: TaskState) => Promise<void>;
}

export function TaskStateActions({ task, onStateChange }: TaskStateActionsProps) {
  const [loading, setLoading] = useState(false);

  const handleStateChange = async (newState: TaskState) => {
    try {
      setLoading(true);
      await onStateChange(task.id, newState);
    } finally {
      setLoading(false);
    }
  };

  const canStart = task.state === 'NotStarted';
  const canComplete = task.state === 'InProgress';
  const canCancel = task.state === 'NotStarted' || task.state === 'InProgress';
  const canRevert = task.state === 'InProgress' || task.state === 'Cancelled';

  return (
    <div className="space-y-3">
      {/* Start Button */}
      {canStart && (
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.3 }}
        >
          <Button
            onClick={() => handleStateChange('InProgress')}
            disabled={loading}
            className="w-full bg-gradient-to-r from-[#3B82F6] to-[#2563EB] hover:from-[#2563EB] hover:to-[#1D4ED8] text-white shadow-lg shadow-[#3B82F6]/40 hover:shadow-[#3B82F6]/60 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                İşleniyor...
              </>
            ) : (
              <>
                <Play className="w-4 h-4 mr-2" />
                Başlat
              </>
            )}
          </Button>
        </motion.div>
      )}

      {/* Complete Button */}
      {canComplete && (
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.3, delay: 0.1 }}
        >
          <Button
            onClick={() => handleStateChange('Completed')}
            disabled={loading}
            className="w-full bg-gradient-to-r from-[#10B981] to-[#059669] hover:from-[#059669] hover:to-[#047857] text-white shadow-lg shadow-[#10B981]/40 hover:shadow-[#10B981]/60 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                İşleniyor...
              </>
            ) : (
              <>
                <CheckCircle className="w-4 h-4 mr-2" />
                Tamamla
              </>
            )}
          </Button>
        </motion.div>
      )}

      {/* Cancel Button */}
      {canCancel && (
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.3, delay: 0.2 }}
        >
          <Button
            onClick={() => handleStateChange('Cancelled')}
            disabled={loading}
            variant="outline"
            className="w-full border-[#EF4444]/30 text-[#EF4444] hover:bg-[#EF4444]/10 hover:border-[#EF4444]/50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                İşleniyor...
              </>
            ) : (
              <>
                <XCircle className="w-4 h-4 mr-2" />
                İptal Et
              </>
            )}
          </Button>
        </motion.div>
      )}

      {/* Revert Button */}
      {canRevert && (
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.3, delay: 0.3 }}
        >
          <Button
            onClick={() => handleStateChange('NotStarted')}
            disabled={loading}
            variant="outline"
            className="w-full border-[#6B7280]/30 text-[#6B7280] hover:bg-[#6B7280]/10 hover:border-[#6B7280]/50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                İşleniyor...
              </>
            ) : (
              <>
                <RotateCcw className="w-4 h-4 mr-2" />
                Geri Al
              </>
            )}
          </Button>
        </motion.div>
      )}

      {/* No Actions Available */}
      {!canStart && !canComplete && !canCancel && !canRevert && (
        <div className="text-center text-[#6B7280] text-sm py-4">
          Bu durumda kullanılabilir aksiyon yok.
        </div>
      )}
    </div>
  );
}

