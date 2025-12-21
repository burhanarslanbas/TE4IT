/**
 * Create Module Page
 * Route: /projects/:projectId/modules/new
 * Modern Glassmorphism Design
 */
import { useState, useEffect } from 'react';
import { useNavigate, Link, useParams } from 'react-router-dom';
import { motion } from 'motion/react';
import { ModuleService } from '../services/moduleService';
import { ProjectService } from '../services/projectService';
import { ApiError } from '../services/api';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { ArrowLeft, Plus, Layers } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import { validateSafeString } from '../utils/security';

interface CreateModuleForm {
  title: string;
  description: string;
}

export function CreateModulePage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [projectTitle, setProjectTitle] = useState<string>('');
  const [loading, setLoading] = useState(true);
  
  const { 
    register, 
    handleSubmit, 
    formState: { errors },
  } = useForm<CreateModuleForm>({
    mode: 'onChange',
  });

  // Proje bilgisini yükle
  useEffect(() => {
    const loadProject = async () => {
      if (!projectId) {
        navigate('/projects');
        return;
      }
      
      try {
        const project = await ProjectService.getProject(projectId);
        setProjectTitle(project.title);
      } catch (error: any) {
        toast.error('Proje yüklenemedi', {
          description: error.message || 'Bir hata oluştu',
        });
        navigate(`/projects/${projectId}`);
      } finally {
        setLoading(false);
      }
    };

    loadProject();
  }, [projectId, navigate]);

  const handleCreateModule = async (data: CreateModuleForm) => {
    if (!projectId) return;

    try {
      setIsSubmitting(true);
      
      // Input sanitization
      const titleValidation = validateSafeString(data.title.trim(), 100);
      const descriptionValidation = data.description?.trim()
        ? validateSafeString(data.description.trim(), 1000)
        : { isValid: true, sanitized: '', errors: [] };

      if (!titleValidation.isValid || !descriptionValidation.isValid) {
        toast.error('Geçersiz giriş', {
          description: 'Lütfen geçerli karakterler kullanın.',
        });
        return;
      }

      const module = await ModuleService.createModule(projectId, {
        title: titleValidation.sanitized,
        description: descriptionValidation.sanitized || undefined,
      });
      
      toast.success('Modül oluşturuldu', {
        description: 'Yeni modülünüz başarıyla oluşturuldu.',
      });
      
      // Modül detay sayfasına yönlendir
      setTimeout(() => {
        navigate(`/projects/${projectId}/modules/${module.id}`);
      }, 500);
    } catch (error: any) {
      let errorMessage = 'Bir hata oluştu. Lütfen tekrar deneyin.';
      
      if (error instanceof ApiError) {
        errorMessage = error.message || 'Modül oluşturulamadı';
        if (error.errors && error.errors.length > 0) {
          errorMessage = error.errors.join(', ');
        }
      } else if (error?.message) {
        errorMessage = error.message;
      }
      
      toast.error('Modül oluşturulamadı', {
        description: errorMessage,
        duration: 5000,
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    if (projectId) {
      navigate(`/projects/${projectId}`);
    } else {
      navigate('/projects');
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center relative overflow-hidden">
        <div className="fixed inset-0 pointer-events-none overflow-hidden">
          <motion.div
            className="absolute -top-40 -right-40 w-[600px] h-[600px] bg-[#8B5CF6] opacity-[0.08] blur-[120px] rounded-full"
            animate={{ scale: [1, 1.2, 1], rotate: [0, 90, 0] }}
            transition={{ duration: 20, repeat: Infinity }}
          />
        </div>
        <div className="text-center relative z-10">
          <motion.div
            animate={{ rotate: 360 }}
            transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
            className="w-12 h-12 border-4 border-[#8B5CF6] border-t-transparent rounded-full mx-auto mb-4"
          />
          <p className="text-[#9CA3AF] text-lg">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#0D1117] text-[#E5E7EB] relative overflow-hidden" style={{ paddingTop: 'var(--navbar-height)' }}>
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
                {projectId && (
                  <>
                    <BreadcrumbItem>
                      <BreadcrumbLink asChild>
                        <Link to={`/projects/${projectId}`} className="hover:text-[#8B5CF6] transition-colors">
                          {projectTitle || 'Proje'}
                        </Link>
                      </BreadcrumbLink>
                    </BreadcrumbItem>
                    <BreadcrumbSeparator />
                  </>
                )}
                <BreadcrumbItem>
                  <BreadcrumbPage>Yeni Modül</BreadcrumbPage>
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
              {projectId ? 'Projeye Dön' : 'Projelere Dön'}
            </Button>
          </motion.div>

          {/* Create Module Form Card */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.2 }}
            className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 shadow-lg relative overflow-hidden"
          >
            {/* Top Gradient Line */}
            <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-[#8B5CF6]/50 to-transparent" />

            {/* Header */}
            <div className="mb-8">
              <div className="flex items-center gap-3 mb-2">
                <div className="w-12 h-12 bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] rounded-xl flex items-center justify-center shadow-lg shadow-[#8B5CF6]/30">
                  <Layers className="w-6 h-6 text-white" />
                </div>
                <h1 className="text-3xl font-bold text-[#E5E7EB]">
                  Yeni Modül Oluştur
                </h1>
              </div>
              <p className="text-[#9CA3AF] text-sm ml-16">
                {projectTitle && (
                  <>
                    <span className="text-[#8B5CF6] font-medium">{projectTitle}</span> projesi için yeni bir modül oluşturun
                  </>
                )}
                {!projectTitle && 'Yeni bir modül oluşturarak çalışmalarınıza başlayın'}
              </p>
            </div>

            {/* Form */}
            <form onSubmit={handleSubmit(handleCreateModule)} className="space-y-6">
              {/* Module Title */}
              <div>
                <Label htmlFor="title" className="text-[#E5E7EB] text-sm font-medium mb-2 block flex items-center gap-2">
                  <Layers className="w-4 h-4" />
                  Modül Başlığı *
                </Label>
                <Input
                  id="title"
                  {...register('title', {
                    required: 'Modül başlığı zorunludur',
                    minLength: { value: 3, message: 'En az 3 karakter olmalıdır' },
                    maxLength: { value: 100, message: 'En fazla 100 karakter olabilir' },
                    validate: (value) => {
                      return value?.trim()?.length >= 3 || 'Modül başlığı en az 3 karakter olmalıdır';
                    }
                  })}
                  className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:border-[#8B5CF6] focus:ring-2 focus:ring-[#8B5CF6]/20 h-12 text-base"
                  placeholder="Örn: Kullanıcı Yönetimi, Ödeme Sistemi, Ürün Kataloğu..."
                  disabled={isSubmitting}
                />
                {errors.title && (
                  <p className="text-[#EF4444] text-xs mt-2 flex items-center gap-1">
                    <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
                    {errors.title.message}
                  </p>
                )}
              </div>

              {/* Module Description */}
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
                  placeholder="Modülünüz hakkında detaylı bir açıklama yazın... (Opsiyonel)"
                  disabled={isSubmitting}
                />
                {errors.description && (
                  <p className="text-[#EF4444] text-xs mt-2 flex items-center gap-1">
                    <span className="w-1 h-1 bg-[#EF4444] rounded-full"></span>
                    {errors.description.message}
                  </p>
                )}
                <p className="text-[#6B7280] text-xs mt-2">
                  Modülün amacını, kapsamını ve işlevlerini açıklayın
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
                      Modülü Oluştur
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
              💡 <strong className="text-[#E5E7EB]">İpucu:</strong> Modül oluşturduktan sonra use case'ler ve task'lar ekleyebilirsiniz.
            </p>
          </motion.div>
        </div>
      </div>
    </div>
  );
}
