/**
 * Task Helpers
 * Task tipi ve durumu için icon, color ve label utilities
 */
import { FileText, Bug, Wrench, TestTube, Ban, Link2, WrenchIcon, Copy } from 'lucide-react';
import type { TaskType, TaskState, TaskRelationType } from '../types';

export const taskTypeConfig: Record<TaskType, { icon: React.ReactNode; color: string; label: string }> = {
  Documentation: {
    icon: <FileText className="w-4 h-4" />,
    color: 'text-[#3B82F6] bg-[#3B82F6]/10',
    label: 'Documentation',
  },
  Feature: {
    icon: <Wrench className="w-4 h-4" />,
    color: 'text-[#10B981] bg-[#10B981]/10',
    label: 'Feature',
  },
  Test: {
    icon: <TestTube className="w-4 h-4" />,
    color: 'text-[#F59E0B] bg-[#F59E0B]/10',
    label: 'Test',
  },
  Bug: {
    icon: <Bug className="w-4 h-4" />,
    color: 'text-[#EF4444] bg-[#EF4444]/10',
    label: 'Bug',
  },
};

export const taskStateConfig: Record<TaskState, { color: string; label: string }> = {
  NotStarted: {
    color: 'bg-[#6B7280]/10 text-[#6B7280]',
    label: 'Başlanmadı',
  },
  InProgress: {
    color: 'bg-[#3B82F6]/10 text-[#3B82F6]',
    label: 'Devam Ediyor',
  },
  Completed: {
    color: 'bg-[#10B981]/10 text-[#10B981]',
    label: 'Tamamlandı',
  },
  Cancelled: {
    color: 'bg-[#EF4444]/10 text-[#EF4444]',
    label: 'İptal Edildi',
  },
};

export const taskRelationConfig: Record<TaskRelationType, { icon: React.ReactNode; color: string; label: string; description: string }> = {
  Blocks: {
    icon: <Ban className="w-4 h-4" />,
    color: 'border-[#EF4444]/40 bg-[#EF4444]/10 text-[#EF4444]',
    label: 'Blokluyor',
    description: 'Bu task tamamlanmadan hedef task başlatılamaz',
  },
  RelatesTo: {
    icon: <Link2 className="w-4 h-4" />,
    color: 'border-[#3B82F6]/40 bg-[#3B82F6]/10 text-[#3B82F6]',
    label: 'İlişkili',
    description: 'Bu task ile ilişkili',
  },
  Fixes: {
    icon: <WrenchIcon className="w-4 h-4" />,
    color: 'border-[#10B981]/40 bg-[#10B981]/10 text-[#10B981]',
    label: 'Düzeltir',
    description: 'Bu task hedef task\'taki problemi düzeltir',
  },
  Duplicates: {
    icon: <Copy className="w-4 h-4" />,
    color: 'border-[#F59E0B]/40 bg-[#F59E0B]/10 text-[#F59E0B]',
    label: 'Tekrar',
    description: 'Bu task hedef task\'ın tekrarı',
  },
};

export const taskRelationTypeConfig: Record<TaskRelationType, { icon: React.ReactNode; color: string; label: string; description: string }> = {
  Blocks: {
    icon: <Ban className="w-4 h-4" />,
    color: 'text-[#EF4444] bg-[#EF4444]/10 border-[#EF4444]/30',
    label: 'Blokluyor',
    description: 'Bu task, hedef task\'ı blokluyor',
  },
  RelatesTo: {
    icon: <Link2 className="w-4 h-4" />,
    color: 'text-[#3B82F6] bg-[#3B82F6]/10 border-[#3B82F6]/30',
    label: 'İlişkili',
    description: 'Bu task, hedef task ile ilişkili',
  },
  Fixes: {
    icon: <WrenchIcon className="w-4 h-4" />,
    color: 'text-[#10B981] bg-[#10B981]/10 border-[#10B981]/30',
    label: 'Düzeltiyor',
    description: 'Bu task, hedef task\'ı düzeltiyor',
  },
  Duplicates: {
    icon: <Copy className="w-4 h-4" />,
    color: 'text-[#F59E0B] bg-[#F59E0B]/10 border-[#F59E0B]/30',
    label: 'Tekrar Ediyor',
    description: 'Bu task, hedef task\'ın kopyası',
  },
};
