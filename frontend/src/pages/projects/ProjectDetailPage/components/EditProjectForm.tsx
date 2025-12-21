/**
 * Edit Project Form Component
 */
import { useForm } from 'react-hook-form';
import { Button } from '../../../../components/ui/button';
import { Input } from '../../../../components/ui/input';
import { Label } from '../../../../components/ui/label';
import { Textarea } from '../../../../components/ui/textarea';
import { sanitizeInput, validateSafeString } from '../../../../utils/security';
import type { Project } from '../../../../types';

interface EditProjectFormData {
  title: string;
  description: string;
}

interface EditProjectFormProps {
  project: Project;
  onSubmit: (data: EditProjectFormData) => Promise<void>;
  onCancel: () => void;
}

export function EditProjectForm({ project, onSubmit, onCancel }: EditProjectFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<EditProjectFormData>({
    defaultValues: {
      title: project.title,
      description: project.description || '',
    },
  });

  const handleFormSubmit = async (data: EditProjectFormData) => {
    // Input sanitization
    const titleValidation = validateSafeString(data.title, 120);
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
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-4">
      <div>
        <Label htmlFor="edit-title">Proje Başlığı *</Label>
        <Input
          id="edit-title"
          {...register('title', {
            required: 'Proje başlığı zorunludur',
            minLength: { value: 3, message: 'En az 3 karakter olmalıdır' },
            maxLength: { value: 120, message: 'En fazla 120 karakter olabilir' },
          })}
          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB]"
        />
        {errors.title && (
          <p className="text-[#EF4444] text-xs mt-2">{errors.title.message}</p>
        )}
      </div>
      <div>
        <Label htmlFor="edit-description">Açıklama</Label>
        <Textarea
          id="edit-description"
          {...register('description', {
            maxLength: { value: 1000, message: 'En fazla 1000 karakter olabilir' },
          })}
          className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] min-h-[100px]"
        />
        {errors.description && (
          <p className="text-[#EF4444] text-xs mt-2">{errors.description.message}</p>
        )}
      </div>
      <div className="flex justify-end gap-2">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
        >
          İptal
        </Button>
        <Button type="submit" className="bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] hover:from-[#7C3AED] hover:to-[#6D28D9] text-white">
          Kaydet
        </Button>
      </div>
    </form>
  );
}

