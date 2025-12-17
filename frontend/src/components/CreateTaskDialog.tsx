/**
 * CreateTaskDialog Component
 * Task oluşturma dialog'u - Modern dark-glass design
 */
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from './ui/dialog';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { AlertTriangle, Loader2, FileText, Bug, Wrench, TestTube } from 'lucide-react';
import { motion, AnimatePresence } from 'motion/react';
import type { TaskType, CreateTaskRequest } from '../types';

export interface CreateTaskDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: CreateTaskRequest) => Promise<void>;
  useCaseTitle?: string;
}

interface CreateTaskForm {
  title: string;
  description: string;
  importantNotes: string;
  type: TaskType;
  dueDate: string;
}

const taskTypeIcons: Record<TaskType, React.ReactNode> = {
  Documentation: <FileText className="w-4 h-4" />,
  Feature: <Wrench className="w-4 h-4" />,
  Test: <TestTube className="w-4 h-4" />,
  Bug: <Bug className="w-4 h-4" />,
};

const taskTypeColors: Record<TaskType, string> = {
  Documentation: 'text-[#3B82F6]',
  Feature: 'text-[#10B981]',
  Test: 'text-[#F59E0B]',
  Bug: 'text-[#EF4444]',
};

export function CreateTaskDialog({
  open,
  onOpenChange,
  onSubmit,
  useCaseTitle,
}: CreateTaskDialogProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
    watch,
    setValue,
  } = useForm<CreateTaskForm>({
    defaultValues: {
      title: '',
      description: '',
      importantNotes: '',
      type: 'Feature',
      dueDate: '',
    },
  });

  const descriptionLength = watch('description')?.length || 0;
  const notesLength = watch('importantNotes')?.length || 0;
  const selectedType = watch('type');

  const handleFormSubmit = async (data: CreateTaskForm) => {
    setIsSubmitting(true);
    setError(null);

    try {
      await onSubmit({
        title: data.title,
        description: data.description || undefined,
        importantNotes: data.importantNotes || undefined,
        type: data.type,
        dueDate: data.dueDate || undefined,
      });
      
      // Başarılı - formu resetle ve dialog'u kapat
      reset();
      onOpenChange(false);
    } catch (err: any) {
      console.error('Create task error:', err);
      setError(err.message || 'Task oluşturulurken bir hata oluştu');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    if (isSubmitting) return;
    reset();
    setError(null);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={handleCancel}>
      <DialogContent className="bg-[#161B22]/95 backdrop-blur-md border border-[#30363D]/50 text-[#E5E7EB] shadow-2xl max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
            Yeni Task Oluştur
          </DialogTitle>
          {useCaseTitle && (
            <DialogDescription className="text-[#9CA3AF]">
              <span className="font-semibold text-[#8B5CF6]">{useCaseTitle}</span> use case'i için yeni bir task oluşturun
            </DialogDescription>
          )}
        </DialogHeader>

        <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6 mt-4">
          {/* Title */}
          <div>
            <Label htmlFor="title" className="text-[#E5E7EB] mb-2 block">
              Task Başlığı <span className="text-[#EF4444]">*</span>
            </Label>
            <Input
              id="title"
              {...register('title', {
                required: 'Başlık zorunludur',
                minLength: { value: 3, message: 'Başlık en az 3 karakter olmalıdır' },
                maxLength: { value: 200, message: 'Başlık en fazla 200 karakter olmalıdır' },
              })}
              placeholder="Task başlığı..."
              disabled={isSubmitting}
              className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:ring-2 focus:ring-[#8B5CF6] transition-all disabled:opacity-50"
            />
            {errors.title && (
              <p className="text-[#EF4444] text-sm mt-2 flex items-center gap-1">
                <AlertTriangle className="w-3 h-3" />
                {errors.title.message}
              </p>
            )}
          </div>

          {/* Type */}
          <div>
            <Label htmlFor="type" className="text-[#E5E7EB] mb-2 block">
              Task Tipi <span className="text-[#EF4444]">*</span>
            </Label>
            <Select
              value={selectedType}
              onValueChange={(value) => setValue('type', value as TaskType)}
              disabled={isSubmitting}
            >
              <SelectTrigger className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6]">
                <SelectValue>
                  <div className="flex items-center gap-2">
                    <span className={taskTypeColors[selectedType]}>
                      {taskTypeIcons[selectedType]}
                    </span>
                    <span>{selectedType}</span>
                  </div>
                </SelectValue>
              </SelectTrigger>
              <SelectContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                <SelectItem value="Feature" className="hover:bg-[#21262D] cursor-pointer">
                  <div className="flex items-center gap-2">
                    <span className="text-[#10B981]">{taskTypeIcons.Feature}</span>
                    <span>Feature</span>
                  </div>
                </SelectItem>
                <SelectItem value="Bug" className="hover:bg-[#21262D] cursor-pointer">
                  <div className="flex items-center gap-2">
                    <span className="text-[#EF4444]">{taskTypeIcons.Bug}</span>
                    <span>Bug</span>
                  </div>
                </SelectItem>
                <SelectItem value="Test" className="hover:bg-[#21262D] cursor-pointer">
                  <div className="flex items-center gap-2">
                    <span className="text-[#F59E0B]">{taskTypeIcons.Test}</span>
                    <span>Test</span>
                  </div>
                </SelectItem>
                <SelectItem value="Documentation" className="hover:bg-[#21262D] cursor-pointer">
                  <div className="flex items-center gap-2">
                    <span className="text-[#3B82F6]">{taskTypeIcons.Documentation}</span>
                    <span>Documentation</span>
                  </div>
                </SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Due Date */}
          <div>
            <Label htmlFor="dueDate" className="text-[#E5E7EB] mb-2 block">
              Bitiş Tarihi <span className="text-[#6B7280]">(Opsiyonel)</span>
            </Label>
            <Input
              id="dueDate"
              type="date"
              {...register('dueDate')}
              disabled={isSubmitting}
              className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] transition-all disabled:opacity-50"
            />
          </div>

          {/* Description */}
          <div>
            <div className="flex items-center justify-between mb-2">
              <Label htmlFor="description" className="text-[#E5E7EB]">
                Açıklama <span className="text-[#6B7280]">(Opsiyonel)</span>
              </Label>
              <span className="text-xs text-[#6B7280]">{descriptionLength}/2000</span>
            </div>
            <Textarea
              id="description"
              {...register('description', {
                maxLength: { value: 2000, message: 'Açıklama en fazla 2000 karakter olmalıdır' },
              })}
              placeholder="Task'ın detaylı açıklaması..."
              disabled={isSubmitting}
              className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] min-h-[120px] focus:ring-2 focus:ring-[#8B5CF6] transition-all resize-none disabled:opacity-50"
            />
            {errors.description && (
              <p className="text-[#EF4444] text-sm mt-2 flex items-center gap-1">
                <AlertTriangle className="w-3 h-3" />
                {errors.description.message}
              </p>
            )}
          </div>

          {/* Important Notes */}
          <div>
            <div className="flex items-center justify-between mb-2">
              <Label htmlFor="importantNotes" className="text-[#E5E7EB] flex items-center gap-2">
                <AlertTriangle className="w-4 h-4 text-[#F59E0B]" />
                Önemli Notlar <span className="text-[#6B7280]">(Opsiyonel)</span>
              </Label>
              <span className="text-xs text-[#6B7280]">{notesLength}/1000</span>
            </div>
            <Textarea
              id="importantNotes"
              {...register('importantNotes', {
                maxLength: { value: 1000, message: 'Önemli notlar en fazla 1000 karakter olmalıdır' },
              })}
              placeholder="⚠️ Dikkat edilmesi gereken önemli notlar..."
              disabled={isSubmitting}
              className="bg-[#0D1117] border-[#F59E0B]/30 text-[#E5E7EB] placeholder:text-[#6B7280] min-h-[100px] focus:ring-2 focus:ring-[#F59E0B] transition-all resize-none disabled:opacity-50"
            />
            {errors.importantNotes && (
              <p className="text-[#EF4444] text-sm mt-2 flex items-center gap-1">
                <AlertTriangle className="w-3 h-3" />
                {errors.importantNotes.message}
              </p>
            )}
          </div>

          {/* Error Message */}
          <AnimatePresence>
            {error && (
              <motion.div
                initial={{ opacity: 0, height: 0 }}
                animate={{ opacity: 1, height: 'auto' }}
                exit={{ opacity: 0, height: 0 }}
                className="bg-[#EF4444]/10 border border-[#EF4444]/30 rounded-lg p-3"
              >
                <p className="text-[#EF4444] text-sm flex items-start gap-2">
                  <AlertTriangle className="w-4 h-4 flex-shrink-0 mt-0.5" />
                  <span>{error}</span>
                </p>
              </motion.div>
            )}
          </AnimatePresence>

          {/* Action Buttons */}
          <DialogFooter className="gap-3">
            <Button
              type="button"
              variant="outline"
              onClick={handleCancel}
              disabled={isSubmitting}
              className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] transition-all disabled:opacity-50"
            >
              İptal
            </Button>
            <Button
              type="submit"
              disabled={isSubmitting}
              className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/40 transition-all disabled:opacity-50 disabled:cursor-not-allowed relative overflow-hidden group"
            >
              {isSubmitting ? (
                <>
                  <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                  Oluşturuluyor...
                </>
              ) : (
                <>
                  Oluştur
                </>
              )}
              <motion.div
                className="absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent"
                initial={{ x: '-100%' }}
                whileHover={{ x: '100%' }}
                transition={{ duration: 0.5 }}
              />
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
