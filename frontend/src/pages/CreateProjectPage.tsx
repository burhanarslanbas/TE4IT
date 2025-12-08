/**
 * Create Project Page
 * Route: /projects/new
 * Modern Glassmorphism Design
 */
import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import { ProjectService } from '../services/projectService';
import { ApiError } from '../services/api';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { ArrowLeft, Plus, FileText } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';

interface CreateProjectForm {
  title: string;
  description: string;
}

export function CreateProjectPage() {
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);
  
  const { 
    register, 
    handleSubmit, 
    formState: { errors, isValid, isDirty },
    watch 
  } = useForm<CreateProjectForm>({
    mode: 'onChange', // Kullanıcı yazarken validasyon çalışsın
  });

  const handleSubmitClick = () => {
    const currentTitle = watch('title');
    const currentDescription = watch('description');
    
    console.log('🔍 Submit Click - Form Values:', {
      title: currentTitle,
      titleLength: currentTitle?.length,
      description: currentDescription,
      isValid,
      isDirty,
      errors
    });
  };

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
      
      // Kısa bir gecikme sonrası proje detay sayfasına yönlendir
      setTimeout(() => {
        navigate(`/projects/${project.id}`);
      }, 500);
    } catch (error: any) {
      // ApiError ise detaylı mesaj göster
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

  const handleCancel = () => {
    navigate('/projects');
  };

  return (
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] relative overflow-hidden">
      {/* Animated Background Blobs */}
      <div className="fixed inset-0 pointer-events-none overflow-hidden">
        <motion.div
          className="absolute -top-40 -right-40 w-[600px] h-[600px] bg-[#8B5CF6] opacity-[0.08] blur-[120px] rounded-full"
          animate={{
            scale: [1, 1.2, 1],
            x: [0, 40, 0],
            y: [0, -30, 0],
          }}
          transition={{
            duration: 20,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
        <motion.div
          className="absolute -bottom-40 -left-40 w-[600px] h-[600px] bg-[#2DD4BF] opacity-[0.08] blur-[120px] rounded-full"
          animate={{
            scale: [1, 1.3, 1],
            x: [0, -40, 0],
            y: [0, 30, 0],
          }}
          transition={{
            duration: 25,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        />
      </div>

      {/* Main Content Container */}
      <div className="relative z-10 pt-24 px-6 pb-12">
        <div className="max-w-3xl mx-auto">
          {/* Breadcrumb */}
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3 }}
            className="mb-6"
          >
            <Breadcrumb>
              <BreadcrumbList>
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link to="/projects" className="hover:text-[#8B5CF6] transition-colors">Projeler</Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbPage>Yeni Proje</BreadcrumbPage>
                </BreadcrumbItem>
              </BreadcrumbList>
            </Breadcrumb>
          </motion.div>

          {/* Back Button */}
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ duration: 0.3, delay: 0.1 }}
          >
            <Button
              variant="ghost"
              onClick={handleCancel}
              className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]/50"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              Projelere Dön
            </Button>
          </motion.div>

          {/* Create Project Form Card */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.2 }}
            className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 shadow-lg"
          >
            {/* Header */}
            <div className="mb-8">
              <div className="flex items-center gap-3 mb-2">
                <div className="w-12 h-12 bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] rounded-xl flex items-center justify-center shadow-lg shadow-[#8B5CF6]/30">
                  <Plus className="w-6 h-6 text-white" />
                </div>
                <h1 className="text-3xl font-bold text-[#E5E7EB]">
                  Yeni Proje Oluştur
                </h1>
              </div>
              <p className="text-[#9CA3AF] text-sm ml-16">
                Yeni bir proje oluşturarak çalışmalarınıza başlayın
              </p>
            </div>

            {/* Form */}
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
                      console.log('🔍 Title Validate:', { value, trimmed: value?.trim(), length: value?.trim()?.length });
                      return value?.trim()?.length >= 3 || 'Proje başlığı en az 3 karakter olmalıdır';
                    }
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
                  onClick={handleCancel}
                  disabled={isSubmitting}
                  className="border-[#30363D] text-[#9CA3AF] hover:bg-[#21262D] hover:text-[#E5E7EB] px-6"
                >
                  İptal
                </Button>
                <Button
                  type="submit"
                  onClick={handleSubmitClick}
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
          </motion.div>

          {/* Help Text */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.5, delay: 0.4 }}
            className="mt-6 p-4 bg-[#161B22]/30 backdrop-blur-sm border border-[#30363D]/30 rounded-lg"
          >
            <p className="text-[#9CA3AF] text-sm text-center">
              💡 <strong className="text-[#E5E7EB]">İpucu:</strong> Proje oluşturduktan sonra modüller, use case'ler ve task'lar ekleyebilirsiniz.
            </p>
          </motion.div>
        </div>
      </div>
    </div>
  );
}

