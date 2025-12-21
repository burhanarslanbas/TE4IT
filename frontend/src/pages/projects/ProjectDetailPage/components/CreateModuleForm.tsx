/**
 * Create Module Form Component
 */
import { useForm } from 'react-hook-form';
import { Button } from '../../../../components/ui/button';
import { Input } from '../../../../components/ui/input';
import { Label } from '../../../../components/ui/label';
import { Textarea } from '../../../../components/ui/textarea';
import { validateSafeString } from '../../../../utils/security';
import { Plus } from 'lucide-react';

interface CreateModuleFormData {
  title: string;
  description: string;
}

interface CreateModuleFormProps {
  onSubmit: (data: CreateModuleFormData) => Promise<void>;
  onCancel: () => void;
}

export function CreateModuleForm({ onSubmit, onCancel }: CreateModuleFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateModuleFormData>();

  const handleFormSubmit = async (data: CreateModuleFormData) => {
    // Input sanitization
    const titleValidation = validateSafeString(data.title, 100);
    const descriptionValidation = data.description
      ? validateSafeString(data.description, 1000)
      : { isValid: true, sanitized: '', errors: [] };

    if (!titleValidation.isValid || !descriptionValidation.isValid) {
      return;
    }

    await onSubmit({
      title: titleValidation.sanitized,
      description: descriptionValidation.sanitized,
    });
    reset();
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6 py-2">
      <div className="space-y-2">
        <Label htmlFor="module-title" className="text-[#E5E7EB] font-semibold flex items-center gap-2">
          <span className="w-1.5 h-1.5 bg-[#8B5CF6] rounded-full" />
          Modül Başlığı *
        </Label>
        <Input
          id="module-title"
          {...register('title', {
            required: 'Modül başlığı zorunludur',
            minLength: { value: 3, message: 'En az 3 karakter olmalıdır' },
            maxLength: { value: 100, message: 'En fazla 100 karakter olabilir' },
          })}
          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] h-12 focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 transition-all"
          placeholder="Örn: Kullanıcı Yönetimi, Ödeme Sistemi..."
        />
        {errors.title && (
          <p className="text-[#EF4444] text-sm mt-1.5 flex items-center gap-1.5">
            <span className="w-1 h-1 bg-[#EF4444] rounded-full" />
            {errors.title.message}
          </p>
        )}
      </div>
      
      <div className="space-y-2">
        <Label htmlFor="module-description" className="text-[#E5E7EB] font-semibold flex items-center gap-2">
          <span className="w-1.5 h-1.5 bg-[#2DD4BF] rounded-full" />
          Açıklama
          <span className="text-xs font-normal text-[#9CA3AF]">(Opsiyonel)</span>
        </Label>
        <Textarea
          id="module-description"
          {...register('description', {
            maxLength: { value: 1000, message: 'En fazla 1000 karakter olabilir' },
          })}
          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[120px] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 transition-all resize-none"
          placeholder="Modülün amacını ve kapsamını açıklayın..."
        />
        {errors.description && (
          <p className="text-[#EF4444] text-sm mt-1.5 flex items-center gap-1.5">
            <span className="w-1 h-1 bg-[#EF4444] rounded-full" />
            {errors.description.message}
          </p>
        )}
      </div>
      
      <div className="flex justify-end gap-3 pt-4 border-t border-[#30363D]">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#40464D] h-11 px-6 transition-all"
        >
          İptal
        </Button>
        <Button 
          type="submit" 
          className="bg-gradient-to-r from-[#8B5CF6] via-[#7C3AED] to-[#6D28D9] hover:from-[#7C3AED] hover:via-[#6D28D9] hover:to-[#5B21B6] text-white shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 h-11 px-8 font-semibold transition-all duration-300"
        >
          <Plus className="w-4 h-4 mr-2" />
          Modül Oluştur
        </Button>
      </div>
    </form>
  );
}

