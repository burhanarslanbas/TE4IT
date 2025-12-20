/**
 * Manuel Create Project Tab
 * Kullanıcının kendi bilgilerini girerek proje oluşturması
 */

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { ProjectService } from '../../services/projectService';
import { ApiError } from '../../services/api';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { toast } from 'sonner';
import { Plus, FileText } from 'lucide-react';

interface CreateProjectForm {
  title: string;
  description: string;
}

interface ManuelCreateProjectTabProps {
  onProjectCreated: (projectId: string) => void;
  onCancel: () => void;
}

export function ManuelCreateProjectTab({ onProjectCreated, onCancel }: ManuelCreateProjectTabProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateProjectForm>({
    mode: 'onChange',
  });

  const handleCreateProject = async (data: CreateProjectForm) => {
    try {
      setIsSubmitting(true);

      const project = await ProjectService.createProject({
        title: data.title.trim(),
        description: data.description?.trim() || undefined,
      });

      toast.success('Proje oluşturuldu', {
        description: 'Yeni projeniz başarıyla oluşturuldu.',
      });

      onProjectCreated(project.id);
    } catch (error: any) {
      let errorMessage = 'Bir hata oluştu. Lütfen tekrar deneyin.';

      if (error instanceof ApiError) {
        errorMessage = error.message || 'Proje oluşturulamadı';
        if (error.errors && error.errors.length > 0) {
          errorMessage = error.errors.join(', ');
        }
      } else if (error?.message) {
        errorMessage = error.message;
      }

      toast.error('Proje oluşturulamadı', {
        description: errorMessage,
        duration: 5000,
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(handleCreateProject)} className="space-y-6">
      {/* Project Title */}
      <div>
        <Label htmlFor="title" className="text-[#E5E7EB] text-sm font-medium mb-2 block flex items-center gap-2">
          <FileText className="w-4 h-4" />
          Proje Başlığı *
        </Label>
        <Input
          id="title"
          {...register('title', {
            required: 'Proje başlığı zorunludur',
            minLength: { value: 3, message: 'En az 3 karakter olmalıdır' },
            maxLength: { value: 120, message: 'En fazla 120 karakter olabilir' },
            validate: (value) => {
              return value?.trim()?.length >= 3 || 'Proje başlığı en az 3 karakter olmalıdır';
            },
          })}
          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 h-12 text-base"
          placeholder="Harika bir proje adı girin..."
          disabled={isSubmitting}
        />
        {errors.title && (
          <p className="text-[#EF4444] text-xs mt-2 flex items-center gap-1">
            <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
            {errors.title.message}
          </p>
        )}
      </div>

      {/* Project Description */}
      <div>
        <Label htmlFor="description" className="text-[#E5E7EB] text-sm font-medium mb-2 block">
          Açıklama
        </Label>
        <Textarea
          id="description"
          {...register('description', {
            maxLength: { value: 1000, message: 'En fazla 1000 karakter olabilir' },
          })}
          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[160px] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 resize-none text-base"
          placeholder="Projeniz hakkında detaylı bir açıklama yazın... (Opsiyonel)"
          disabled={isSubmitting}
        />
        {errors.description && (
          <p className="text-[#EF4444] text-xs mt-2 flex items-center gap-1">
            <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
            {errors.description.message}
          </p>
        )}
        <p className="text-[#6B7280] text-xs mt-2">
          Projenizin amacını, kapsamını ve hedeflerini açıklayın
        </p>
      </div>

      {/* Form Actions */}
      <div className="flex items-center justify-end gap-3 pt-6 border-t border-[#30363D]/30">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          disabled={isSubmitting}
          className="border-[#30363D] text-[#9CA3AF] hover:bg-[#21262D] hover:text-[#E5E7EB] px-6"
        >
          İptal
        </Button>
        <Button
          type="submit"
          disabled={isSubmitting}
          className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/30 hover:shadow-[#8B5CF6]/50 px-6 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isSubmitting ? (
            <>
              <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></div>
              Oluşturuluyor...
            </>
          ) : (
            <>
              <Plus className="w-4 h-4 mr-2" />
              Projeyi Oluştur
            </>
          )}
        </Button>
      </div>
    </form>
  );
}
