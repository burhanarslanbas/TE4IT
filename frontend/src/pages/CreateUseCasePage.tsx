/**
 * Create UseCase Page
 * Route: /projects/:projectId/modules/:moduleId/usecases/new
 */
import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import { UseCaseService } from '../services/useCaseService';
import { ModuleService } from '../services/moduleService';
import { ProjectService } from '../services/projectService';
import type { Module, Project } from '../types';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { ArrowLeft, Loader2, AlertTriangle, Layers, AlertCircle } from 'lucide-react';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import { Alert, AlertDescription, AlertTitle } from '../components/ui/alert';

interface CreateUseCaseForm {
  title: string;
  description: string;
  importantNotes: string;
}

export function CreateUseCasePage() {
  const { projectId, moduleId } = useParams<{ projectId: string; moduleId: string }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [module, setModule] = useState<Module | null>(null);
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { 
    register, 
    handleSubmit, 
    formState: { errors }, 
    watch 
  } = useForm<CreateUseCaseForm>({
    defaultValues: {
      title: '',
      description: '',
      importantNotes: '',
    }
  });

  const descriptionLength = watch('description')?.length || 0;
  const notesLength = watch('importantNotes')?.length || 0;

  useEffect(() => {
    if (projectId && moduleId) {
      loadData();
    }
  }, [projectId, moduleId]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [projectData, moduleData] = await Promise.all([
        projectId ? ProjectService.getProject(projectId) : Promise.resolve(null),
        moduleId ? ModuleService.getModule(moduleId) : Promise.resolve(null),
      ]);
      
      setProject(projectData);
      setModule(moduleData);
      
      // Modül arşivdeyse uyarı göster
      if (moduleData && moduleData.status === 'Archived') {
        toast.warning('Bu modül arşivlenmiş durumda', {
          description: 'Use case oluşturmak için modülü aktifleştirin',
        });
      }
      
      // Proje arşivdeyse uyarı göster
      if (projectData && projectData.status === 'Archived') {
        toast.warning('Bu proje arşivlenmiş durumda', {
          description: 'Use case oluşturmak için projeyi aktifleştirin',
        });
      }
    } catch (error: any) {
      toast.error('Veri yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
      if (projectId && moduleId) {
        navigate(`/projects/${projectId}/modules/${moduleId}`);
      }
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data: CreateUseCaseForm) => {
    if (!moduleId || !projectId || !module || !project) return;
    
    // Modül veya proje arşivdeyse işlem yapma
    if (module.status === 'Archived') {
      toast.error('Use case oluşturulamaz', {
        description: 'Modül arşivlenmiş durumda',
      });
      return;
    }
    
    if (project.status === 'Archived') {
      toast.error('Use case oluşturulamaz', {
        description: 'Proje arşivlenmiş durumda',
      });
      return;
    }
    
    setIsSubmitting(true);
    try {
      const newUseCase = await UseCaseService.createUseCase(moduleId, {
        title: data.title,
        description: data.description || undefined,
        importantNotes: data.importantNotes || undefined,
      });
      
      toast.success('Use case oluşturuldu');
      
      // Navigate to the newly created use case detail page
      if (newUseCase && newUseCase.id) {
        navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${newUseCase.id}`);
      } else {
        // If no ID returned, go back to module page
        navigate(`/projects/${projectId}/modules/${moduleId}`);
      }
    } catch (error: any) {
      const errorMessage = error?.message || error?.response?.data?.message || 'Bir hata oluştu';
      toast.error('Use case oluşturulamadı', {
        description: errorMessage,
      });
    } finally {
      setIsSubmitting(false);
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

  if (!project || !module) {
    return null;
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

      {/* Main Content */}
      <div className="relative z-10 pt-24 px-4 sm:px-6 pb-12">
        <div className="max-w-4xl mx-auto">
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
                    <Link to="/projects" className="hover:text-[#8B5CF6] transition-colors">
                      Projeler
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link to={`/projects/${projectId}`} className="hover:text-[#8B5CF6] transition-colors">
                      {project.title}
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbLink asChild>
                    <Link to={`/projects/${projectId}/modules/${moduleId}`} className="hover:text-[#8B5CF6] transition-colors">
                      {module.title}
                    </Link>
                  </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator />
                <BreadcrumbItem>
                  <BreadcrumbPage className="text-[#E5E7EB]">Yeni Use Case</BreadcrumbPage>
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
              onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}`)}
              className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]/50"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              Modüle Dön
            </Button>
          </motion.div>

          {/* Form Card */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.4, delay: 0.2 }}
            className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-6 sm:p-8 shadow-lg relative overflow-hidden"
          >
            {/* Top Gradient Line */}
            <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-transparent via-[#8B5CF6]/50 to-transparent" />

            {/* Header */}
            <div className="flex items-start gap-4 mb-8">
              <div className="w-14 h-14 bg-gradient-to-br from-[#8B5CF6]/20 to-[#2DD4BF]/20 rounded-xl flex items-center justify-center flex-shrink-0">
                <Layers className="w-7 h-7 text-[#8B5CF6]" />
              </div>
              <div>
                <h1 className="text-3xl font-bold mb-2 bg-gradient-to-r from-[#E5E7EB] to-[#9CA3AF] bg-clip-text text-transparent">
                  Yeni Use Case Oluştur
                </h1>
                <p className="text-[#9CA3AF]">
                  <span className="font-medium text-[#E5E7EB]">{module.title}</span> modülü için yeni bir use case oluşturun
                </p>
              </div>
            </div>

            {/* Status Warning */}
            {(module.status === 'Archived' || project.status === 'Archived') && (
              <Alert className="mb-6 border-[#F59E0B] bg-[#F59E0B]/10">
                <AlertCircle className="h-4 w-4 text-[#F59E0B]" />
                <AlertTitle className="text-[#F59E0B] font-semibold">Uyarı</AlertTitle>
                <AlertDescription className="text-[#E5E7EB]">
                  {module.status === 'Archived' && project.status === 'Archived' ? (
                    <>
                      Bu proje ve modül arşivlenmiş durumda. Use case oluşturmak için{' '}
                      <Link to={`/projects/${projectId}`} className="underline hover:text-[#8B5CF6]">
                        projeyi
                      </Link>{' '}
                      ve{' '}
                      <Link to={`/projects/${projectId}/modules/${moduleId}`} className="underline hover:text-[#8B5CF6]">
                        modülü
                      </Link>{' '}
                      aktifleştirmeniz gerekiyor.
                    </>
                  ) : module.status === 'Archived' ? (
                    <>
                      Bu modül arşivlenmiş durumda. Use case oluşturmak için{' '}
                      <Link to={`/projects/${projectId}/modules/${moduleId}`} className="underline hover:text-[#8B5CF6]">
                        modülü aktifleştirin
                      </Link>
                      .
                    </>
                  ) : (
                    <>
                      Bu proje arşivlenmiş durumda. Use case oluşturmak için{' '}
                      <Link to={`/projects/${projectId}`} className="underline hover:text-[#8B5CF6]">
                        projeyi aktifleştirin
                      </Link>
                      .
                    </>
                  )}
                </AlertDescription>
              </Alert>
            )}

            {/* Form */}
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
              {/* Title */}
              <div>
                <Label htmlFor="title" className="text-[#E5E7EB] mb-2 block">
                  Başlık <span className="text-[#EF4444]">*</span>
                </Label>
                <Input
                  id="title"
                  {...register('title', {
                    required: 'Başlık zorunludur',
                    minLength: { value: 3, message: 'Başlık en az 3 karakter olmalıdır' },
                    maxLength: { value: 100, message: 'Başlık en fazla 100 karakter olmalıdır' },
                  })}
                  placeholder="Use case başlığı..."
                  disabled={module.status === 'Archived' || project.status === 'Archived'}
                  className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:ring-2 focus:ring-[#8B5CF6] transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                />
                {errors.title && (
                  <p className="text-[#EF4444] text-sm mt-2 flex items-center gap-1">
                    <AlertTriangle className="w-3 h-3" />
                    {errors.title.message}
                  </p>
                )}
              </div>

              {/* Description */}
              <div>
                <div className="flex items-center justify-between mb-2">
                  <Label htmlFor="description" className="text-[#E5E7EB]">
                    Açıklama
                  </Label>
                  <span className="text-xs text-[#6B7280]">{descriptionLength}/1000</span>
                </div>
                <Textarea
                  id="description"
                  {...register('description', {
                    maxLength: { value: 1000, message: 'Açıklama en fazla 1000 karakter olmalıdır' },
                  })}
                  placeholder="Use case'in detaylı açıklaması..."
                  disabled={module.status === 'Archived' || project.status === 'Archived'}
                  className="bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] min-h-[150px] focus:ring-2 focus:ring-[#8B5CF6] transition-all resize-none disabled:opacity-50 disabled:cursor-not-allowed"
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
                    Önemli Notlar
                  </Label>
                  <span className="text-xs text-[#6B7280]">{notesLength}/500</span>
                </div>
                <Textarea
                  id="importantNotes"
                  {...register('importantNotes', {
                    maxLength: { value: 500, message: 'Önemli notlar en fazla 500 karakter olmalıdır' },
                  })}
                  placeholder="⚠️ Dikkat edilmesi gereken önemli notlar..."
                  disabled={module.status === 'Archived' || project.status === 'Archived'}
                  className="bg-[#0D1117] border-[#F59E0B]/30 text-[#E5E7EB] placeholder:text-[#6B7280] min-h-[120px] focus:ring-2 focus:ring-[#F59E0B] transition-all resize-none disabled:opacity-50 disabled:cursor-not-allowed"
                />
                {errors.importantNotes && (
                  <p className="text-[#EF4444] text-sm mt-2 flex items-center gap-1">
                    <AlertTriangle className="w-3 h-3" />
                    {errors.importantNotes.message}
                  </p>
                )}
              </div>

              {/* Action Buttons */}
              <div className="flex gap-3 pt-4 border-t border-[#30363D]/50">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}`)}
                  disabled={isSubmitting}
                  className="flex-1 border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:border-[#8B5CF6]/50 transition-all"
                >
                  İptal
                </Button>
                <Button
                  type="submit"
                  disabled={isSubmitting || module.status === 'Archived' || project.status === 'Archived'}
                  className="flex-1 bg-gradient-to-r from-[#8B5CF6] via-[#7C3AED] to-[#6D28D9] text-white hover:from-[#7C3AED] hover:via-[#6D28D9] hover:to-[#5B21B6] shadow-lg shadow-[#8B5CF6]/40 hover:shadow-[#8B5CF6]/60 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed relative overflow-hidden group"
                >
                  {isSubmitting ? (
                    <>
                      <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                      Oluşturuluyor...
                    </>
                  ) : (
                    <>
                      <span className="relative z-10">Use Case Oluştur</span>
                      <motion.div
                        className="absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent"
                        initial={{ x: '-100%' }}
                        whileHover={{ x: '100%' }}
                        transition={{ duration: 0.6 }}
                      />
                    </>
                  )}
                </Button>
              </div>
            </form>
          </motion.div>
        </div>
      </div>
    </div>
  );
}

export default CreateUseCasePage;
