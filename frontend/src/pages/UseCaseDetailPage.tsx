/**
 * UseCase Detail Page
 * Use case detayları ve task listesi (List View)
 */

import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'motion/react';
import {
  ArrowLeft,
  Edit,
  Trash2,
  Archive,
  ArchiveRestore,
  Plus,
  Search,
  Filter,
  Eye,
  Folder,
  Package,
  FileText,
  CheckSquare,
  Calendar,
  User,
  AlertTriangle,
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
import { useCaseService } from '../services/useCaseService';
import { projectService } from '../services/projectService';
import { moduleService } from '../services/moduleService';
import { hasPermission } from '../utils/permissions';
import {
  UseCase,
  Project,
  Module,
  Task,
  TaskType,
  TaskState,
  CreateTaskRequest,
  UpdateUseCaseRequest,
} from '../types';
import { toast } from 'sonner';
import { ApiError } from '../services/api';

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

export const UseCaseDetailPage: React.FC = () => {
  const { projectId, moduleId, useCaseId } = useParams<{
    projectId: string;
    moduleId: string;
    useCaseId: string;
  }>();
  const navigate = useNavigate();
  const [project, setProject] = useState<Project | null>(null);
  const [module, setModule] = useState<Module | null>(null);
  const [useCase, setUseCase] = useState<UseCase | null>(null);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [tasksLoading, setTasksLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [stateFilter, setStateFilter] = useState<TaskState | null>(null);
  const [typeFilter, setTypeFilter] = useState<TaskType | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [isCreateTaskModalOpen, setIsCreateTaskModalOpen] = useState(false);
  const [editFormData, setEditFormData] = useState<UpdateUseCaseRequest>({
    title: '',
    description: '',
    importantNotes: '',
  });
  const [createTaskFormData, setCreateTaskFormData] =
    useState<CreateTaskRequest>({
      title: '',
      description: '',
      importantNotes: '',
      type: TaskType.Feature,
      dueDate: '',
    });
  const [isSaving, setIsSaving] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isCreatingTask, setIsCreatingTask] = useState(false);

  const pageSize = 20;

  // Proje, modül ve use case detaylarını yükle
  const loadData = async () => {
    if (!projectId || !moduleId || !useCaseId) return;

    try {
      setLoading(true);
      const [projectResponse, moduleResponse, useCaseResponse] =
        await Promise.all([
          projectService.getById(projectId),
          moduleService.getById(moduleId),
          useCaseService.getById(useCaseId),
        ]);

      if (projectResponse.success && projectResponse.data) {
        setProject(projectResponse.data);
      }

      if (moduleResponse.success && moduleResponse.data) {
        setModule(moduleResponse.data);
      }

      if (useCaseResponse.success && useCaseResponse.data) {
        setUseCase(useCaseResponse.data);
        setEditFormData({
          title: useCaseResponse.data.title,
          description: useCaseResponse.data.description || '',
          importantNotes: useCaseResponse.data.importantNotes || '',
        });
      }
    } catch (error) {
      console.error('Error loading data:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Veri yüklenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Veri yüklenirken bir hata oluştu.',
        });
      }
      navigate(`/projects/${projectId}/modules/${moduleId}`);
    } finally {
      setLoading(false);
    }
  };

  // Task'ları yükle
  const loadTasks = async () => {
    if (!useCaseId) return;

    try {
      setTasksLoading(true);
      const response = await useCaseService.getTasks(useCaseId, {
        page,
        pageSize,
        state: stateFilter,
        type: typeFilter,
        search: searchTerm || undefined,
      });

      if (response.success && response.data) {
        setTasks(response.data.items);
        setTotalPages(response.data.totalPages);
      }
    } catch (error) {
      console.error('Error loading tasks:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Task\'lar yüklenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Task\'lar yüklenirken bir hata oluştu.',
        });
      }
    } finally {
      setTasksLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [projectId, moduleId, useCaseId]);

  useEffect(() => {
    if (useCase) {
      loadTasks();
    }
  }, [useCase, page, stateFilter, typeFilter, searchTerm]);

  // Use case güncelle
  const handleUpdateUseCase = async () => {
    if (!useCaseId || !editFormData.title?.trim()) {
      toast.error('Hata', {
        description: 'Use case başlığı zorunludur.',
      });
      return;
    }

    try {
      setIsSaving(true);
      const response = await useCaseService.update(useCaseId, editFormData);

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: 'Use case başarıyla güncellendi.',
        });
        setIsEditModalOpen(false);
        loadData();
      }
    } catch (error) {
      console.error('Error updating use case:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Use case güncellenirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Use case güncellenirken bir hata oluştu.',
        });
      }
    } finally {
      setIsSaving(false);
    }
  };

  // Use case durumunu değiştir
  const handleChangeStatus = async () => {
    if (!useCaseId || !useCase) return;

    try {
      const response = await useCaseService.changeStatus(useCaseId, {
        isActive: !useCase.isActive,
      });

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: `Use case ${useCase.isActive ? 'arşivlendi' : 'aktifleştirildi'}.`,
        });
        loadData();
      }
    } catch (error) {
      console.error('Error changing status:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Durum değiştirilirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Durum değiştirilirken bir hata oluştu.',
        });
      }
    }
  };

  // Use case sil
  const handleDeleteUseCase = async () => {
    if (!useCaseId) return;

    try {
      setIsDeleting(true);
      const response = await useCaseService.delete(useCaseId);

      if (response.success) {
        toast.success('Başarılı', {
          description: 'Use case başarıyla silindi.',
        });
        navigate(`/projects/${projectId}/modules/${moduleId}`);
      }
    } catch (error) {
      console.error('Error deleting use case:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Use case silinirken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Use case silinirken bir hata oluştu.',
        });
      }
    } finally {
      setIsDeleting(false);
      setIsDeleteDialogOpen(false);
    }
  };

  // Task oluştur
  const handleCreateTask = async () => {
    if (!useCaseId || !createTaskFormData.title.trim()) {
      toast.error('Hata', {
        description: 'Task başlığı zorunludur.',
      });
      return;
    }

    try {
      setIsCreatingTask(true);
      const response = await useCaseService.createTask(
        useCaseId,
        createTaskFormData
      );

      if (response.success && response.data) {
        toast.success('Başarılı', {
          description: 'Task başarıyla oluşturuldu.',
        });
        setIsCreateTaskModalOpen(false);
        setCreateTaskFormData({
          title: '',
          description: '',
          importantNotes: '',
          type: TaskType.Feature,
          dueDate: '',
        });
        loadTasks();
      }
    } catch (error) {
      console.error('Error creating task:', error);
      if (error instanceof ApiError) {
        toast.error('Hata', {
          description: error.message || 'Task oluşturulurken bir hata oluştu.',
        });
      } else {
        toast.error('Hata', {
          description: 'Task oluşturulurken bir hata oluştu.',
        });
      }
    } finally {
      setIsCreatingTask(false);
    }
  };

  const canUpdate = hasPermission('UseCaseUpdate');
  const canDelete = hasPermission('UseCaseDelete');
  const canCreateTask = hasPermission('TaskCreate');

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#161B22] to-[#0D1117] flex items-center justify-center">
        <div className="text-center">
          <div className="w-8 h-8 border-2 border-[#8B5CF6] border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
          <p className="text-[#9CA3AF]">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  if (!useCase || !module || !project) {
    return null;
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#0D1117] via-[#161B22] to-[#0D1117] relative overflow-hidden">
      {/* Animated Background Gradients */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <motion.div
          className="absolute top-0 left-1/4 w-96 h-96 bg-[#EC4899]/20 rounded-full blur-3xl"
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
          className="absolute bottom-0 right-1/4 w-96 h-96 bg-[#8B5CF6]/20 rounded-full blur-3xl"
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
          initial={{ y: -20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5 }}
          className="mb-6"
        >
          <Breadcrumb>
            <BreadcrumbList className="text-[#9CA3AF]">
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link to="/projects" className="hover:text-[#8B5CF6]">
                    Projects
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbLink asChild>
                  <Link
                    to={`/projects/${projectId}`}
                    className="hover:text-[#8B5CF6]"
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
                    className="hover:text-[#8B5CF6]"
                  >
                    {module.title}
                  </Link>
                </BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem className="text-[#E5E7EB]">
                {useCase.title}
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
        </motion.div>

        {/* Back Button */}
        <motion.div
          initial={{ y: -20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.1 }}
          className="mb-6"
        >
          <Button
            variant="ghost"
            onClick={() => navigate(`/projects/${projectId}/modules/${moduleId}`)}
            className="text-[#E5E7EB] hover:bg-[#EC4899]/10 border border-[#30363D]/50"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Back to Module
          </Button>
        </motion.div>

        {/* Use Case Info Card */}
        <motion.div
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.2 }}
          className="bg-[#161B22] border border-[#30363D] rounded-xl p-6 mb-8"
        >
          <div className="flex items-start justify-between mb-4">
            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2">
                <FileText className="w-8 h-8 text-[#EC4899]" />
                <h1 className="text-3xl font-bold text-[#E5E7EB]">
                  {useCase.title}
                </h1>
                <Badge
                  variant={useCase.isActive ? 'default' : 'secondary'}
                  className={
                    useCase.isActive
                      ? 'bg-[#10B981]/10 text-[#10B981] border-[#10B981]/30'
                      : 'bg-[#6B7280]/10 text-[#6B7280] border-[#6B7280]/30'
                  }
                >
                  {useCase.isActive ? 'Active' : 'Archived'}
                </Badge>
              </div>
              {useCase.description && (
                <p className="text-[#9CA3AF] mb-4">{useCase.description}</p>
              )}
              {useCase.importantNotes && (
                <div className="bg-[#F97316]/10 border border-[#F97316]/30 rounded-lg p-4 mb-4">
                  <div className="flex items-start gap-2">
                    <AlertTriangle className="w-5 h-5 text-[#F97316] mt-0.5 flex-shrink-0" />
                    <div>
                      <p className="text-sm font-semibold text-[#F97316] mb-1">
                        Important Notes
                      </p>
                      <p className="text-sm text-[#E5E7EB]">
                        {useCase.importantNotes}
                      </p>
                    </div>
                  </div>
                </div>
              )}
            </div>
            <div className="flex gap-2">
              {canUpdate && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setIsEditModalOpen(true)}
                  className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
                >
                  <Edit className="w-4 h-4 mr-2" />
                  Edit
                </Button>
              )}
              <Button
                variant="outline"
                size="sm"
                onClick={handleChangeStatus}
                className={
                  useCase.isActive
                    ? 'bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]'
                    : 'bg-[#10B981]/10 border-[#10B981]/30 text-[#10B981] hover:bg-[#10B981]/20'
                }
              >
                {useCase.isActive ? (
                  <>
                    <Archive className="w-4 h-4 mr-2" />
                    Archive
                  </>
                ) : (
                  <>
                    <ArchiveRestore className="w-4 h-4 mr-2" />
                    Activate
                  </>
                )}
              </Button>
              {canDelete && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => setIsDeleteDialogOpen(true)}
                  className="bg-[#EF4444]/10 border-[#EF4444]/30 text-[#EF4444] hover:bg-[#EF4444]/20"
                >
                  <Trash2 className="w-4 h-4 mr-2" />
                  Delete
                </Button>
              )}
            </div>
          </div>
        </motion.div>

        {/* Tasks Section */}
        <motion.div
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.5, delay: 0.3 }}
        >
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="text-2xl font-bold text-[#E5E7EB] mb-2 flex items-center gap-2">
                <CheckSquare className="w-6 h-6 text-[#8B5CF6]" />
                Tasks
              </h2>
              <p className="text-[#9CA3AF]">
                Manage tasks for this use case
              </p>
            </div>
            {canCreateTask && (
              <Button
                onClick={() => setIsCreateTaskModalOpen(true)}
                className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90 shadow-lg shadow-[#8B5CF6]/25"
              >
                <Plus className="w-4 h-4 mr-2" />
                Create Task
              </Button>
            )}
          </div>

          {/* Filters */}
          <div className="mb-6 flex flex-col sm:flex-row gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-[#9CA3AF]" />
              <Input
                placeholder="Search tasks..."
                value={searchTerm}
                onChange={(e) => {
                  setSearchTerm(e.target.value);
                  setPage(1);
                }}
                className="pl-10 bg-[#161B22] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
              />
            </div>
            <Select
              value={stateFilter || 'all'}
              onValueChange={(value) => {
                setStateFilter(value === 'all' ? null : (value as TaskState));
                setPage(1);
              }}
            >
              <SelectTrigger className="w-full sm:w-[180px] bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                <Filter className="w-4 h-4 mr-2" />
                <SelectValue placeholder="Filter by state" />
              </SelectTrigger>
              <SelectContent className="bg-[#161B22] border-[#30363D]">
                <SelectItem value="all">All States</SelectItem>
                <SelectItem value={TaskState.NotStarted}>Not Started</SelectItem>
                <SelectItem value={TaskState.InProgress}>In Progress</SelectItem>
                <SelectItem value={TaskState.Completed}>Completed</SelectItem>
                <SelectItem value={TaskState.Cancelled}>Cancelled</SelectItem>
              </SelectContent>
            </Select>
            <Select
              value={typeFilter || 'all'}
              onValueChange={(value) => {
                setTypeFilter(value === 'all' ? null : (value as TaskType));
                setPage(1);
              }}
            >
              <SelectTrigger className="w-full sm:w-[180px] bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
                <Filter className="w-4 h-4 mr-2" />
                <SelectValue placeholder="Filter by type" />
              </SelectTrigger>
              <SelectContent className="bg-[#161B22] border-[#30363D]">
                <SelectItem value="all">All Types</SelectItem>
                <SelectItem value={TaskType.Documentation}>Documentation</SelectItem>
                <SelectItem value={TaskType.Feature}>Feature</SelectItem>
                <SelectItem value={TaskType.Test}>Test</SelectItem>
                <SelectItem value={TaskType.Bug}>Bug</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Tasks Table */}
          {tasksLoading ? (
            <div className="bg-[#161B22] border border-[#30363D] rounded-xl p-6">
              <div className="space-y-4">
                {[...Array(5)].map((_, i) => (
                  <div
                    key={i}
                    className="h-16 bg-[#21262D] rounded animate-pulse"
                  ></div>
                ))}
              </div>
            </div>
          ) : tasks.length === 0 ? (
            <div className="text-center py-16 bg-[#161B22] border border-[#30363D] rounded-xl">
              <CheckSquare className="w-16 h-16 text-[#6B7280] mx-auto mb-4" />
              <p className="text-[#9CA3AF] text-lg">
                {searchTerm || stateFilter || typeFilter
                  ? 'No tasks found matching your filters.'
                  : 'No tasks yet. Create your first task!'}
              </p>
            </div>
          ) : (
            <div className="bg-[#161B22] border border-[#30363D] rounded-xl overflow-hidden">
              <Table>
                <TableHeader>
                  <TableRow className="border-[#30363D] hover:bg-[#21262D]">
                    <TableHead className="text-[#E5E7EB]">Title</TableHead>
                    <TableHead className="text-[#E5E7EB]">Type</TableHead>
                    <TableHead className="text-[#E5E7EB]">State</TableHead>
                    <TableHead className="text-[#E5E7EB]">Assignee</TableHead>
                    <TableHead className="text-[#E5E7EB]">Due Date</TableHead>
                    <TableHead className="text-[#E5E7EB] text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {tasks.map((task, index) => {
                    const isOverdue =
                      task.dueDate &&
                      new Date(task.dueDate) < new Date() &&
                      task.state !== TaskState.Completed &&
                      task.state !== TaskState.Cancelled;

                    return (
                      <motion.tr
                        key={task.id}
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ duration: 0.3, delay: index * 0.05 }}
                        className="border-[#30363D] hover:bg-[#21262D] cursor-pointer transition-colors"
                        onClick={() =>
                          navigate(
                            `/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${task.id}`
                          )
                        }
                      >
                        <TableCell className="text-[#E5E7EB] font-medium">
                          {task.title}
                        </TableCell>
                        <TableCell>
                          <Badge
                            className={taskTypeColors[task.type]}
                          >
                            {task.type}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <Badge className={taskStateColors[task.state]}>
                            {task.state}
                          </Badge>
                        </TableCell>
                        <TableCell className="text-[#9CA3AF]">
                          {task.assignee ? (
                            <div className="flex items-center gap-2">
                              <User className="w-4 h-4" />
                              {task.assignee.firstName} {task.assignee.lastName}
                            </div>
                          ) : (
                            <span className="text-[#6B7280]">Unassigned</span>
                          )}
                        </TableCell>
                        <TableCell>
                          {task.dueDate ? (
                            <div
                              className={`flex items-center gap-2 ${
                                isOverdue ? 'text-[#EF4444]' : 'text-[#9CA3AF]'
                              }`}
                            >
                              <Calendar className="w-4 h-4" />
                              {new Date(task.dueDate).toLocaleDateString('tr-TR')}
                              {isOverdue && (
                                <AlertTriangle className="w-4 h-4 text-[#EF4444]" />
                              )}
                            </div>
                          ) : (
                            <span className="text-[#6B7280]">No due date</span>
                          )}
                        </TableCell>
                        <TableCell className="text-right">
                          <Button
                            variant="ghost"
                            size="sm"
                            className="text-[#8B5CF6] hover:text-[#8B5CF6] hover:bg-[#8B5CF6]/10"
                            onClick={(e) => {
                              e.stopPropagation();
                              navigate(
                                `/projects/${projectId}/modules/${moduleId}/usecases/${useCaseId}/tasks/${task.id}`
                              );
                            }}
                          >
                            <Eye className="w-4 h-4 mr-2" />
                            View
                          </Button>
                        </TableCell>
                      </motion.tr>
                    );
                  })}
                </TableBody>
              </Table>
            </div>
          )}

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-8 flex justify-center gap-2">
              <Button
                variant="outline"
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
              >
                Previous
              </Button>
              <div className="flex items-center gap-2">
                {[...Array(totalPages)].map((_, i) => {
                  const pageNum = i + 1;
                  if (
                    pageNum === 1 ||
                    pageNum === totalPages ||
                    (pageNum >= page - 1 && pageNum <= page + 1)
                  ) {
                    return (
                      <Button
                        key={pageNum}
                        variant={page === pageNum ? 'default' : 'outline'}
                        onClick={() => setPage(pageNum)}
                        className={
                          page === pageNum
                            ? 'bg-[#8B5CF6] text-white'
                            : 'bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]'
                        }
                      >
                        {pageNum}
                      </Button>
                    );
                  } else if (pageNum === page - 2 || pageNum === page + 2) {
                    return <span key={pageNum} className="text-[#9CA3AF]">...</span>;
                  }
                  return null;
                })}
              </div>
              <Button
                variant="outline"
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
                className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] hover:bg-[#21262D]"
              >
                Next
              </Button>
            </div>
          )}
        </motion.div>
      </div>

      {/* Edit Use Case Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              Edit Use Case
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="edit-title" className="text-[#E5E7EB]">
                Title <span className="text-[#EF4444]">*</span>
              </Label>
              <Input
                id="edit-title"
                value={editFormData.title}
                onChange={(e) =>
                  setEditFormData({ ...editFormData, title: e.target.value })
                }
                placeholder="Enter use case title"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                maxLength={100}
              />
            </div>
            <div>
              <Label htmlFor="edit-description" className="text-[#E5E7EB]">
                Description
              </Label>
              <Textarea
                id="edit-description"
                value={editFormData.description}
                onChange={(e) =>
                  setEditFormData({
                    ...editFormData,
                    description: e.target.value,
                  })
                }
                placeholder="Enter use case description"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={4}
                maxLength={1000}
              />
            </div>
            <div>
              <Label htmlFor="edit-notes" className="text-[#E5E7EB]">
                Important Notes
              </Label>
              <Textarea
                id="edit-notes"
                value={editFormData.importantNotes}
                onChange={(e) =>
                  setEditFormData({
                    ...editFormData,
                    importantNotes: e.target.value,
                  })
                }
                placeholder="Enter important notes"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={3}
                maxLength={500}
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsEditModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              Cancel
            </Button>
            <Button
              onClick={handleUpdateUseCase}
              disabled={isSaving || !editFormData.title?.trim()}
              className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
            >
              {isSaving ? 'Saving...' : 'Save'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB]">
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Use Case</AlertDialogTitle>
            <AlertDialogDescription className="text-[#9CA3AF]">
              Are you sure you want to delete this use case? This action cannot be
              undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]">
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteUseCase}
              disabled={isDeleting}
              className="bg-[#EF4444] text-white hover:bg-[#EF4444]/90"
            >
              {isDeleting ? 'Deleting...' : 'Delete'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Create Task Modal */}
      <Dialog
        open={isCreateTaskModalOpen}
        onOpenChange={setIsCreateTaskModalOpen}
      >
        <DialogContent className="bg-[#161B22] border-[#30363D] text-[#E5E7EB] max-w-2xl">
          <DialogHeader>
            <DialogTitle className="text-2xl font-bold text-[#E5E7EB]">
              Create New Task
            </DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div>
              <Label htmlFor="task-title" className="text-[#E5E7EB]">
                Title <span className="text-[#EF4444]">*</span>
              </Label>
              <Input
                id="task-title"
                value={createTaskFormData.title}
                onChange={(e) =>
                  setCreateTaskFormData({
                    ...createTaskFormData,
                    title: e.target.value,
                  })
                }
                placeholder="Enter task title"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                maxLength={200}
              />
            </div>
            <div>
              <Label htmlFor="task-type" className="text-[#E5E7EB]">
                Type <span className="text-[#EF4444]">*</span>
              </Label>
              <Select
                value={createTaskFormData.type}
                onValueChange={(value) =>
                  setCreateTaskFormData({
                    ...createTaskFormData,
                    type: value as TaskType,
                  })
                }
              >
                <SelectTrigger className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent className="bg-[#161B22] border-[#30363D]">
                  <SelectItem value={TaskType.Documentation}>
                    Documentation
                  </SelectItem>
                  <SelectItem value={TaskType.Feature}>Feature</SelectItem>
                  <SelectItem value={TaskType.Test}>Test</SelectItem>
                  <SelectItem value={TaskType.Bug}>Bug</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label htmlFor="task-description" className="text-[#E5E7EB]">
                Description
              </Label>
              <Textarea
                id="task-description"
                value={createTaskFormData.description}
                onChange={(e) =>
                  setCreateTaskFormData({
                    ...createTaskFormData,
                    description: e.target.value,
                  })
                }
                placeholder="Enter task description"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={4}
                maxLength={2000}
              />
            </div>
            <div>
              <Label htmlFor="task-notes" className="text-[#E5E7EB]">
                Important Notes
              </Label>
              <Textarea
                id="task-notes"
                value={createTaskFormData.importantNotes}
                onChange={(e) =>
                  setCreateTaskFormData({
                    ...createTaskFormData,
                    importantNotes: e.target.value,
                  })
                }
                placeholder="Enter important notes"
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
                rows={3}
                maxLength={1000}
              />
            </div>
            <div>
              <Label htmlFor="task-due-date" className="text-[#E5E7EB]">
                Due Date
              </Label>
              <Input
                id="task-due-date"
                type="date"
                value={createTaskFormData.dueDate}
                onChange={(e) =>
                  setCreateTaskFormData({
                    ...createTaskFormData,
                    dueDate: e.target.value,
                  })
                }
                className="mt-2 bg-[#21262D] border-[#30363D] text-[#E5E7EB] placeholder:text-[#9CA3AF] focus:border-[#8B5CF6]"
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsCreateTaskModalOpen(false)}
              className="bg-[#21262D] border-[#30363D] text-[#E5E7EB] hover:bg-[#30363D]"
            >
              Cancel
            </Button>
            <Button
              onClick={handleCreateTask}
              disabled={isCreatingTask || !createTaskFormData.title.trim()}
              className="bg-[#8B5CF6] text-white hover:bg-[#8B5CF6]/90"
            >
              {isCreatingTask ? 'Creating...' : 'Create'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

