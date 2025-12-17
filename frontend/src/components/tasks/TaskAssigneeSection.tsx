/**
 * Task Assignee Section Component
 * Task atama işlemleri için
 */
import { useState } from 'react';
import { Button } from '../ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { User, UserPlus, Loader2 } from 'lucide-react';
import type { Task } from '../../types';
import { motion } from 'motion/react';

interface TaskAssigneeSectionProps {
  task: Task;
  onAssign: (taskId: string, assigneeId: string) => Promise<void>;
  canAssign: boolean;
}

// TODO: Backend'den gerçek kullanıcı listesi alınacak
const MOCK_USERS = [
  { id: '1', name: 'Ahmet Yılmaz' },
  { id: '2', name: 'Ayşe Demir' },
  { id: '3', name: 'Mehmet Kaya' },
];

export function TaskAssigneeSection({ task, onAssign, canAssign }: TaskAssigneeSectionProps) {
  const [loading, setLoading] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState<string>(task.assigneeId || '');

  const handleAssign = async () => {
    if (!selectedUserId || selectedUserId === task.assigneeId) return;

    try {
      setLoading(true);
      await onAssign(task.id, selectedUserId);
    } finally {
      setLoading(false);
    }
  };

  const hasChanged = selectedUserId !== task.assigneeId;

  return (
    <motion.div
      initial={{ opacity: 0, x: 20 }}
      animate={{ opacity: 1, x: 0 }}
      transition={{ duration: 0.4, delay: 0.3 }}
      className="bg-[#161B22]/50 backdrop-blur-md border border-[#2DD4BF]/20 rounded-xl p-6 space-y-4"
    >
      <h2 className="text-lg font-semibold text-[#E5E7EB] mb-4 flex items-center gap-2">
        <UserPlus className="w-5 h-5 text-[#2DD4BF]" />
        Atama Yönetimi
      </h2>

      {/* Current Assignee */}
      <div className="space-y-2">
        <div className="flex items-center gap-2 text-sm text-[#6B7280]">
          <User className="w-4 h-4" />
          <span>Mevcut Atanan</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-10 h-10 rounded-full bg-gradient-to-br from-[#2DD4BF] to-[#14B8A6] flex items-center justify-center text-white text-sm font-medium shadow-lg shadow-[#2DD4BF]/20">
            {task.assigneeName?.[0]?.toUpperCase() || 'U'}
          </div>
          <div>
            <p className="text-[#E5E7EB] font-medium">{task.assigneeName || 'Atanmamış'}</p>
            {task.assigneeId && (
              <p className="text-[#6B7280] text-xs">ID: {task.assigneeId.substring(0, 8)}...</p>
            )}
          </div>
        </div>
      </div>

      {/* Assign Dropdown */}
      {canAssign && (
        <div className="space-y-3 pt-2 border-t border-[#30363D]/30">
          <label className="text-sm text-[#6B7280]">Yeni Atama</label>
          <Select value={selectedUserId} onValueChange={setSelectedUserId}>
            <SelectTrigger className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#2DD4BF]">
              <SelectValue placeholder="Kullanıcı seçin..." />
            </SelectTrigger>
            <SelectContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
              {MOCK_USERS.map((user) => (
                <SelectItem 
                  key={user.id} 
                  value={user.id}
                  className="hover:bg-[#21262D] cursor-pointer"
                >
                  <div className="flex items-center gap-2">
                    <div className="w-6 h-6 rounded-full bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] flex items-center justify-center text-white text-xs font-medium">
                      {user.name[0]}
                    </div>
                    <span>{user.name}</span>
                  </div>
                </SelectItem>
              ))}
            </SelectContent>
          </Select>

          <Button
            onClick={handleAssign}
            disabled={loading || !hasChanged}
            className="w-full bg-gradient-to-r from-[#2DD4BF] to-[#14B8A6] hover:from-[#14B8A6] hover:to-[#0D9488] text-white shadow-lg shadow-[#2DD4BF]/40 hover:shadow-[#2DD4BF]/60 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Atanıyor...
              </>
            ) : (
              <>
                <UserPlus className="w-4 h-4 mr-2" />
                Ata
              </>
            )}
          </Button>
        </div>
      )}
    </motion.div>
  );
}
