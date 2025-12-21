/**
 * Add Task Relation Modal Component
 * Yeni task ilişkisi eklemek için modal
 */
import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter } from '../ui/dialog';
import { Button } from '../ui/button';
import { Label } from '../ui/label';
import { RadioGroup, RadioGroupItem } from '../ui/radio-group';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Loader2, Plus } from 'lucide-react';
import type { TaskRelationType, Task } from '../../types';
import { taskRelationConfig } from '../../utils/taskHelpers';
import { motion } from 'motion/react';

interface AddTaskRelationModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  currentTaskId: string;
  onSubmit: (targetTaskId: string, relationType: TaskRelationType) => Promise<void>;
  availableTasks?: Task[];
}

export function AddTaskRelationModal({ 
  open, 
  onOpenChange, 
  currentTaskId,
  onSubmit,
  availableTasks = []
}: AddTaskRelationModalProps) {
  const [targetTaskId, setTargetTaskId] = useState<string>('');
  const [relationType, setRelationType] = useState<TaskRelationType>('RelatesTo');
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async () => {
    if (!targetTaskId) {
      return;
    }

    try {
      setSubmitting(true);
      await onSubmit(targetTaskId, relationType);
      
      // Reset form
      setTargetTaskId('');
      setRelationType('RelatesTo');
      onOpenChange(false);
    } catch (error) {
      // Error toast already handled in hook
    } finally {
      setSubmitting(false);
    }
  };

  const relationTypes: TaskRelationType[] = ['Blocks', 'RelatesTo', 'Fixes', 'Duplicates'];

  // Mevcut task'ı available tasks'tan çıkar
  const filteredTasks = availableTasks.filter(t => t.id !== currentTaskId);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="text-xl font-bold bg-gradient-to-r from-[#E5E7EB] to-[#8B5CF6] bg-clip-text text-transparent">
            İlişki Ekle
          </DialogTitle>
          <DialogDescription className="text-[#9CA3AF]">
            Bu task ile başka bir task arasında ilişki oluşturun.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6 py-4">
          {/* Target Task Selection */}
          <div className="space-y-3">
            <Label htmlFor="targetTask" className="text-[#E5E7EB]">
              Hedef Task <span className="text-[#EF4444]">*</span>
            </Label>
            {filteredTasks.length === 0 ? (
              <div className="text-[#6B7280] text-sm bg-[#0D1117]/60 border border-[#30363D]/50 rounded-lg p-4 text-center">
                İlişkilendirilecek başka task bulunmuyor.
              </div>
            ) : (
              <Select value={targetTaskId} onValueChange={setTargetTaskId}>
                <SelectTrigger className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6]">
                  <SelectValue placeholder="Task seçin..." />
                </SelectTrigger>
                <SelectContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] max-h-[300px]">
                  {filteredTasks.map((task) => (
                    <SelectItem 
                      key={task.id} 
                      value={task.id}
                      className="hover:bg-[#21262D] cursor-pointer"
                    >
                      <div className="flex flex-col gap-1">
                        <span className="font-medium">{task.title}</span>
                        <span className="text-xs text-[#6B7280]">
                          {task.type} · {task.state}
                        </span>
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          </div>

          {/* Relation Type Selection */}
          <div className="space-y-3">
            <Label className="text-[#E5E7EB]">
              İlişki Tipi <span className="text-[#EF4444]">*</span>
            </Label>
            <RadioGroup value={relationType} onValueChange={(value) => setRelationType(value as TaskRelationType)}>
              <div className="space-y-2">
                {relationTypes.map((type) => {
                  const config = taskRelationConfig[type];
                  return (
              <motion.div
                      key={type}
                      whileHover={{ scale: 1.01, x: 2 }}
                      transition={{ duration: 0.2 }}
                      className={`flex items-start space-x-3 p-4 rounded-lg border ${
                        relationType === type
                          ? `${config.color} border`
                          : 'bg-[#0D1117]/40 border-[#30363D]/30 hover:border-[#30363D]/50'
                      } transition-all cursor-pointer`}
                      onClick={() => setRelationType(type)}
              >
                      <RadioGroupItem value={type} id={type} className="mt-1" />
                      <div className="flex-1">
                        <label htmlFor={type} className="flex items-center gap-2 cursor-pointer">
                          <div className={relationType === type ? '' : 'opacity-60'}>
                            {config.icon}
                  </div>
                          <span className={`font-medium ${relationType === type ? '' : 'text-[#9CA3AF]'}`}>
                            {config.label}
                          </span>
                        </label>
                        <p className={`text-xs mt-1 ${relationType === type ? 'opacity-90' : 'text-[#6B7280]'}`}>
                          {config.description}
                        </p>
                  </div>
              </motion.div>
                  );
                })}
                  </div>
            </RadioGroup>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={submitting}
            className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
          >
            İptal
          </Button>
          <Button
            onClick={handleSubmit}
            disabled={!targetTaskId || submitting || filteredTasks.length === 0}
            className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/40 hover:shadow-[#8B5CF6]/60 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {submitting ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Oluşturuluyor...
              </>
            ) : (
              <>
                <Plus className="w-4 h-4 mr-2" />
                İlişki Ekle
              </>
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
