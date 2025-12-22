/**
 * Edit Task Page
 * Route: /projects/:projectId/modules/:moduleId/usecases/:useCaseId/tasks/:taskId/edit
 */
import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { motion } from 'motion/react';
import { TaskService } from '../services/taskService';
import { UseCaseService } from '../services/useCaseService';
import { ModuleService } from '../services/moduleService';
import { ProjectService } from '../services/projectService';
import type { UseCase, Module, Project, TaskType, UpdateTaskRequest, Task } from '../types';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Alert, AlertDescription, AlertTitle } from '../components/ui/alert';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import { toast } from 'sonner';
import { ArrowLeft, AlertTriangle, Loader2, FileText, Bug, Wrench, TestTube } from 'lucide-react';

interface EditTaskForm {
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

export function EditTaskPage() {
  const { projectId, moduleId, useCaseId, taskId } = useParams<{ 
    projectId: string; 
    moduleId: string; 
    useCaseId: string;
    taskId: string;
  }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [module, setModule] = useState<Module | null>(null);
  const [useCase, setUseCase] = useState<UseCase | null>(null);
  const [task, setTask] = useState<Task | null>(null);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
    setValue,
    reset,
  } = useForm<EditTaskForm>({
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

  useEffect(() => {
    if (taskId && projectId && moduleId && useCaseId) {
      loadData();
    }
  }, [taskId, projectId, moduleId, useCaseId]);

  const loadData = async () => {
    if (!projectId || !moduleId || !useCaseId || !taskId) return;

    try {
      setLoading(true);
      const [projectData, moduleData, useCaseData, taskData] = await Promise.all([
        ProjectService.getProject(projectId),
        ModuleService.getModule(moduleId),
        UseCaseService.getUseCase(useCaseId),
        TaskService.getTask(taskId),
      ]);

      setProject(projectData);
      setModule(moduleData);
      setUseCase(useCaseData);
      setTask(taskData);

      // Form'u task verileriyle doldur
      if (taskData) {
        reset({
          title: taskData.title,
          description: taskData.description || '',
          importantNotes: taskData.importantNotes || '',
          type: taskData.type,
          dueDate: taskData.dueDate ? new Date(taskData.dueDate).toISOString().split('T')[0] : '',
        });
      }

      // Uyarı göster eğer arşivdeyse
      if (projectData.status === 'Archived') {
        toast.warning('Proje arşivde', {
          description: 'Bu proje arşivlenmiş. Task düzenlemek için projeyi aktifleştirin.',
        });
      }
      if (moduleData.status === 'Archived') {
        toast.warning('Modül arşivde', {
          description: 'Bu modül arşivlenmiş. Task düzenlemek için modülü aktifleştirin.',
        });
      }
      if (useCaseData.status === 'Archived') {
        toast.warning('Use case arşivde', {
          description: 'Bu use case arşivlenmiş. Task düzenlemek için use case\'i aktifleştirin.',
        });
      }
    } catch (error: any) {
      console.error('Load data error:', error);
      toast.error('Veriler yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
      navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${taskId}`);
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (data: EditTaskForm) => {
    if (!useCaseId || !useCase || !module || !project || !taskId) return;

    // Aktiflik kontrolü
    if (project.status === 'Archived') {
      toast.error('Proje arşivde', {
        description: 'Task düzenlemek için projeyi aktifleştirin.',
      });
      return;
    }

    if (module.status === 'Archived') {
      toast.error('Modül arşivde', {
        description: 'Task düzenlemek için modülü aktifleştirin.',
      });
      return;
    }

    if (useCase.status === 'Archived') {
      toast.error('Use case arşivde', {
        description: 'Task düzenlemek için use case\'i aktifleştirin.',
      });
      return;
    }

    try {
      setSubmitting(true);
      console.log('[EDIT TASK PAGE] Starting task update');

      const taskData: UpdateTaskRequest = {
        title: data.title,
        description: data.description || undefined,
        importantNotes: data.importantNotes || undefined,
        type: data.type,
        dueDate: data.dueDate || undefined,
      };

      await TaskService.updateTask(taskId, taskData);

      console.log('[EDIT TASK PAGE] Successfully updated task');
      toast.success('Task güncellendi');

      // Task detail sayfasına yönlendir
      navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${taskId}`);
    } catch (error: any) {
      console.error('[EDIT TASK PAGE] Error:', error);
      toast.error('Task güncellenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center relative overflow-hidden" style={{ paddingTop: 'var(--navbar-height)' }}>
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

  if (!project || !module || !useCase || !task) {
    return null;
  }

  const isArchived = module.status === 'Archived' || project.status === 'Archived' || useCase.status === 'Archived';

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
      <div className="relative z-10 max-w-4xl mx-auto px-4 sm:px-6 py-12">
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
                    Projects
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
                <BreadcrumbLink asChild>
                  <Link to={`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`} className="hover:text-[#8B5CF6] transition-colors">
                    {useCase.title}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link to={`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${taskId}`} className="hover:text-[#8B5CF6] transition-colors">
                    {task.title}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage>Düzenle</BreadcrumbPage>
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
            onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${taskId}`)}
            className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]/50"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Task Detayına Geri Dön
          </Button>
        </motion.div>

        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.2 }}
          className="mb-8"
        >
          <h1 className="text-4xl font-bold text-[#8B5CF6] mb-3">
            Task Düzenle
          </h1>
          <p className="text-[#9CA3AF] text-lg">
            <span className="font-semibold text-[#8B5CF6]">{task.title}</span> task'ını düzenleyin
          </p>
        </motion.div>

        {/* Archive Warning */}
        {isArchived && (
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.3, delay: 0.3 }}
            className="mb-6"
          >
            <Alert className="bg-[#F59E0B]/10 border-[#F59E0B]/30">
              <AlertTriangle className="h-5 w-5 text-[#F59E0B]" />
              <AlertTitle className="text-[#F59E0B]">Arşiv Uyarısı</AlertTitle>
              <AlertDescription className="text-[#E5E7EB]">
                {project.status === 'Archived' && (
                  <p>
                    Proje arşivde. Task düzenlemek için{' '}
                    <Link to={`/projects/${projectId}`} className="underline text-[#8B5CF6] hover:text-[#7C3AED]">
                      projeyi aktifleştirin
                    </Link>
                    .
                  </p>
                )}
                {module.status === 'Archived' && (
                  <p>
                    Modül arşivde. Task düzenlemek için{' '}
                    <Link to={`/projects/${projectId}/modules/${moduleId}`} className="underline text-[#8B5CF6] hover:text-[#7C3AED]">
                      modülü aktifleştirin
                    </Link>
                    .
                  </p>
                )}
                {useCase.status === 'Archived' && (
                  <p>
                    Use case arşivde. Task düzenlemek için{' '}
                    <Link to={`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`} className="underline text-[#8B5CF6] hover:text-[#7C3AED]">
                      use case'i aktifleştirin
                    </Link>
                    .
                  </p>
                )}
              </AlertDescription>
            </Alert>
          </motion.div>
        )}

        {/* Form */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.4 }}
          className="bg-[#161B22]/60 backdrop-blur-md border border-[#30363D]/50 rounded-2xl p-8 shadow-xl"
        >
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
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
                disabled={submitting || isArchived}
                className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] focus:ring-2 focus:ring-[#8B5CF6] transition-all ${
                  isArchived ? 'opacity-50 cursor-not-allowed' : ''
                }`}
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
                disabled={submitting || isArchived}
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
                disabled={submitting || isArchived}
                className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] focus:ring-2 focus:ring-[#8B5CF6] transition-all ${
                  isArchived ? 'opacity-50 cursor-not-allowed' : ''
                }`}
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
                disabled={submitting || isArchived}
                className={`bg-[#0D1117] border-[#30363D] text-[#E5E7EB] placeholder:text-[#6B7280] min-h-[150px] focus:ring-2 focus:ring-[#8B5CF6] transition-all resize-none ${
                  isArchived ? 'opacity-50 cursor-not-allowed' : ''
                }`}
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
                disabled={submitting || isArchived}
                className={`bg-[#0D1117] border-[#F59E0B]/30 text-[#E5E7EB] placeholder:text-[#6B7280] min-h-[120px] focus:ring-2 focus:ring-[#F59E0B] transition-all resize-none ${
                  isArchived ? 'opacity-50 cursor-not-allowed' : ''
                }`}
              />
              {errors.importantNotes && (
                <p className="text-[#EF4444] text-sm mt-2 flex items-center gap-1">
                  <AlertTriangle className="w-3 h-3" />
                  {errors.importantNotes.message}
                </p>
              )}
            </div>

            {/* Action Buttons */}
            <div className="flex gap-4 pt-6">
              <Button
                type="button"
                variant="outline"
                onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${taskId}`)}
                disabled={submitting}
                className="flex-1 border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] transition-all"
              >
                İptal
              </Button>
              <Button
                type="submit"
                disabled={submitting || isArchived}
                className="flex-1 bg-gradient-to-r from-[#8B5CF6] to-[#7C3AED] text-white hover:from-[#7C3AED] hover:to-[#6D28D9] shadow-lg shadow-[#8B5CF6]/40 hover:shadow-[#8B5CF6]/60 transition-all disabled:opacity-50 disabled:cursor-not-allowed relative overflow-hidden group"
              >
                {submitting ? (
                  <>
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    Güncelleniyor...
                  </>
                ) : (
                  <>Task Güncelle</>
                )}
                <motion.div
                  className="absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent"
                  initial={{ x: '-100%' }}
                  whileHover={{ x: '100%' }}
                  transition={{ duration: 0.5 }}
                />
              </Button>
            </div>
          </form>
        </motion.div>
      </div>
    </div>
  );
}
