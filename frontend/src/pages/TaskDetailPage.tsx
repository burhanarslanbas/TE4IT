/**
 * Task Detail Page
 * Route: /projects/:projectId/modules/:moduleId/usecases/:useCaseId/tasks/:taskId
 */
import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import { useTaskDetail } from '../hooks/useTaskDetail';
import { useTaskRelations } from '../hooks/useTaskRelations';
import { useTasks } from '../hooks/useTasks';
import { UseCaseService } from '../services/useCaseService';
import { ModuleService } from '../services/moduleService';
import { ProjectService } from '../services/projectService';
import { TaskService } from '../services/taskService';
import type { UseCase, Module, Project } from '../types';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../components/ui/breadcrumb';
import { toast } from 'sonner';
import { ArrowLeft, Edit, Trash2, FileText, Bug, Wrench, TestTube, Calendar, User, Clock, AlertTriangle } from 'lucide-react';
import { taskTypeConfig, taskStateConfig } from '../utils/taskHelpers';
import { ConfirmDeleteDialog } from '../components/ConfirmDeleteDialog';
import { TaskStateActions } from '../components/tasks/TaskStateActions';
import { TaskAssigneeSection } from '../components/tasks/TaskAssigneeSection';
import { TaskRelationsList } from '../components/tasks/TaskRelationsList';
import { AddTaskRelationModal } from '../components/tasks/AddTaskRelationModal';

export function TaskDetailPage() {
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
  const [loadingContext, setLoadingContext] = useState(true);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [addRelationModalOpen, setAddRelationModalOpen] = useState(false);

  // Task detail hook
  const { task, loading: taskLoading, refetch: refetchTask, updateState, assignTask } = useTaskDetail(taskId);
  
  // Task relations hook
  const { relations, loading: relationsLoading, createRelation, deleteRelation } = useTaskRelations(taskId);
  
  // Available tasks hook (for relation modal)
  const { tasks: availableTasks } = useTasks(useCaseId, { pageSize: 100 });

  useEffect(() => {
    if (projectId && moduleId && useCaseId) {
      loadContext();
    }
  }, [projectId, moduleId, useCaseId]);

  const loadContext = async () => {
    try {
      setLoadingContext(true);
      const [projectData, moduleData, useCaseData] = await Promise.all([
        ProjectService.getProject(projectId!),
        ModuleService.getModule(moduleId!),
        UseCaseService.getUseCase(useCaseId!),
      ]);

      setProject(projectData);
      setModule(moduleData);
      setUseCase(useCaseData);
    } catch (error: any) {
      console.error('Load context error:', error);
      toast.error('Bilgiler yüklenemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    } finally {
      setLoadingContext(false);
    }
  };

  const handleDeleteTask = async () => {
    if (!taskId) return;
    
    try {
      await TaskService.deleteTask(taskId);
      toast.success('Task silindi');
      navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`);
    } catch (error: any) {
      console.error('Delete task error:', error);
      toast.error('Task silinemedi', {
        description: error.message || 'Bir hata oluştu',
      });
    }
  };

  const loading = taskLoading || loadingContext;

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

  if (!task || !project || !module || !useCase) {
    return (
      <div className="min-h-screen bg-[#0D1117] flex items-center justify-center" style={{ paddingTop: 'var(--navbar-height)' }}>
        <div className="text-center">
          <AlertTriangle className="w-16 h-16 text-[#F59E0B] mx-auto mb-4" />
          <p className="text-[#E5E7EB] text-xl mb-2">Task bulunamadı</p>
          <Button
            variant="ghost"
            onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`)}
            className="text-[#8B5CF6] hover:text-[#7C3AED]"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Geri Dön
          </Button>
        </div>
      </div>
    );
  }

  const typeConfig = taskTypeConfig[task.type];
  const stateConfig = taskStateConfig[task.state];
  const isOverdue = task.dueDate && new Date(task.dueDate) < new Date() && task.state !== 'Completed';

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
      <div className="relative z-10 max-w-6xl mx-auto px-4 sm:px-6 py-12">
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
                <BreadcrumbPage>{task.title}</BreadcrumbPage>
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
            onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`)}
            className="mb-6 text-[#E5E7EB] hover:text-[#8B5CF6] hover:bg-[#21262D]/50"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Use Case'e Geri Dön
          </Button>
        </motion.div>

        {/* Task Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.4, delay: 0.2 }}
          className="mb-8"
        >
          <div className="flex items-start justify-between gap-4 mb-6">
            <div className="flex-1 min-w-0">
              <h1 className="text-4xl font-bold bg-gradient-to-r from-[#E5E7EB] via-[#8B5CF6] to-[#2DD4BF] bg-clip-text text-transparent mb-4">
                {task.title}
              </h1>
              <div className="flex items-center gap-3 flex-wrap">
                <Badge className={`${typeConfig.color} border-0`}>
                  <span className="flex items-center gap-1.5">
                    {typeConfig.icon}
                    <span>{typeConfig.label}</span>
                  </span>
                </Badge>
                <Badge className={`${stateConfig.color} border-0`}>
                  {stateConfig.label}
                </Badge>
                {isOverdue && (
                  <Badge className="bg-[#EF4444]/10 text-[#EF4444] border border-[#EF4444]/30">
                    <AlertTriangle className="w-3 h-3 mr-1" />
                    Gecikmiş
                  </Badge>
                )}
              </div>
            </div>

            {/* Action Buttons */}
            <div className="flex items-center gap-2 flex-shrink-0">
              <Button
                variant="outline"
                size="sm"
                onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${taskId}/edit`)}
                className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D] hover:text-[#8B5CF6]"
              >
                <Edit className="w-4 h-4 mr-2" />
                Düzenle
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setDeleteDialogOpen(true)}
                className="border-[#30363D] text-[#EF4444] hover:bg-[#EF4444]/10 hover:border-[#EF4444]/30"
              >
                <Trash2 className="w-4 h-4 mr-2" />
                Sil
              </Button>
            </div>
          </div>
        </motion.div>

        {/* Task Content Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Main Content - Left Column */}
          <div className="lg:col-span-2 space-y-6">
            {/* Description Card */}
            {task.description && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4, delay: 0.3 }}
                className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6 hover:border-[#8B5CF6]/20 transition-all"
              >
                <h2 className="text-lg font-semibold text-[#E5E7EB] mb-4 flex items-center gap-2">
                  <FileText className="w-5 h-5 text-[#8B5CF6]" />
                  Açıklama
                </h2>
                <p className="text-[#9CA3AF] leading-relaxed whitespace-pre-wrap">
                  {task.description}
                </p>
              </motion.div>
            )}

            {/* Important Notes Card */}
            {task.importantNotes && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4, delay: 0.4 }}
                className="bg-[#161B22]/50 backdrop-blur-md border border-[#F59E0B]/20 rounded-xl p-6 hover:border-[#F59E0B]/30 transition-all"
              >
                <h2 className="text-lg font-semibold text-[#E5E7EB] mb-4 flex items-center gap-2">
                  <AlertTriangle className="w-5 h-5 text-[#F59E0B]" />
                  Önemli Notlar
                </h2>
                <p className="text-[#9CA3AF] leading-relaxed whitespace-pre-wrap">
                  {task.importantNotes}
                </p>
              </motion.div>
            )}

            {/* Task Relations Card */}
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.4, delay: 0.5 }}
              className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6"
            >
              <h2 className="text-lg font-semibold text-[#E5E7EB] mb-4">
                İlişkili Task'lar
              </h2>
              <TaskRelationsList
                relations={relations}
                loading={relationsLoading}
                onDelete={deleteRelation}
                onAddRelation={() => setAddRelationModalOpen(true)}
                onViewTask={(targetTaskId) => navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${targetTaskId}`)}
              />
            </motion.div>
          </div>

          {/* Sidebar - Right Column */}
          <div className="space-y-6">
            {/* Assignee Section */}
            <TaskAssigneeSection 
              task={task} 
              onAssign={assignTask}
              canAssign={true} // TODO: Permission kontrolü eklenecek
            />

            {/* Metadata Card */}
            <motion.div
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.4, delay: 0.3 }}
              className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6 space-y-4"
            >
              <h2 className="text-lg font-semibold text-[#E5E7EB] mb-4">
                Bilgiler
              </h2>

              {/* Assignee */}
              <div className="space-y-2">
                <div className="flex items-center gap-2 text-sm text-[#6B7280]">
                  <User className="w-4 h-4" />
                  <span>Atanan Kişi</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-8 h-8 rounded-full bg-gradient-to-br from-[#8B5CF6] to-[#7C3AED] flex items-center justify-center text-white text-sm font-medium">
                    {task.assigneeName?.[0] || 'U'}
                  </div>
                  <span className="text-[#E5E7EB]">{task.assigneeName || 'Atanmamış'}</span>
                </div>
              </div>

              {/* Started Date */}
              {task.startedDate && (
                <div className="space-y-2">
                  <div className="flex items-center gap-2 text-sm text-[#6B7280]">
                    <Clock className="w-4 h-4" />
                    <span>Başlangıç Tarihi</span>
                  </div>
                  <p className="text-[#E5E7EB]">
                    {new Date(task.startedDate).toLocaleDateString('tr-TR', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                    })}
                  </p>
                </div>
              )}

              {/* Due Date */}
              {task.dueDate && (
                <div className="space-y-2">
                  <div className="flex items-center gap-2 text-sm text-[#6B7280]">
                    <Calendar className="w-4 h-4" />
                    <span>Bitiş Tarihi</span>
                  </div>
                  <p className={`${isOverdue ? 'text-[#EF4444]' : 'text-[#E5E7EB]'}`}>
                    {new Date(task.dueDate).toLocaleDateString('tr-TR', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                    })}
                  </p>
                </div>
              )}
            </motion.div>

            {/* State Actions Card */}
            <motion.div
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ duration: 0.4, delay: 0.4 }}
              className="bg-[#161B22]/50 backdrop-blur-md border border-[#30363D]/50 rounded-xl p-6"
            >
              <h2 className="text-lg font-semibold text-[#E5E7EB] mb-4">
                Durum Aksiyonları
              </h2>
              <TaskStateActions task={task} onStateChange={updateState} />
            </motion.div>
          </div>
        </div>
      </div>

      {/* Delete Confirmation Dialog */}
      <ConfirmDeleteDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        entityType="Task"
        entityName={task.title}
        onConfirm={handleDeleteTask}
      />

      {/* Add Task Relation Modal */}
      <AddTaskRelationModal
        open={addRelationModalOpen}
        onOpenChange={setAddRelationModalOpen}
        currentTaskId={task.id}
        onSubmit={createRelation}
        availableTasks={availableTasks}
      />
    </div>
  );
}

