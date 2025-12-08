/**
 * Task Detail Page
 * Task detayları ve relations yönetimi
 */

import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import {
  ArrowLeft,
  Edit,
  Trash2,
  CheckSquare,
  Calendar,
  User,
  AlertTriangle,
  Play,
  CheckCircle,
  XCircle,
  RotateCcw,
  Link as LinkIcon,
  Plus,
  Eye,
} from 'lucide-react';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from '../components/ui/dialog';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '../components/ui/alert-dialog';
import { Label } from '../components/ui/label';
import { Textarea } from '../components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../components/ui/select';
import { Badge } from '../components/ui/badge';
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbSeparator,
} from '../components/ui/breadcrumb';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../components/ui/table';
import { RadioGroup, RadioGroupItem } from '../components/ui/radio-group';
import { taskService } from '../services/taskService';
import { projectService } from '../services/projectService';
import { moduleService } from '../services/moduleService';
import { useCaseService } from '../services/useCaseService';
import { hasPermission } from '../utils/permissions';
import {
  Task,
  Project,
  Module,
  UseCase,
  TaskType,
  TaskState,
  TaskRelation,
  TaskRelationType,
  UpdateTaskRequest,
  CreateTaskRelationRequest,
} from '../types';
import { toast } from 'sonner';
import { ApiError } from '../services/api';
import { useLanguage } from '../contexts/LanguageContext';
import { cn } from '../components/ui/utils';

// Task Type ve State renkleri
const taskTypeColors: Record<TaskType, string> = {
  [TaskType.Documentation]: 'bg-[#F97316]/10 text-[#F97316] border-[#F97316]/30',
  [TaskType.Feature]: 'bg-[#3B82F6]/10 text-[#3B82F6] border-[#3B82F6]/30',
  [TaskType.Test]: 'bg-[#9C27B0]/10 text-[#9C27B0] border-[#9C27B0]/30',
  [TaskType.Bug]: 'bg-[#EF4444]/10 text-[#EF4444] border-[#EF4444]/30',
};

const taskStateColors: Record<TaskState, string> = {
  [TaskState.NotStarted]: 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30',
  [TaskState.InProgress]: 'bg-[#3B82F6]/10 text-[#3B82F6] border-[#3B82F6]/30',
  [TaskState.Completed]: 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30',
  [TaskState.Cancelled]: 'bg-[#EF4444]/10 text-[#EF4444] border-[#EF4444]/30',
};

const relationTypeColors: Record<TaskRelationType, string> = {
  [TaskRelationType.Blocks]: 'bg-[#EF4444]/10 text-[#EF4444] border-[#EF4444]/30',
  [TaskRelationType.RelatesTo]: 'bg-[#3B82F6]/10 text-[#3B82F6] border-[#3B82F6]/30',
  [TaskRelationType.Fixes]: 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30',
  [TaskRelationType.Duplicates]: 'bg-[#F97316]/10 text-[#F97316] border-[#F97316]/30',
};

export const TaskDetailPage: React.FC = () => {
  const { projectId, moduleId, useCaseId, taskId } = useParams<{
    projectId: string;
    moduleId: string;
    useCaseId: string;
    taskId: string;
  }>();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [project, setProject] = useState<Project | null>(null);
  const [module, setModule] = useState<Module | null>(null);
  const [useCase, setUseCase] = useState<UseCase | null>(null);
  const [task, setTask] = useState<Task | null>(null);
  const [relations, setRelations] = useState<TaskRelation[]>([]);
  const [loading, setLoading] = useState(true);
  const [relationsLoading, setRelationsLoading] = useState(true);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isAddRelationModalOpen, setIsAddRelationModalOpen] = useState(false);
  const [editFormData, setEditFormData] = useState<UpdateTaskRequest>({
    title: '',
    description: '',
    importantNotes: '',
    type: TaskType.Feature,
    dueDate: '',
  });
  const [assigneeId, setAssigneeId] = useState<string>('');
  const [relationFormData, setRelationFormData] = useState<CreateTaskRelationRequest>({
    targetTaskId: '',
    relationType: TaskRelationType.RelatesTo,
  });
  const [isSaving, setIsSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isAssigning, setIsAssigning] = useState(false);
  const [isAddingRelation, setIsAddingRelation] = useState(false);

  // Proje, modül, use case ve task detaylarını yükle
  const loadData = async () => {
    if (!projectId || !moduleId || !useCaseId || !taskId) return;

    try {
      setLoading(true);
      const [projectResponse, moduleResponse, useCaseResponse, taskResponse] =
        await Promise.all([
          projectService.getById(projectId),
          moduleService.getById(moduleId),
          useCaseService.getById(useCaseId),
          taskService.getById(taskId),
        ]);

      if (projectResponse.success && projectResponse.data) {
        setProject(projectResponse.data);
      }

      if (moduleResponse.success && moduleResponse.data) {
        setModule(moduleResponse.data);
      }

      if (useCaseResponse.success && useCaseResponse.data) {
        setUseCase(useCaseResponse.data);
      }

      if (taskResponse.success && taskResponse.data) {
        setTask(taskResponse.data);
        setEditFormData({
          title: taskResponse.data.title,
          description: taskResponse.data.description || '',
          importantNotes: taskResponse.data.importantNotes || '',
          type: taskResponse.data.type,
          dueDate: taskResponse.data.dueDate || '',
        });
        setAssigneeId(taskResponse.data.assigneeId || '');
      }
    } catch (error) {
      console.error('Error loading data:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.loadError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('task.loadError'),
        });
      }
      navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`);
    } finally {
      setLoading(false);
    }
  };

  // Task ilişkilerini yükle
  const loadRelations = async () => {
    if (!taskId) return;

    try {
      setRelationsLoading(true);
      const response = await taskService.getRelations(taskId);

      if (response.success && response.data) {
        setRelations(response.data);
      }
    } catch (error) {
      console.error('Error loading relations:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.relationsLoadError'),
        });
      }
    } finally {
      setRelationsLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [projectId, moduleId, useCaseId, taskId]);

  useEffect(() => {
    if (task) {
      loadRelations();
    }
  }, [task]);

  // Task güncelle
  const handleUpdateTask = async () => {
    if (!taskId || !editFormData.title?.trim()) {
      toast.error(t('common.error'), {
        description: t('task.titleRequired'),
      });
      return;
    }

    try {
      setIsSaving(true);
      const response = await taskService.update(taskId, editFormData);

      if (response.success && response.data) {
        toast.success(t('common.success'), {
          description: t('task.updateSuccess'),
        });
        setIsEditModalOpen(false);
        loadData();
      }
    } catch (error) {
      console.error('Error updating task:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.updateError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('task.updateError'),
        });
      }
    } finally {
      setIsSaving(false);
    }
  };

  // Task durumunu değiştir
  const handleChangeState = async (newState: TaskState) => {
    if (!taskId) return;

    try {
      const response = await taskService.changeState(taskId, { state: newState });

      if (response.success && response.data) {
        toast.success(t('common.success'), {
          description: t('task.stateChangeSuccess'),
        });
        loadData();
      }
    } catch (error) {
      console.error('Error changing state:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.stateChangeError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('task.stateChangeError'),
        });
      }
    }
  };

  // Task'a kullanıcı ata
  const handleAssign = async () => {
    if (!taskId || !assigneeId) {
      toast.error(t('common.error'), {
        description: t('task.assigneeRequired'),
      });
      return;
    }

    try {
      setIsAssigning(true);
      const response = await taskService.assign(taskId, { assigneeId });

      if (response.success && response.data) {
        toast.success(t('common.success'), {
          description: t('task.assignSuccess'),
        });
        loadData();
      }
    } catch (error) {
      console.error('Error assigning task:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.assignError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('task.assignError'),
        });
      }
    } finally {
      setIsAssigning(false);
    }
  };

  // Task sil
  const handleDeleteTask = async () => {
    if (!taskId) return;

    try {
      setIsDeleting(true);
      const response = await taskService.delete(taskId);

      if (response.success) {
        toast.success(t('common.success'), {
          description: t('task.deleteSuccess'),
        });
        navigate(`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`);
      }
    } catch (error) {
      console.error('Error deleting task:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.deleteError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('task.deleteError'),
        });
      }
    } finally {
      setIsDeleting(false);
      setIsDeleteDialogOpen(false);
    }
  };

  // Task ilişkisi ekle
  const handleAddRelation = async () => {
    if (!taskId || !relationFormData.targetTaskId) {
      toast.error(t('common.error'), {
        description: t('task.relationTargetRequired'),
      });
      return;
    }

    try {
      setIsAddingRelation(true);
      const response = await taskService.createRelation(taskId, relationFormData);

      if (response.success && response.data) {
        toast.success(t('common.success'), {
          description: t('task.relationAddSuccess'),
        });
        setIsAddRelationModalOpen(false);
        setRelationFormData({
          targetTaskId: '',
          relationType: TaskRelationType.RelatesTo,
        });
        loadRelations();
      }
    } catch (error) {
      console.error('Error adding relation:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.relationAddError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('task.relationAddError'),
        });
      }
    } finally {
      setIsAddingRelation(false);
    }
  };

  // Task ilişkisini sil
  const handleDeleteRelation = async (relationId: string) => {
    if (!taskId) return;

    try {
      const response = await taskService.deleteRelation(taskId, relationId);

      if (response.success) {
        toast.success(t('common.success'), {
          description: t('task.relationDeleteSuccess'),
        });
        loadRelations();
      }
    } catch (error) {
      console.error('Error deleting relation:', error);
      if (error instanceof ApiError) {
        toast.error(t('common.error'), {
          description: error.message || t('task.relationDeleteError'),
        });
      } else {
        toast.error(t('common.error'), {
          description: t('task.relationDeleteError'),
        });
      }
    }
  };

  // Overdue kontrolü
  const isOverdue = task?.dueDate
    ? new Date(task.dueDate) < new Date() && task.state !== TaskState.Completed
    : false;

  // Permission kontrolleri
  const canUpdate = hasPermission('TaskUpdate');
  const canDelete = hasPermission('TaskDelete');
  const canAssign = hasPermission('TaskAssign');
  const canChangeState = hasPermission('TaskStateChange');
  const canCreateRelation = hasPermission('TaskRelationCreate');
  const canDeleteRelation = hasPermission('TaskRelationDelete');

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#161B22] to-[#0D1117] flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-[#9CA3AF]">{t('common.loading')}</p>
        </div>
      </div>
    );
  }

  if (!task || !project || !module || !useCase) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#161B22] to-[#0D1117] relative overflow-hidden">
      {/* Animated Background Gradients */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <motion.div
          className="absolute top-0 left-1/4 w-96 h-96 bg-[#8B5CF6]/20 rounded-full blur-3xl"
          animate={{
            x: [0, 100, 0],
            y: [0, -50, 0],
            scale: [1, 1.2, 1],
          }}
          transition={{
            duration: 20,
            repeat: Infinity,
            ease: 'easeInOut',
          }}
        />
        <motion.div
          className="absolute bottom-0 right-1/4 w-96 h-96 bg-[#2DD4BF]/20 rounded-full blur-3xl"
          animate={{
            x: [0, -100, 0],
            y: [0, 50, 0],
            scale: [1, 1.2, 1],
          }}
          transition={{
            duration: 25,
            repeat: Infinity,
            ease: 'easeInOut',
          }}
        />
      </div>

      <div className="container mx-auto px-6 py-24 relative z-10">
        {/* Breadcrumb */}
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="mb-6"
        >
          <Breadcrumb>
            <BreadcrumbList>
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link
                    to="/projects"
                    className="text-[#9CA3AF] hover:text-[#8B5CF6] transition-colors"
                  >
                    {t('nav.projects')}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link
                    to={`/projects/${projectId}`}
                    className="text-[#9CA3AF] hover:text-[#8B5CF6] transition-colors"
                  >
                    {project.title}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link
                    to={`/projects/${projectId}/modules/${moduleId}`}
                    className="text-[#9CA3AF] hover:text-[#8B5CF6] transition-colors"
                  >
                    {module.title}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link
                    to={`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`}
                    className="text-[#9CA3AF] hover:text-[#8B5CF6] transition-colors"
                  >
                    {useCase.title}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <span className="text-[#E5E7EB]">{task.title}</span>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </motion.div>

        {/* Back Button */}
        <motion.div
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.5, delay: 0.1 }}
          className="mb-6"
        >
          <Button
            variant="ghost"
            onClick={() =>
              navigate(
                `/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}`
              )
            }
            className="text-[#9CA3AF] hover:text-[#E5E7EB] hover:bg-[#8B5CF6]/10"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            {t('task.back')}
          </Button>
        </motion.div>

        {/* Task Info Card */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.2 }}
          className="bg-[#161B22] border border-[#30363D] rounded-2xl p-8 mb-8 relative overflow-hidden"
        >
          <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#8B5CF6] via-[#EC4899] to-[#2DD4BF]" />
          
          <div className="flex items-start justify-between mb-6">
            <div className="flex-1">
              <div className="flex items-center gap-3 mb-4">
                <CheckSquare className="w-8 h-8 text-[#8B5CF6]" />
                <h1 className="text-3xl font-bold text-[#E5E7EB]">{task.title}</h1>
              </div>
              
              <div className="flex items-center gap-3 mb-4">
                <Badge className={taskTypeColors[task.type]}>
                  {task.type}
                </Badge>
                <Badge className={taskStateColors[task.state]}>
                  {task.state}
                </Badge>
              </div>

              {task.description && (
                <p className="text-[#9CA3AF] mb-4">{task.description}</p>
              )}

              {task.importantNotes && (
                <div className="bg-[#F97316]/10 border border-[#F97316]/30 rounded-lg p-4 mb-4">
                  <div className="flex items-start gap-2">
                    <AlertTriangle className="w-5 h-5 text-[#F97316] mt-0.5 flex-shrink-0" />
                    <div>
                      <p className="text-sm font-semibold text-[#F97316] mb-1">
                        {t('task.importantNotes')}
                      </p>
                      <p className="text-sm text-[#E5E7EB]">{task.importantNotes}</p>
                    </div>
                  </div>
                </div>
              )}

              {/* Assignee Section */}
              <div className="flex items-center gap-4 mb-4">
                <div className="flex items-center gap-2">
                  <User className="w-5 h-5 text-[#9CA3AF]" />
                  <span className="text-[#9CA3AF]">{t('task.assignee')}:</span>
                  {task.assignee ? (
                    <span className="text-[#E5E7EB]">
                      {task.assignee.firstName} {task.assignee.lastName}
                    </span>
                  ) : (
                    <span className="text-[#6B7280]">{t('task.unassigned')}</span>
                  )}
                </div>
                {canAssign && (
                  <Select value={assigneeId} onValueChange={setAssigneeId}>
                    <SelectTrigger className="w-[200px] bg-[#21262D] border-[#30363D] text-[#E5E7EB]">
                      <SelectValue placeholder={t('task.selectAssignee')} />
                    </SelectTrigger>
                    <SelectContent className="bg-[#161B22] border-[#30363D]">
                      <SelectItem value="">{t('task.unassigned')}</SelectItem>
                      {/* TODO: User listesi API'den gelecek */}
                    </SelectContent>
                  </Select>
                )}
                {canAssign && assigneeId && (
                  <Button
                    size="sm"
                    onClick={handleAssign}
                    disabled={isAssigning}
                    className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
                  >
                    {isAssigning ? t('common.saving') : t('task.assign')}
                  </Button>
                )}
              </div>

              {/* Dates Section */}
              <div className="flex items-center gap-6 mb-4">
                {task.startedDate && (
                  <div className="flex items-center gap-2">
                    <Calendar className="w-5 h-5 text-[#9CA3AF]" />
                    <span className="text-[#9CA3AF]">{t('task.started')}:</span>
                    <span className="text-[#E5E7EB]">
                      {new Date(task.startedDate).toLocaleDateString('tr-TR')}
                    </span>
                  </div>
                )}
                {task.dueDate && (
                  <div className={cn(
                    "flex items-center gap-2",
                    isOverdue && "text-[#EF4444]"
                  )}>
                    <Calendar className={cn(
                      "w-5 h-5",
                      isOverdue ? "text-[#EF4444]" : "text-[#9CA3AF]"
                    )} />
                    <span className={isOverdue ? "text-[#EF4444]" : "text-[#9CA3AF]"}>
                      {t('task.due')}:
                    </span>
                    <span className={isOverdue ? "text-[#EF4444]" : "text-[#E5E7EB]"}>
                      {new Date(task.dueDate).toLocaleDateString('tr-TR')}
                    </span>
                    {isOverdue && (
                      <AlertTriangle className="w-4 h-4 text-[#EF4444] ml-1" />
                    )}
                  </div>
                )}
              </div>

              {/* State Actions */}
              {canChangeState && (
                <div className="flex items-center gap-2 flex-wrap">
                  {task.state === TaskState.NotStarted && (
                    <Button
                      size="sm"
                      onClick={() => handleChangeState(TaskState.InProgress)}
                      className="bg-[#3B82F6] text-white hover:bg-[#3B82F6]/90"
                    >
                      <Play className="w-4 h-4 mr-2" />
                      {t('task.start')}
                    </Button>
                  )}
                  {task.state === TaskState.InProgress && (
                    <>
                      <Button
                        size="sm"
                        onClick={() => handleChangeState(TaskState.Completed)}
                        className="bg-[#10B981] text-white hover:bg-[#10B981]/90"
                      >
                        <CheckCircle className="w-4 h-4 mr-2" />
                        {t('task.complete')}
                      </Button>
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => handleChangeState(TaskState.Cancelled)}
                        className="border-[#EF4444] text-[#EF4444] hover:bg-[#EF4444]/10"
                      >
                        <XCircle className="w-4 h-4 mr-2" />
                        {t('task.cancel')}
                      </Button>
                    </>
                  )}
                  {(task.state === TaskState.InProgress || task.state === TaskState.Cancelled) && (
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => handleChangeState(TaskState.NotStarted)}
                      className="border-[#6B7280] text-[#6B7280] hover:bg-[#6B7280]/10"
                    >
                      <RotateCcw className="w-4 h-4 mr-2" />
                      {t('task.revert')}
                    </Button>
                  )}
                </div>
              )}
            </div>

            {/* Action Buttons */}
            <div className="flex gap-2">
              {canUpdate && (
                <Button
                  variant="outline"
                  onClick={() => setIsEditModalOpen(true)}
                  className="border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
                >
                  <Edit className="w-4 h-4 mr-2" />
                  {t('common.edit')}
                </Button>
              )}
              {canDelete && (
                <Button
                  variant="outline"
                  onClick={() => setIsDeleteDialogOpen(true)}
                  className="border-[#EF4444] text-[#EF4444] hover:bg-[#EF4444]/10"
                >
                  <Trash2 className="w-4 h-4 mr-2" />
                  {t('common.delete')}
                </Button>
              )}
            </div>
          </div>
        </motion.div>

        {/* Task Relations Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.3 }}
          className="bg-[#161B22] border border-[#30363D] rounded-2xl p-8"
        >
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <LinkIcon className="w-6 h-6 text-[#8B5CF6]" />
              <h2 className="text-2xl font-bold text-[#E5E7EB]">{t('task.relations')}</h2>
            </div>
            {canCreateRelation && (
              <Button
                onClick={() => setIsAddRelationModalOpen(true)}
                className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
              >
                <Plus className="w-4 h-4 mr-2" />
                {t('task.addRelation')}
              </Button>
            )}
          </div>

          {relationsLoading ? (
            <div className="text-center py-8">
              <div className="w-6 h-6 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin mx-auto mb-2"></div>
              <p className="text-[#9CA3AF] text-sm">{t('common.loading')}</p>
            </div>
          ) : relations.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-[#9CA3AF]">{t('task.noRelations')}</p>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow className="border-[#30363D] hover:bg-[#21262D]">
                  <TableHead className="text-[#E5E7EB]">{t('task.relationType')}</TableHead>
                  <TableHead className="text-[#E5E7EB]">{t('task.relatedTask')}</TableHead>
                  <TableHead className="text-[#E5E7EB] text-right">{t('common.actions')}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {relations.map((relation) => (
                  <TableRow
                    key={relation.id}
                    className="border-[#30363D] hover:bg-[#21262D]"
                  >
                    <TableCell>
                      <Badge className={relationTypeColors[relation.relationType]}>
                        {relation.relationType}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      {relation.targetTask ? (
                        <Link
                          to={`/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${relation.targetTaskId}`}
                          className="text-[#8B5CF6] hover:text-[#8B5CF6]/80 hover:underline"
                        >
                          {relation.targetTask.title}
                        </Link>
                      ) : (
                        <span className="text-[#6B7280]">{t('task.taskNotFound')}</span>
                      )}
                    </TableCell>
                    <TableCell className="text-right">
                      <div className="flex items-center justify-end gap-2">
                        {relation.targetTask && (
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() =>
                              navigate(
                                `/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${relation.targetTaskId}`
                              )
                            }
                            className="text-[#8B5CF6] hover:text-[#8B5CF6] hover:bg-[#8B5CF6]/10"
                          >
                            <Eye className="w-4 h-4 mr-2" />
                            {t('common.view')}
                          </Button>
                        )}
                        {canDeleteRelation && (
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleDeleteRelation(relation.id)}
                            className="text-[#EF4444] hover:text-[#EF4444] hover:bg-[#EF4444]/10"
                          >
                            <Trash2 className="w-4 h-4 mr-2" />
                            {t('common.delete')}
                          </Button>
                        )}
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </motion.div>
      </div>

      {/* Edit Task Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] max-w-2xl">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              {t('task.editModal.title')}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="title" className="text-[#E5E7EB]">
                {t('task.title')} <span className="text-[#EF4444]">*</span>
              </Label>
              <Input
                id="title"
                value={editFormData.title || ''}
                onChange={(e) =>
                  setEditFormData({ ...editFormData, title: e.target.value })
                }
                placeholder={t('task.title')}
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                maxLength={200}
              />
              <p className="text-xs text-[#9CA3AF] mt-1">
                {(editFormData.title?.length || 0)}/200 {t('common.characters')}
              </p>
            </div>

            <div>
              <Label htmlFor="type" className="text-[#E5E7EB]">
                {t('task.type')} <span className="text-[#EF4444]">*</span>
              </Label>
              <Select
                value={editFormData.type || TaskType.Feature}
                onValueChange={(value) =>
                  setEditFormData({ ...editFormData, type: value as TaskType })
                }
              >
                <SelectTrigger className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent className="bg-[#161B22] border-[#30363D]">
                  {Object.values(TaskType).map((type) => (
                    <SelectItem key={type} value={type} className="text-[#E5E7EB]">
                      {type}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label htmlFor="description" className="text-[#E5E7EB]">
                {t('task.description')}
              </Label>
              <Textarea
                id="description"
                value={editFormData.description || ''}
                onChange={(e) =>
                  setEditFormData({ ...editFormData, description: e.target.value })
                }
                placeholder={t('task.description')}
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={4}
                maxLength={2000}
              />
              <p className="text-xs text-[#9CA3AF] mt-1">
                {(editFormData.description?.length || 0)}/2000 {t('common.characters')}
              </p>
            </div>

            <div>
              <Label htmlFor="importantNotes" className="text-[#E5E7EB]">
                {t('task.importantNotes')}
              </Label>
              <Textarea
                id="importantNotes"
                value={editFormData.importantNotes || ''}
                onChange={(e) =>
                  setEditFormData({ ...editFormData, importantNotes: e.target.value })
                }
                placeholder={t('task.importantNotes')}
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={3}
                maxLength={1000}
              />
              <p className="text-xs text-[#9CA3AF] mt-1">
                {(editFormData.importantNotes?.length || 0)}/1000 {t('common.characters')}
              </p>
            </div>

            <div>
              <Label htmlFor="dueDate" className="text-[#E5E7EB]">
                {t('task.dueDate')}
              </Label>
              <Input
                id="dueDate"
                type="date"
                value={editFormData.dueDate || ''}
                onChange={(e) =>
                  setEditFormData({ ...editFormData, dueDate: e.target.value })
                }
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsEditModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              {t('common.cancel')}
            </Button>
            <Button
              onClick={handleUpdateTask}
              disabled={isSaving || !editFormData.title?.trim()}
              className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
            >
              {isSaving ? t('common.saving') : t('common.save')}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Task Dialog */}
      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-xl font-bold text-[#E5E7EB]">
              {t('task.deleteModal.title')}
            </AlertDialogTitle>
            <AlertDialogDescription className="text-[#9CA3AF]">
              {t('task.deleteModal.description')}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]">
              {t('common.cancel')}
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteTask}
              disabled={isDeleting}
              className="bg-[#EF4444] text-white hover:bg-[#EF4444]/90"
            >
              {isDeleting ? t('common.deleting') : t('common.delete')}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Add Relation Modal */}
      <Dialog open={isAddRelationModalOpen} onOpenChange={setIsAddRelationModalOpen}>
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              {t('task.addRelationModal.title')}
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="targetTask" className="text-[#E5E7EB]">
                {t('task.relatedTask')} <span className="text-[#EF4444]">*</span>
              </Label>
              <Input
                id="targetTask"
                value={relationFormData.targetTaskId}
                onChange={(e) =>
                  setRelationFormData({
                    ...relationFormData,
                    targetTaskId: e.target.value,
                  })
                }
                placeholder={t('task.relatedTaskPlaceholder')}
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
              />
              <p className="text-xs text-[#9CA3AF] mt-1">
                {t('task.relatedTaskHint')}
              </p>
            </div>

            <div>
              <Label className="text-[#E5E7EB] mb-3 block">
                {t('task.relationType')} <span className="text-[#EF4444]">*</span>
              </Label>
              <RadioGroup
                value={relationFormData.relationType}
                onValueChange={(value) =>
                  setRelationFormData({
                    ...relationFormData,
                    relationType: value as TaskRelationType,
                  })
                }
                className="space-y-3"
              >
                {Object.values(TaskRelationType).map((type) => (
                  <div key={type} className="flex items-center space-x-2">
                    <RadioGroupItem
                      value={type}
                      id={type}
                      className="border-[#30363D] text-[#8B5CF6]"
                    />
                    <Label
                      htmlFor={type}
                      className="text-[#E5E7EB] cursor-pointer flex items-center gap-2"
                    >
                      <Badge className={relationTypeColors[type]}>{type}</Badge>
                    </Label>
                  </div>
                ))}
              </RadioGroup>
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsAddRelationModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              {t('common.cancel')}
            </Button>
            <Button
              onClick={handleAddRelation}
              disabled={isAddingRelation || !relationFormData.targetTaskId}
              className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
            >
              {isAddingRelation ? t('common.creating') : t('task.addRelation')}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

