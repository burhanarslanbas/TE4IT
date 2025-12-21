/**
 * ConfirmDeleteDialog Component
 * Reusable confirmation dialog for delete operations
 * Modern dark-glass design with name confirmation
 */
import { useState } from 'react';
import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from './ui/alert-dialog';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { AlertTriangle, Loader2, Trash2 } from 'lucide-react';
import { motion, AnimatePresence } from 'motion/react';

export interface ConfirmDeleteDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  entityType: 'Project' | 'Module' | 'UseCase' | 'Task';
  entityName: string;
  onConfirm: () => Promise<void>;
  confirmText?: string; // İsim yazarak onay gerekiyorsa
  children?: { count: number; type: string }[]; // Alt öğeler (409 uyarısı için)
}

const entityTypeLabels: Record<string, { singular: string; plural: string; article: string }> = {
  Project: { singular: 'Proje', plural: 'projeler', article: 'bu projeyi' },
  Module: { singular: 'Modül', plural: 'modüller', article: 'bu modülü' },
  UseCase: { singular: 'Use Case', plural: 'use case\'ler', article: 'bu use case\'i' },
  Task: { singular: 'Task', plural: 'task\'lar', article: 'bu task\'ı' },
};

export function ConfirmDeleteDialog({
  open,
  onOpenChange,
  entityType,
  entityName,
  onConfirm,
  confirmText,
  children,
}: ConfirmDeleteDialogProps) {
  const [isDeleting, setIsDeleting] = useState(false);
  const [confirmInput, setConfirmInput] = useState('');
  const [error, setError] = useState<string | null>(null);

  const labels = entityTypeLabels[entityType] || entityTypeLabels.Project;
  const requireTextConfirmation = confirmText !== undefined;
  const isConfirmTextValid = !requireTextConfirmation || confirmInput === confirmText;

  const handleConfirm = async () => {
    if (!isConfirmTextValid) return;

    setIsDeleting(true);
    setError(null);

    try {
      await onConfirm();
      // Başarılı - dialog kapanacak
      onOpenChange(false);
      setConfirmInput('');
    } catch (err: any) {
      // Hata - dialog açık kalsın, hatayı göster
      console.error('Delete error:', err);
      setError(err.message || 'Silme işlemi başarısız oldu');
    } finally {
      setIsDeleting(false);
    }
  };

  const handleCancel = () => {
    if (isDeleting) return;
    setConfirmInput('');
    setError(null);
    onOpenChange(false);
  };

  return (
    <AlertDialog open={open} onOpenChange={handleCancel}>
      <AlertDialogContent className="bg-[#161B22]/95 backdrop-blur-md border border-[#30363D]/50 text-[#E5E7EB] shadow-2xl max-w-lg">
        <AlertDialogHeader>
          <div className="flex items-start gap-4">
            <div className="w-12 h-12 bg-gradient-to-br from-[#EF4444]/20 to-[#DC2626]/20 rounded-xl flex items-center justify-center flex-shrink-0">
              <AlertTriangle className="w-6 h-6 text-[#EF4444]" />
            </div>
            <div className="flex-1">
              <AlertDialogTitle className="text-xl font-bold text-[#E5E7EB] mb-2">
                {labels.singular} Sil
              </AlertDialogTitle>
              <AlertDialogDescription className="text-[#9CA3AF] text-base">
                <span className="font-semibold text-[#EF4444]">{entityName}</span> {labels.article} silmek
                üzeresiniz. Bu işlem{' '}
                <span className="font-semibold text-[#EF4444]">geri alınamaz</span>.
              </AlertDialogDescription>
            </div>
          </div>
        </AlertDialogHeader>

        {/* Children Warning */}
        {children && children.length > 0 && (
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            className="bg-[#F59E0B]/10 border border-[#F59E0B]/30 rounded-lg p-4 mt-4"
          >
            <div className="flex items-start gap-3">
              <AlertTriangle className="w-5 h-5 text-[#F59E0B] flex-shrink-0 mt-0.5" />
              <div>
                <p className="text-[#F59E0B] font-semibold text-sm mb-1">Uyarı</p>
                <p className="text-[#E5E7EB] text-sm">
                  Bu {labels.singular.toLowerCase()} içinde{' '}
                  {children.map((child, idx) => (
                    <span key={idx}>
                      {idx > 0 && ' ve '}
                      <span className="font-semibold text-[#F59E0B]">
                        {child.count} {child.type}
                      </span>
                    </span>
                  ))}{' '}
                  bulunuyor. Önce alt öğeleri silmeniz gerekebilir.
                </p>
              </div>
            </div>
          </motion.div>
        )}

        {/* Confirm Text Input */}
        {requireTextConfirmation && (
          <div className="mt-4 space-y-2">
            <Label htmlFor="confirmText" className="text-[#E5E7EB] text-sm">
              Onaylamak için <span className="font-mono text-[#EF4444]">{confirmText}</span> yazın
            </Label>
            <Input
              id="confirmText"
              value={confirmInput}
              onChange={(e) => setConfirmInput(e.target.value)}
              placeholder={confirmText}
              disabled={isDeleting}
              className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:ring-2 focus:ring-[#EF4444] transition-all disabled:opacity-50"
              autoComplete="off"
            />
            {confirmInput && !isConfirmTextValid && (
              <p className="text-[#EF4444] text-xs flex items-center gap-1">
                <AlertTriangle className="w-3 h-3" />
                Metin eşleşmiyor
              </p>
            )}
          </div>
        )}

        {/* Error Message */}
        <AnimatePresence>
          {error && (
            <motion.div
              initial={{ opacity: 0, height: 0 }}
              animate={{ opacity: 1, height: 'auto' }}
              exit={{ opacity: 0, height: 0 }}
              className="bg-[#EF4444]/10 border border-[#EF4444]/30 rounded-lg p-3 mt-4"
            >
              <p className="text-[#EF4444] text-sm flex items-start gap-2">
                <AlertTriangle className="w-4 h-4 flex-shrink-0 mt-0.5" />
                <span>{error}</span>
              </p>
            </motion.div>
          )}
        </AnimatePresence>

        <AlertDialogFooter className="mt-6 gap-3">
          <Button
            variant="outline"
            onClick={handleCancel}
            disabled={isDeleting}
            className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] transition-all disabled:opacity-50"
          >
            İptal
          </Button>
          <Button
            onClick={handleConfirm}
            disabled={!isConfirmTextValid || isDeleting}
            className="bg-gradient-to-r from-[#EF4444] to-[#DC2626] text-white hover:from-[#DC2626] hover:to-[#B91C1C] shadow-lg shadow-[#EF4444]/40 transition-all disabled:opacity-50 disabled:cursor-not-allowed relative overflow-hidden group"
          >
            {isDeleting ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Siliniyor...
              </>
            ) : (
              <>
                <Trash2 className="w-4 h-4 mr-2" />
                Sil
              </>
            )}
            <motion.div
              className="absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent"
              initial={{ x: '-100%' }}
              whileHover={{ x: '100%' }}
              transition={{ duration: 0.5 }}
            />
          </Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
